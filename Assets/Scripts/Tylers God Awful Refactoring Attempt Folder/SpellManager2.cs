using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manager for handling spell casting and effects.
/// handles cooldowns and resource management for each spell
/// attach to the player and allows them to cast spells based on input and spell definitions.
/// </summary>

public class SpellManager2 : MonoBehaviour
{

    //TODO
    //Implement resource pool for each spell type
    //Implement each spell type as an enum
    //Implement individual cooldowns for each spell type resource
    //Implement a basic attack with no resource cost and a short cooldown
    //Implement input handling for casting spells based on player input and spell definitions
    //Implement combat area and farm area seperation
    //Implement spell swapping and hotkeys for each spell type
    //Implement charge ability to change which spell tier is cast
    //Implement spell tier system where holding the charge ability increases the tier of the spell cast, up to a maximum tier

    //Singleton
    public static SpellManager2 Instance;

    [Header("Spells")]
    [Tooltip("Assign spells in inspector. Make sure to have the same number of tiers for combat and farm, and to keep the same spell types in corresponding tiers.")]
    public SO_SpellDefs2[] combatSpellsTier1 = new SO_SpellDefs2[4];
    public SO_SpellDefs2[] combatSpellsTier2 = new SO_SpellDefs2[4];
    public SO_SpellDefs2[] combatSpellsTier3 = new SO_SpellDefs2[4];
    public SO_SpellDefs2[] combatSpellsTier4 = new SO_SpellDefs2[4];
    public SO_SpellDefs2[] farmSpellsTier1 = new SO_SpellDefs2[4];
    public SO_SpellDefs2[] farmSpellsTier2 = new SO_SpellDefs2[4];
    public SO_SpellDefs2[] farmSpellsTier3 = new SO_SpellDefs2[4];
    public SO_SpellDefs2[] farmSpellsTier4 = new SO_SpellDefs2[4];

    InputAction specialAttackAction;
    InputAction attackAction;

    [Header("Resource Management")]
    [Tooltip("Maximum resource for each element type (fire, earth, water, air). Adjust as needed.")]
    [Min(0f)][SerializeField] private float maxElementResource = 100f;
    [SerializeField] private float rechargeRate = 10f;
    [Min(0f)] public float fireMana;
    [Min(0f)] public float earthMana;
    [Min(0f)] public float waterMana;
    [Min(0f)] public float airMana;

    [Header("Basic Attack")]
    [SerializeField] private GameObject basicAttackPrefab;
    [Min(0f)][SerializeField] private float basicAttackCooldown = 0.25f;
    [Min(0f)][SerializeField] private float basicAttackLifetime = 1.0f;
    [Min(0f)][SerializeField] private float basicAttackSpeed = 0f;
    private float _nextBasicAttackTime;
    [Min(0f)][SerializeField] private float avgSpeed = 15f;

    [Header("References")]
    public GameObject player;
    [Tooltip("This will be targeted by the enemies so keep it somewhere above her waist")]
    public Transform hitPt;
    [Tooltip("This is the point where spells will be cast from.")]
    [SerializeField] private Transform CastOrigin;
    [Tooltip("Camera used for aiming. Should be the main camera or a dedicated aiming camera.")]
    [SerializeField] private Camera aimCamera;
    [Tooltip("Determines what type of spells are cast")]
    public bool inCombatArea = false;
    private bool timerOn = false;
    [SerializeField] private float timer = 0f;
    public int attackChoice = 0;
    private GameObject _SpellPreviewInstance;
    private int currentTier;

    [Header("Aiming")]
    [SerializeField] private LayerMask aimMask = ~0; // Layer mask for aiming raycast
    [SerializeField] private float aimDistance = 200f; // matches inspector aim distance, Adjust as needed
    private SO_SpellDefs2 _lastPreviewedSpell;
    private int _lastPreviewedTier;

    [Header("Spawn")]
    [SerializeField] private float spawnOffset = 1.2f; // Offset distance in front of cast origin for spell spawn

    [Header("Spell Tier References")]
    [Tooltip("Assigns spell tier charge times and resource costs here.")]
    [SerializeField] private float[] tierChargeTimes = new float[3] { 1f, 2f, 3f};
    [SerializeField] private float[] tierResourceCosts = new float[4] { 25f, 50f, 75f, 100f };
    public bool[] fireTierUnlocked = new bool[4] { true, false, false, false };
    public bool[] earthTierUnlocked = new bool[4] { true, false, false, false };
    public bool[] waterTierUnlocked = new bool[4] { true, false, false, false };
    public bool[] airTierUnlocked = new bool[4] { true, false, false, false };
    private int _maxAllowedTier = 1; // Track the highest unlocked tier for the current spell
    private float _maxAllowedTimer = 0f; // Track the max timer for the current spell

    [Header("Cheats/Debug")]
    [SerializeField] private bool infiniteManaRegen = false;
    private float _defaultRechargeRate;

    //Checks for CombatArea trigger tag to switch between combat and farm spells
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CombatArea"))
        {
            //tutorial
            if (TutorialManager.instance != null && !TutorialManager.instance.fireIsland)
            {
                //completes billboard 10; go to fire island
                TutorialManager.instance.ProgressTutorial(10);
                TutorialManager.instance.fireIsland = true;
            }
            inCombatArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CombatArea"))
        {
            inCombatArea = false;
        }
    }

    private void OnEnable()
    {
        specialAttackAction ??= InputSystem.actions.FindAction("SpecialAttack");
        if (specialAttackAction != null)
        {
            specialAttackAction.started += Attack;
            specialAttackAction.canceled += Attack;
            if (!specialAttackAction.enabled) specialAttackAction.Enable();
        }

        attackAction ??= InputSystem.actions.FindAction("BasicAttack");
        if (attackAction != null && inCombatArea)
        {
            attackAction.performed += TryBasicAttack;
            if (!attackAction.enabled) attackAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (specialAttackAction != null)
        {
            specialAttackAction.started -= Attack;
            specialAttackAction.canceled -= Attack;
            if (specialAttackAction.enabled) specialAttackAction.Disable();
        }

        if (attackAction != null && inCombatArea)
        {
            attackAction.performed -= TryBasicAttack;
            if (attackAction.enabled) attackAction.Disable();
        }
    }

    private void Awake()
    {
        specialAttackAction = InputSystem.actions.FindAction("SpecialAttack");
        attackAction = InputSystem.actions.FindAction("BasicAttack");
        // Initialize element resource pools
        fireMana = maxElementResource;
        earthMana = maxElementResource;
        waterMana = maxElementResource;
        airMana = maxElementResource;

        _defaultRechargeRate = rechargeRate; // Store the default recharge rate

        if (aimCamera == null) aimCamera = Camera.main;
        // Find the action here but subscribe in OnEnable/OnDisable for lifecycle correctness
       //attackAction = InputSystem.actions.FindAction("Attack");
        timer = 0;
        if (player == null)
            player = gameObject;

        //Making canvas manager a singleton
        if (Instance != null && Instance != this)
        {
            Debug.Log("Destroy New Spell Manager");
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        DevMana();
        Timer();
        RechargeElementPools(Time.deltaTime);
        ChooseSpell();
        UpdateGroundTargetPreview();
        UpdateChargedUI();
    }

    public void ChooseSpell()
    {
        // Only allow spell switching if not holding the attack button
        if (!timerOn)
        {
            if (Input.GetKeyUp(KeyCode.Alpha1)) attackChoice = 1;
            else if (Input.GetKeyUp(KeyCode.Alpha2)) attackChoice = 2;
            else if (Input.GetKeyUp(KeyCode.Alpha3)) attackChoice = 3;
            else if (Input.GetKeyUp(KeyCode.Alpha4)) attackChoice = 4;
        }
    }

    void Timer()
    {
        if (timerOn)
        {
            // Clamp timer to the max allowed for the current spell
            timer += Time.deltaTime;
            if (timer > _maxAllowedTimer)
                timer = _maxAllowedTimer;
        }
        else
        {
            timer = 0;
        }
    }
    // calls basic attack on attack action
    public void TryBasicAttack(InputAction.CallbackContext context)
    {
        if (basicAttackPrefab == null)
            return;

        float now = Time.time;
        if (now < _nextBasicAttackTime)
            return;
        _nextBasicAttackTime = now + basicAttackCooldown;


        //THIS IS TEMP basic way to stop shooting when interacting with something. In future want to just call clickselectors function
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, ClickSelector.Instance.raycastDistance))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable == null)
            {
                interactable = hit.collider.GetComponentInParent<Interactable>();
            }
            if (interactable != null)
            {
                Debug.Log("Can not basic attack while interacting");
                return;
            }
        }


        Transform origin = CastOrigin != null ? CastOrigin : player.transform;

        Vector3 spawnPos = origin.position + origin.forward * spawnOffset;
        Quaternion spawnRot = Quaternion.LookRotation(GetAimForward(), Vector3.up);

        GameObject spawned = Instantiate(basicAttackPrefab, spawnPos, spawnRot);

        float speed = basicAttackSpeed > 0f ? basicAttackSpeed : avgSpeed;
        ApplyInitialVelocity(spawned, spawnRot * Vector3.forward, speed);

        if (basicAttackLifetime > 0f)
            Destroy(spawned, basicAttackLifetime);
    }

    private void ApplyInitialVelocity(GameObject obj, Vector3 direction, float speed)
    {
        if (obj.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = direction * speed;
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        // Start the hold timer on press
        if (context.started)
        {
            timerOn = true;
            timer = 0f;
            UpdateMaxAllowedTierAndTimer();
            return;
        }

        // On release, decide which tier to cast based on how long the button was held
        if (context.canceled)
        {
            // Determine tier by timer
            int tier;
            if (timer < tierChargeTimes[0]) tier = 1;
            else if (timer < tierChargeTimes[1]) tier = 2;
            else if (timer < tierChargeTimes[2]) tier = 3;
            else tier = 4;

            //reference unlocked tiers for the current element type and adjust tier if necessary
            if (attackChoice > 0 && attackChoice <= 4)
            {
                int elementIdx = attackChoice - 1;
                bool tierUnlocked = false;
                switch (elementIdx)
                {
                    case 0: tierUnlocked = fireTierUnlocked[tier - 1]; break;
                    case 1: tierUnlocked = earthTierUnlocked[tier - 1]; break;
                    case 2: tierUnlocked = waterTierUnlocked[tier - 1]; break;
                    case 3: tierUnlocked = airTierUnlocked[tier - 1]; break;
                }
                if (!tierUnlocked)
                {
                    // Find the highest unlocked tier less than the desired tier
                    bool[] unlockedArray = null;
                    switch (elementIdx)
                    {
                        case 0: unlockedArray = fireTierUnlocked; break;
                        case 1: unlockedArray = earthTierUnlocked; break;
                        case 2: unlockedArray = waterTierUnlocked; break;
                        case 3: unlockedArray = airTierUnlocked; break;
                    }
                    int highestUnlocked = 1;
                    for (int i = 0; i < tier; i++)
                    {
                        if (unlockedArray != null && unlockedArray[i])
                            highestUnlocked = i + 1;
                    }
                    if (highestUnlocked < tier)
                    {
                        Debug.LogWarning($"Tier {tier} for element index {elementIdx} is not unlocked. Using highest unlocked tier {highestUnlocked}.");
                    }
                    tier = highestUnlocked;
                }

            }
            else
            {
                Debug.LogWarning("No attack choice selected. Defaulting to first element.");
                attackChoice = 1; // Default to first element if no valid choice
            }

            // Select the appropriate spells array based on combat/farm and tier
            SO_SpellDefs2[] selectedArray = null;
            currentTier = tier; // Store current tier for reference purposes

            if (inCombatArea)
            {
                switch (tier)
                {
                    case 1: selectedArray = combatSpellsTier1; break;
                    case 2: selectedArray = combatSpellsTier2; break;
                    case 3: selectedArray = combatSpellsTier3; break;
                    case 4: selectedArray = combatSpellsTier4; break;
                }
            }
            else
            {
                switch (tier)
                {
                    case 1: selectedArray = farmSpellsTier1; break;
                    case 2: selectedArray = farmSpellsTier2; break;
                    case 3: selectedArray = farmSpellsTier3; break;
                    case 4: selectedArray = farmSpellsTier4; break;
                }
            }

            // Validate attackChoice and array bounds
            if (attackChoice <= 0)
            {
                Debug.LogWarning("No attack choice selected.");
            }
            else if (selectedArray == null || selectedArray.Length == 0)
            {
                Debug.LogWarning($"No spells assigned for tier {tier} in {(inCombatArea ? "combat" : "farm")}.");
            }
            else if (attackChoice - 1 < 0 || attackChoice - 1 >= selectedArray.Length)
            {
                Debug.LogWarning($"Attack choice {attackChoice} is out of range for tier {tier}. Array length: {selectedArray.Length}");
            }
            else
            {
                SO_SpellDefs2 chosen = selectedArray[attackChoice - 1];
                if (chosen == null)
                {
                    Debug.LogWarning("Selected spell is null.");
                }
                else
                {
                    Cast(chosen);
                }
            }

            // Reset timer state
            timerOn = false;
            timer = 0f;
            return;
        }
    }

    private void Cast(SO_SpellDefs2 spell)
    {
        if (spell == null) return;

        int elementIdx = (int)spell.spellType;

        Vector3 planarForward = Vector3.ProjectOnPlane(aimCamera.transform.forward, Vector3.up).normalized;
        if (planarForward.sqrMagnitude < 0.0001f) planarForward = aimCamera.transform.forward.normalized;

        if (!inCombatArea)
        {
            Transform farmOriginT = CastOrigin != null ? CastOrigin : player.transform;

            SpellCastContext farmCtx = new()
            {
                caster = player,

                castOrigin = CastOrigin,

                aimCamera = aimCamera,
                aimMask = aimMask,
                aimDistance = aimDistance,

                inCombatArea = false,

                spawnOffset = spawnOffset,

                hasHit = false,
                hitCollider = null,
                aimPoint = farmOriginT.position + planarForward * aimDistance,
                aimNormal = Vector3.up,

                cameraPlanarForward = planarForward
            };

            spell.CastSpell2(farmCtx);

            return;
        }

        Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 aimPoint = ray.origin + ray.direction * aimDistance;
        Vector3 aimNormal = Vector3.up;

        bool hasHit = Physics.Raycast(ray, out RaycastHit hit, aimDistance, aimMask, QueryTriggerInteraction.Ignore);
        Collider hitCol = null;

        if (hasHit)
        {
            aimPoint = hit.point;
            aimNormal = hit.normal;
            hitCol = hit.collider;
        }

        SpellCastContext ctx = new()
        {
            caster = player,
            castOrigin = CastOrigin,
            aimCamera = aimCamera,
            aimMask = aimMask,
            aimDistance = aimDistance,
            inCombatArea = inCombatArea,
            spawnOffset = spawnOffset,
            aimPoint = aimPoint,
            aimNormal = aimNormal,
            hasHit = hasHit,
            hitCollider = hitCol
        };

        float cost = tierResourceCosts[currentTier - 1];
        if (!SpendMana(cost, elementIdx))
            return; // Not enough mana

        spell.CastSpell2(ctx);
    }

    // Recharge all element pools
    private void RechargeElementPools(float dt)
    {
        fireMana = Mathf.Min(maxElementResource, fireMana + rechargeRate * dt);
        earthMana = Mathf.Min(maxElementResource, earthMana + rechargeRate * dt);
        waterMana = Mathf.Min(maxElementResource, waterMana + rechargeRate * dt);
        airMana = Mathf.Min(maxElementResource, airMana + rechargeRate * dt);


        HUD.instance.UpdateManaDisplay(new float[]{ fireMana / maxElementResource, earthMana / maxElementResource, waterMana / maxElementResource, airMana / maxElementResource });
    }

    // Spend mana from the correct pool
    private bool SpendMana(float cost, int elementIdx)
    {
        switch (elementIdx)
        {
            case 0: // Fire
                if (fireMana >= cost) { fireMana -= cost; return true; }
                break;
            case 1: // Earth
                if (earthMana >= cost) { earthMana -= cost; return true; }
                break;
            case 2: // Water
                if (waterMana >= cost) { waterMana -= cost; return true; }
                break;
            case 3: // Air
                if (airMana >= cost) { airMana -= cost; return true; }
                break;
        }
        Debug.LogWarning("Not enough mana to cast the spell.");
        return false;
    }

    private void UpdateChargedUI()
    {

        int tier;

        // Determine current tier based on timer
        if (timer == 0) tier = 0;
        else if (timer < tierChargeTimes[0]) tier = 1;
        else if (timer < tierChargeTimes[1]) tier = 2;
        else if (timer < tierChargeTimes[2]) tier = 3;
        else tier = 4;

        HUD.instance.UpdatedSpellCharge(tier);


    }
    private void UpdateGroundTargetPreview()
    {
        SO_SpellDefs2 selected = null;
        int tier = 1;

        // Determine current tier based on timer
        if (timer < tierChargeTimes[0]) tier = 1;
        else if (timer < tierChargeTimes[1]) tier = 2;
        else if (timer < tierChargeTimes[2]) tier = 3;
        else tier = 4;

        // Validate attackChoice and retrieve the selected spell
        if (attackChoice > 0 && attackChoice <= 4)
        {
            if (inCombatArea)
            {
                switch (tier)
                {
                    case 1: selected = combatSpellsTier1[attackChoice - 1]; break;
                    case 2: selected = combatSpellsTier2[attackChoice - 1]; break;
                    case 3: selected = combatSpellsTier3[attackChoice - 1]; break;
                    case 4: selected = combatSpellsTier4[attackChoice - 1]; break;
                }
            }
            else
            {
                switch (tier)
                {
                    case 1: selected = farmSpellsTier1[attackChoice - 1]; break;
                    case 2: selected = farmSpellsTier2[attackChoice - 1]; break;
                    case 3: selected = farmSpellsTier3[attackChoice - 1]; break;
                    case 4: selected = farmSpellsTier4[attackChoice - 1]; break;
                }
            }
        }

        bool wantsPreview = selected is GroundTargetSpells earthSpell && earthSpell.previewPrefab != null;

        // If the spell or tier has changed, destroy the old preview instance
        if (selected != _lastPreviewedSpell || tier != _lastPreviewedTier)
        {
            if (_SpellPreviewInstance != null)
            {
                Destroy(_SpellPreviewInstance);
                _SpellPreviewInstance = null;
            }
            _lastPreviewedSpell = selected;
            _lastPreviewedTier = tier;
        }

        if (!wantsPreview)
        {
            if (_SpellPreviewInstance != null)
            {
                Destroy(_SpellPreviewInstance);
                _SpellPreviewInstance = null;
            }
            return;
        }

        GroundTargetSpells spell = (GroundTargetSpells)selected;

        if (_SpellPreviewInstance == null)
        {
            _SpellPreviewInstance = Instantiate(spell.previewPrefab);
        }

        if (!TryGetAimHit(out RaycastHit hit))
        {
            _SpellPreviewInstance.SetActive(false);
            return;
        }

        if (!string.IsNullOrWhiteSpace(spell.groundTag) && !hit.collider.CompareTag(spell.groundTag))
        {
            _SpellPreviewInstance.SetActive(false);
            return;
        }

        _SpellPreviewInstance.SetActive(true);

        Vector3 pos = hit.point + (Vector3.up * spell.GroundYOffset);
        _SpellPreviewInstance.transform.position = pos;

        if (spell.AlignToSurfaceNormal)
        {
            Vector3 forwardProjected = Vector3.ProjectOnPlane(GetAimForward(), hit.normal).normalized;
            if (forwardProjected.sqrMagnitude < 0.001f)
                forwardProjected = Vector3.Cross(hit.normal, Vector3.right);

            _SpellPreviewInstance.transform.rotation = Quaternion.LookRotation(forwardProjected, hit.normal) * Quaternion.Euler(spell.RotationOffsetEuler);
        }
        else
        {
            _SpellPreviewInstance.transform.rotation = Quaternion.LookRotation(GetAimForward(), Vector3.up) * Quaternion.Euler(spell.RotationOffsetEuler);
        }
    }

    private bool TryGetAimHit(out RaycastHit hit)
    {
        hit = default;

        if (aimCamera == null)
            return false;

        Ray ray = new(aimCamera.transform.position, aimCamera.transform.forward);
        bool didHit = Physics.Raycast(ray, out hit, aimDistance, aimMask, QueryTriggerInteraction.Ignore);

        return didHit;
    }

    private Vector3 GetAimForward()
    {
        if (aimCamera != null)
            return aimCamera.transform.forward;

        return player != null ? player.transform.forward : transform.forward;
    }

    private void DevMana()
    {
        if (infiniteManaRegen)
        {
            rechargeRate = 900f;
        }
        else
        {
            rechargeRate = _defaultRechargeRate;
        }
    }

    private void UpdateMaxAllowedTierAndTimer()
    {
        int elementIdx = attackChoice - 1;
        bool[] unlockedArray = null;

        switch (elementIdx)
        {
            case 0: unlockedArray = fireTierUnlocked; break;
            case 1: unlockedArray = earthTierUnlocked; break;
            case 2: unlockedArray = waterTierUnlocked; break;
            case 3: unlockedArray = airTierUnlocked; break;
        }

        int highestUnlocked = 1;
        for (int i = 0; i < unlockedArray.Length; i++)
        {
            if (unlockedArray[i])
                highestUnlocked = i + 1;
        }
        _maxAllowedTier = highestUnlocked;
        // Clamp to available charge times
        int chargeIdx = Mathf.Clamp(_maxAllowedTier - 1, 0, tierChargeTimes.Length - 1);
        _maxAllowedTimer = tierChargeTimes[chargeIdx];
    }
}
