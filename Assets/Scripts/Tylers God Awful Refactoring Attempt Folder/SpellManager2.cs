using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

/// <summary>
/// Manager for handling spell casting and effects.
/// handles cooldowns and resource management for each spell
/// attach to the player and allows them to cast spells based on input and spell definitions.
/// </summary>

public class SpellManager2 : MonoBehaviour
{

    //TODO

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
    public Transform projectilCastOrigin;
    public Transform stationaryCastOrigin;
    [Tooltip("Camera used for aiming. Should be the main camera or a dedicated aiming camera.")]
    [SerializeField] private Camera aimCamera;
    [Tooltip("Reference to the player's animator")]
    [SerializeField] private Animator _animator;
    [Tooltip("Determines what type of spells are cast")]
    public bool inCombatArea = false;
    public bool timerOn = false;
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

    // menu state - prevents attacks/casts while true
    [Header("State")]
    [Tooltip("When true, prevents attempts to basic attack or cast spells (e.g. UI/menu open).")]
    public bool inMenu = false;

    // Coroutine handle for clearing casting state
    private Coroutine _clearCastingCoroutine;

    public bool midCast = false;

    //Checks for CombatArea trigger tag to switch between combat and farm spells
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CombatArea"))
        {
            //tutorial
            if (TutorialManager.instance != null && !TutorialManager.instance.fireIsland)
            {
                //completes billboard 10; go to fire island
                if (TutorialManager.instance.currentBillboard == 9)
                {
                    TutorialManager.instance.ProgressTutorial(10);
                    TutorialManager.instance.fireIsland = true;
                }
            }

            inCombatArea = true;
            if (attackAction != null) attackAction.Enable();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CombatArea"))
        {
            inCombatArea = false;
            if (attackAction != null) attackAction.Disable();
        }
    }

    private void OnEnable()
    {
        specialAttackAction = InputSystem.actions.FindAction("SpecialAttack");
        if (specialAttackAction != null)
        {
            specialAttackAction.started += Attack;
            specialAttackAction.canceled += Attack;
            if (!specialAttackAction.enabled) specialAttackAction.Enable();
        }

        attackAction = InputSystem.actions.FindAction("BasicAttack");
        if (attackAction != null)
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

        if (attackAction != null)
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

        //Making Spell manager a singleton
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

        if (!inCombatArea) infiniteManaRegen = true;
        else infiniteManaRegen = false;

        if (FishingManager.Instance != null && FishingManager.Instance.inFishingMode)
        {
            ChooseSpell();
            UpdateGroundTargetPreview();
            UpdateChargedUI();
            return;
        }
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

            // Fetch the Vector2 scroll delta from the mouse
            Vector2 scrollDelta = Mouse.current.scroll.ReadValue();

            // Check the Y axis value for up/down motion
            if (scrollDelta.y < 0)
            {
                Debug.Log("Scrolling Up!");
                attackChoice++;
                if (attackChoice > 4) attackChoice = 0;
            }
            else if (scrollDelta.y > 0)
            {
                Debug.Log("Scrolling Down!");
                attackChoice--;
                if (attackChoice < 0) attackChoice = 4;
            }
        }
        HUD.instance.UpdateSpellSelection(attackChoice);
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
        //Debug.Log("timer is: " + timer);
    }
    // calls basic attack on attack action
    public void TryBasicAttack(InputAction.CallbackContext context)
    {
        Debug.Log("Trying Basic Attack");
        if (basicAttackPrefab == null)
            return;

        // Prevent basic attack if midCast is true
        if (midCast)
        {
            Debug.Log("Cannot basic attack while midCast is true");
            return;
        }

        // Block basic attacks while in flight
        if (ThirdPersonController.Instance != null && ThirdPersonController.Instance.inFlightMode)
        {
            Debug.Log("Cannot basic attack while in flight");
            return;
        }

        // block while in menus
        if (inMenu)
        {
            Debug.Log("Cannot basic attack while in menu");
            return;
        }

        // block during fishing
        if (FishingManager.Instance.inFishingMode)
        {
            Debug.Log("Cannot basic attack while fishing");
            return;
        }

        /*// block during mining
        if (MiningManager.Instance.isMining)
        {
            Debug.Log("Cannot basic attack while mining");
            return;
        }*/

        // Block while not in combat area
        if (!inCombatArea)
        {
            Debug.Log("Cannot basic attack outside of combat area");
            return;
        }

        

        float now = Time.time;
        if (now < _nextBasicAttackTime)
            return;
        _nextBasicAttackTime = now + basicAttackCooldown;
        AlignPlayerToCamera();

        //THIS IS TEMP basic way to stop shooting when interacting with something. In future want to just call clickselectors function
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, ClickSelector.Instance.raycastDistance))
        {
            Debug.Log("Raycast hit: " + hit.collider.name);
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

        Debug.Log("Basic Attack Cast");

        // Mark controller as casting so flight can't be toggled during this action
        if (ThirdPersonController.Instance != null)
        {
            if (_clearCastingCoroutine != null) StopCoroutine(_clearCastingCoroutine);
            ThirdPersonController.Instance.isCasting = true;
        }

        // Set midCast to true and start coroutine to clear it after attack duration
        midCast = true;
        if (_clearCastingCoroutine != null) StopCoroutine(_clearCastingCoroutine);
        _clearCastingCoroutine = StartCoroutine(ClearCastingAfter(Mathf.Max(0.01f, basicAttackLifetime)));

        //Triggers the spellcast animation
        _animator.SetTrigger("Spellcast");

        Transform origin = projectilCastOrigin != null ? projectilCastOrigin : player.transform;

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

        // Only block NEW spell starts while midCast is true.
        // Do NOT block context.canceled, because release is what finishes the charged spell.
        if (midCast && context.started)
        {
            Debug.Log("Cannot start a new spell while midCast is true");
            return;
        }

        // block while in menus
        if (inMenu)
        {
            Debug.Log("Cannot cast spells while in menu");
            return;
        }

        // block while fishing
        if (FishingManager.Instance.inFishingMode)
        {
            Debug.Log("Cannot cast spells while in fishing");
            return;
        }

        /*// block while mining
        if (MiningManager.Instance.isMining)
        {
            Debug.Log("Cannot cast spells while mining");
            return;
        }*/

        // Block casting if player is currently in flight
        if (ThirdPersonController.Instance != null && ThirdPersonController.Instance.inFlightMode)
        {
            Debug.Log("Cannot cast spells while in flight");
            return;
        }     

        // Start the hold timer on press
        if (context.started)
        {
            // Mark controller as casting so flight can't be toggled during charge
            if (ThirdPersonController.Instance != null)
                ThirdPersonController.Instance.isCasting = true;

            // Set midCast to true (will be cleared after spell cast duration)
            midCast = true;

            // Ensure max allowed tier/timer are updated for the current attackChoice
            UpdateMaxAllowedTierAndTimer();

            // Start timer (Timer() will clamp to _maxAllowedTimer)
            timerOn = true;
            timer = 0f;

            // Triggers the spellcast animation
            _animator.SetTrigger("Spellcast");

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

            // Clamp desired tier to the max allowed for this element to prevent selecting tiers beyond what's unlocked
            tier = Mathf.Min(tier, _maxAllowedTier);

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
                    AlignPlayerToCamera();
                    Cast(chosen);
                }
            }

            // Reset timer state
            timerOn = false;
            timer = 0f;

            // note: do not clear isCasting or midCast here � Cast() (or TryBasicAttack) will clear based on prefab lifetime

            return;
        }
    }

    private void Cast(SO_SpellDefs2 spell)
    {
        float cost;

        if (spell == null)
        {
            ClearCastingStateNow();
            return;
        }

        int elementIdx = (int)spell.spellType;

        Vector3 planarForward = Vector3.ProjectOnPlane(aimCamera.transform.forward, Vector3.up).normalized;
        if (planarForward.sqrMagnitude < 0.0001f) planarForward = aimCamera.transform.forward.normalized;

        if (!inCombatArea)
        {
            Transform farmOriginT = projectilCastOrigin != null ? projectilCastOrigin : player.transform;

            SpellCastContext farmCtx = new()
            {
                caster = player,

                castOrigin = projectilCastOrigin,

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

            cost = tierResourceCosts[currentTier - 1];
            if (!SpendMana(cost, elementIdx))
            {
                ClearCastingStateNow();
                return; // Not enough mana
            }

            // mark casting and schedule clear based on spell lifetime
            if (ThirdPersonController.Instance != null)
            {
                if (_clearCastingCoroutine != null) StopCoroutine(_clearCastingCoroutine);
                ThirdPersonController.Instance.isCasting = true;
            }

            // Set midCast to true and start coroutine to clear it after spell lifetime
            midCast = true;
            if (_clearCastingCoroutine != null) StopCoroutine(_clearCastingCoroutine);
            _clearCastingCoroutine = StartCoroutine(ClearCastingAfter(Mathf.Max(0.01f, spell.Lifetime)));

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
            castOrigin = projectilCastOrigin,
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
        

        cost = tierResourceCosts[currentTier - 1];
        if (!SpendMana(cost, elementIdx))
        {
            ClearCastingStateNow();
            return; // Not enough mana
        }

        // mark casting and schedule clear based on spell lifetime
        if (ThirdPersonController.Instance != null)
        {
            if (_clearCastingCoroutine != null) StopCoroutine(_clearCastingCoroutine);
            ThirdPersonController.Instance.isCasting = true;
        }

        // Set midCast to true and start coroutine to clear it after spell lifetime
        midCast = true;
        if (_clearCastingCoroutine != null) StopCoroutine(_clearCastingCoroutine);
        _clearCastingCoroutine = StartCoroutine(ClearCastingAfter(Mathf.Max(0.01f, spell.Lifetime)));

        spell.CastSpell2(ctx);

        // clears mana text popup for clarity
        if (HUD.instance.manaText.activeInHierarchy)
        {
            HUD.instance.SetManaText();
        }
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
        HUD.instance.manaText.SetActive(true);
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
        // Guard against invalid attackChoice
        if (attackChoice <= 0 || attackChoice > 4)
        {
            _maxAllowedTier = 1;
            _maxAllowedTimer = (tierChargeTimes != null && tierChargeTimes.Length > 0) ? Mathf.Max(0f, tierChargeTimes[0] - 0.0001f) : 0f;
            return;
        }

        int elementIdx = attackChoice - 1;
        bool[] unlockedArray = null;

        switch (elementIdx)
        {
            case 0: unlockedArray = fireTierUnlocked; break;
            case 1: unlockedArray = earthTierUnlocked; break;
            case 2: unlockedArray = waterTierUnlocked; break;
            case 3: unlockedArray = airTierUnlocked; break;
        }

        if (unlockedArray == null || unlockedArray.Length == 0)
        {
            _maxAllowedTier = 1;
            _maxAllowedTimer = (tierChargeTimes != null && tierChargeTimes.Length > 0) ? Mathf.Max(0f, tierChargeTimes[0] - 0.0001f) : 0f;
            return;
        }

        int highestUnlocked = 1;
        for (int i = 0; i < unlockedArray.Length; i++)
        {
            if (unlockedArray[i])
                highestUnlocked = i + 1;
        }

        _maxAllowedTier = highestUnlocked;

        // If the highest unlocked tier is the top tier (4) allow uncapped charging.
        if (_maxAllowedTier >= 4)
        {
            _maxAllowedTimer = float.MaxValue;
            return;
        }

        // Otherwise clamp the timer to just before the next tier threshold so the timer cannot select a higher tier.
        int thresholdIndex = Mathf.Clamp(_maxAllowedTier - 1, 0, Mathf.Max(0, tierChargeTimes.Length - 1));
        float threshold = tierChargeTimes.Length > 0 ? tierChargeTimes[thresholdIndex] : 0f;
        const float clampEpsilon = 0.0001f;
        _maxAllowedTimer = Mathf.Max(0f, threshold - clampEpsilon);
    }

    private void AlignPlayerToCamera()
    {
        if (player == null || aimCamera == null)
            return;

        // Project camera forward onto XZ plane to avoid tilting the player up/down
        Vector3 cameraForward = Vector3.ProjectOnPlane(aimCamera.transform.forward, Vector3.up).normalized;
        if (cameraForward.sqrMagnitude > 0.001f)
            player.transform.forward = cameraForward;
    }

    // Clears the casting lock after the provided duration.
    private System.Collections.IEnumerator ClearCastingAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (ThirdPersonController.Instance != null)
            ThirdPersonController.Instance.isCasting = false;
        midCast = false;
        _clearCastingCoroutine = null;
    }

    // Immediately clears casting lock when casting fails.
    private void ClearCastingStateNow()
    {
        timerOn = false;
        timer = 0f;
        midCast = false;

        if (ThirdPersonController.Instance != null)
            ThirdPersonController.Instance.isCasting = false;

        if (_clearCastingCoroutine != null)
        {
            StopCoroutine(_clearCastingCoroutine);
            _clearCastingCoroutine = null;
        }
    }
}
