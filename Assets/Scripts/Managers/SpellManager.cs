using System;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI.Table;

/// <summary>
/// Manages spell casting, resource cooldown, and dodge mechanic.
/// </summary>

public class SpellManager : MonoBehaviour
{
    InputAction attackAction;
    public int attackChoice = 0;

    [Header("Spells")]
    [Tooltip("Assign spells in inspector. Make sure to have the same number of tiers for combat and farm, and to keep the same spell types in corresponding tiers.")]
    public SO_Spells[] combatSpellsTier1 = new SO_Spells[4];
    public SO_Spells[] combatSpellsTier2 = new SO_Spells[4];
    public SO_Spells[] combatSpellsTier3 = new SO_Spells[4];
    public SO_Spells[] combatSpellsTier4 = new SO_Spells[4];
    public SO_Spells[] farmSpellsTier1 = new SO_Spells[4];
    public SO_Spells[] farmSpellsTier2 = new SO_Spells[4];
    public SO_Spells[] farmSpellsTier3 = new SO_Spells[4];
    public SO_Spells[] farmSpellsTier4 = new SO_Spells[4];

    [Header("References")]
    public GameObject player;
    [Tooltip("Targeted by the enemy when attacking")]
    public Transform hitPt;
    [SerializeField] private Transform attackCastOrigin; // Origin point for spell casting
    [SerializeField] private Camera aimCamera;
    [SerializeField] private Transform farmCastOrigin;
    [SerializeField] private Animator _animator;



    [Header("Aiming")]
    [SerializeField] private LayerMask aimMask = ~0; // Layer mask for aiming raycast
    [SerializeField] private float aimDistance = 200f; // matches inspector aim distance, Adjust as needed

    [Header("Spawn")]
    [SerializeField] private float spawnOffset = 1.2f; // Offset distance in front of cast origin for spell spawn

    [Header("Spell Speed")]
    public int avgSpeed = 15;

    [Header("Farm Spell Settings")]
    [SerializeField] private float farmSpawnOffset = 1.2f;

    private bool timerOn = false;
    [SerializeField] private float timer = 0f;
    public bool inCombatArea = false;

    [Serializable]
    private struct ElementPool
    {
        public float current;
        public float nextReadyTime;
    }
    [SerializeField] private float[] maxElementResource = new float[4] { 100f, 100f, 100f, 100f };
    [SerializeField] private float[] rechargeRates = new float[4] { 10f, 10f, 10f, 10f };
    [SerializeField, HideInInspector] private ElementPool[] elementPools = new ElementPool[4];


    [Header("Dodge Settings")]
    [Tooltip("Dodge speed multiplier.")]
    public float dodgeSpeed = 20f;
    [Tooltip("Dodge duration in seconds.")]
    public float dodgeDuration = 0.2f;
    private bool isDodging = false;

    [Header("Basic Attack")]
    [SerializeField] private GameObject basicAttackPrefab;
    [Min(0f)][SerializeField] private float basicAttackCooldown = 0.25f;
    [Min(0f)][SerializeField] private float basicAttackLifetime = 1.0f;
    [Min(0f)][SerializeField] private float basicAttackSpeed = 0f;

    private StarterAssetsInputs _inputs;
    private CharacterController _characterController;

    private float _nextBasicAttackTime;

    private GameObject _earthPreviewInstance;

    private void Awake()
    {
        if (aimCamera == null) aimCamera = Camera.main;
        // Find the action here but subscribe in OnEnable/OnDisable for lifecycle correctness
        attackAction = InputSystem.actions.FindAction("Attack");
        timer = 0;
        if (player == null)
            player = gameObject;

        _inputs = player.GetComponent<StarterAssetsInputs>();
        _characterController = player.GetComponent<CharacterController>();

        for (int i = 0; i < elementPools.Length; i++)
            elementPools[i].current = maxElementResource[i];

        EnsureElementPoolsInitialized();
    }

    private void OnEnable()
    {
        if (attackAction == null) attackAction = InputSystem.actions.FindAction("Attack");
        if (attackAction != null)
        {
            // Subscribe to started (press) and canceled (release)
            attackAction.started += Attack;
            attackAction.canceled += Attack;
            // Ensure the action is enabled so it receives input
            if (!attackAction.enabled) attackAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (attackAction != null)
        {
            attackAction.started -= Attack;
            attackAction.canceled -= Attack;
            if (attackAction.enabled) attackAction.Disable();
        }
    }

    private void Update()
    {
        ChooseSpell();
        Timer();

        RechargeElementPools(Time.deltaTime);

        HandleDodgeInput();
        HandleBasicAttackInput();

        UpdateEarthPreview();
    }

    private void HandleDodgeInput()
    {
        if (_inputs == null)
            return;

        if (_inputs.dodge && !isDodging)
        {
            _inputs.dodge = false; // consume one-shot input
            StartCoroutine(DodgeRoutine());
        }
    }

    private void HandleBasicAttackInput()
    {
        if (_inputs == null)
            return;

        if (_inputs.basicAttack)
        {
            _inputs.basicAttack = false; // consume one-shot input
            TryBasicAttack();
        }
    }

    private System.Collections.IEnumerator DodgeRoutine()
    {
        if (_characterController == null)
            yield break;

        isDodging = true;

        Vector3 dodgeDir = GetCurrentMoveDirectionWorld();
        float elapsed = 0f;

        // CharacterController.Move expects an absolute delta (per call).
        while (elapsed < dodgeDuration)
        {
            float dt = Time.deltaTime;
            elapsed += dt;

            _characterController.Move(dodgeDir * dodgeSpeed * dt);

            yield return null;
        }

        isDodging = false;
    }

    private Vector3 GetCurrentMoveDirectionWorld()
    {
        // Use current move input from StarterAssetsInputs.
        Vector2 move = _inputs != null ? _inputs.move : Vector2.zero;

        // If no input, dodge forward.
        if (move.sqrMagnitude < 0.0001f)
            return player.transform.forward.normalized;

        // Convert input (x,y) to world-space direction using camera yaw.
        Transform cam = aimCamera != null ? aimCamera.transform : Camera.main != null ? Camera.main.transform : null;

        float yaw = cam != null ? cam.eulerAngles.y : player.transform.eulerAngles.y;
        Quaternion yawRotation = Quaternion.Euler(0f, yaw, 0f);

        Vector3 dir = new Vector3(move.x, 0f, move.y);
        dir = yawRotation * dir;

        return dir.sqrMagnitude > 0.0001f ? dir.normalized : player.transform.forward.normalized;
    }

    private void TryBasicAttack()
    {
        if (basicAttackPrefab == null)
            return;

        float now = Time.time;
        if (now < _nextBasicAttackTime)
            return;

        _nextBasicAttackTime = now + basicAttackCooldown;

        Transform origin = attackCastOrigin != null ? attackCastOrigin : player.transform;

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

    //Checks for CombatArea trigger tag to switch between combat and farm spells
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CombatArea"))
        {
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

    // Set attackChoice and corresponding spell based on key input
    public void ChooseSpell()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1)) attackChoice = 1;
        else if (Input.GetKeyUp(KeyCode.Alpha2)) attackChoice = 2;
        else if (Input.GetKeyUp(KeyCode.Alpha3)) attackChoice = 3;
        else if (Input.GetKeyUp(KeyCode.Alpha4)) attackChoice = 4;
    }

    private void UpdateEarthPreview()
    {
        SO_Spells selected = null;

        // Validate attackChoice and retrieve the selected spell
        if (attackChoice > 0 && attackChoice <= 4)
        {
            if (inCombatArea)
            {
                selected = combatSpellsTier1[attackChoice - 1]; // Assuming Tier 1 for simplicity
            }
        }

        bool wantsPreview = selected is EarthAttackSpell earthSpell && earthSpell.previewPrefab != null;

        if (!wantsPreview)
        {
            if (_earthPreviewInstance != null)
            {
                Destroy(_earthPreviewInstance);
                _earthPreviewInstance = null;
            }
            return;
        }

        EarthAttackSpell earth = (EarthAttackSpell)selected;

        if (_earthPreviewInstance == null)
        {
            _earthPreviewInstance = Instantiate(earth.previewPrefab);
        }

        if (!TryGetAimHit(out RaycastHit hit))
        {
            _earthPreviewInstance.SetActive(false);
            return;
        }

        if (!string.IsNullOrWhiteSpace(earth.groundTag) && !hit.collider.CompareTag(earth.groundTag))
        {
            _earthPreviewInstance.SetActive(false);
            return;
        }

        _earthPreviewInstance.SetActive(true);

        Vector3 pos = hit.point + (Vector3.up * earth.GroundYOffset);
        _earthPreviewInstance.transform.position = pos;

        if (earth.AlignToSurfaceNormal)
        {
            Vector3 forwardProjected = Vector3.ProjectOnPlane(GetAimForward(), hit.normal).normalized;
            if (forwardProjected.sqrMagnitude < 0.001f)
                forwardProjected = Vector3.Cross(hit.normal, Vector3.right);

            _earthPreviewInstance.transform.rotation = Quaternion.LookRotation(forwardProjected, hit.normal) * Quaternion.Euler(earth.RotationOffsetEuler);
        }
        else
        {
            _earthPreviewInstance.transform.rotation = Quaternion.LookRotation(GetAimForward(), Vector3.up) * Quaternion.Euler(earth.RotationOffsetEuler);
        }
    }

    private bool TryGetAimHit(out RaycastHit hit)
    {
        hit = default;

        if (aimCamera == null)
            return false;

        Ray ray = new Ray(aimCamera.transform.position, aimCamera.transform.forward);
        bool didHit = Physics.Raycast(ray, out hit, aimDistance, aimMask, QueryTriggerInteraction.Ignore);

        return didHit;
    }

    private Vector3 GetAimForward()
    {
        if (aimCamera != null)
            return aimCamera.transform.forward;

        return player != null ? player.transform.forward : transform.forward;
    }

    public void Attack(InputAction.CallbackContext context)
    {
        // Start the hold timer on press
        if (context.started)
        {
            timerOn = true;
            timer = 0f;
            // Debug.Log("Attack started, timer on.");
            return;
        }

        // On release, decide which tier to cast based on how long the button was held
        if (context.canceled)
        {
            // Determine tier by timer
            int tier;
            if (timer < 2f) tier = 1;
            else if (timer < 4f) tier = 2;
            else if (timer < 6f) tier = 3;
            else tier = 4;

            // Select the appropriate spells array based on combat/farm and tier
            SO_Spells[] selectedArray = null;

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
                SO_Spells chosen = selectedArray[attackChoice - 1];
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

    private void RechargeElementPools(float dt)
    {
        for (int i = 0; i < elementPools.Length; i++)
        {
            elementPools[i].current = Mathf.Min(
                maxElementResource[i],
                elementPools[i].current + rechargeRates[i] * dt
            );
        }
    }

    void Timer()
    {
        if (timerOn)
        {
            timer += Time.deltaTime;
        }
        else timer = 0;
    }

    private void Cast(SO_Spells spell)
    {
        if (spell == null) return;

        int elementIdx = (int)spell.spellType;
        float cost = 0;
        if (elementPools[elementIdx].current < cost)
        {
            Debug.LogWarning("Not enough resource for this spell.");
            return;
        }
        elementPools[elementIdx].current -= cost;


        //Spellcast animation
        _animator.SetTrigger("Spellcast");


        Vector3 planarForward = Vector3.ProjectOnPlane(aimCamera.transform.forward, Vector3.up).normalized;
        if (planarForward.sqrMagnitude < 0.0001f) planarForward = aimCamera.transform.forward.normalized;

        if (!inCombatArea)
        {
            Transform farmOriginT = farmCastOrigin != null ? farmCastOrigin : player.transform;

            SpellCastContext farmCtx = new SpellCastContext
            {
                caster = player,

                attackCastOrigin = attackCastOrigin,
                farmCastOrigin = farmOriginT,

                aimCamera = aimCamera,
                aimMask = aimMask,
                aimDistance = aimDistance,

                inCombatArea = false,

                combatSpawnOffset = spawnOffset,
                farmSpawnOffset = farmSpawnOffset,

                hasHit = false,
                hitCollider = null,
                aimPoint = farmOriginT.position + planarForward * aimDistance,
                aimNormal = Vector3.up,

                cameraPlanarForward = planarForward
            };

            spell.CastSpell(farmCtx);

            return;
        }

        Transform originT = attackCastOrigin != null ? attackCastOrigin : player.transform;

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

        SpellCastContext ctx = new SpellCastContext
        {
            caster = player,
            attackCastOrigin = attackCastOrigin,
            farmCastOrigin = farmCastOrigin,
            aimCamera = aimCamera,
            aimMask = aimMask,
            aimDistance = aimDistance,
            inCombatArea = inCombatArea,
            combatSpawnOffset = spawnOffset,
            farmSpawnOffset = farmSpawnOffset,
            aimPoint = aimPoint,
            aimNormal = aimNormal,
            hasHit = hasHit,
            hitCollider = hitCol
        };

        spell.CastSpell(ctx);
    }

    public void ToggleCombatArea()
    {
        if (inCombatArea)
        {
            inCombatArea = false;
        }
        else
        {
            inCombatArea = true;
        }
    }
    /// <summary>
    /// Returns the current spell tier based on timer.
    /// </summary>
    private int GetCurrentTier()
    {
        if (timer < 2f) return 1;
        if (timer < 4f) return 2;
        if (timer < 6f) return 3;
        return 4;
    }

    /// <summary>
    /// Performs a quick dodge in the current movement direction.
    /// </summary>
    public void Dodge()
    {
        if (isDodging) return;
        StartCoroutine(DodgeCoroutine());
    }

    private System.Collections.IEnumerator DodgeCoroutine()
    {
        isDodging = true;
        Vector3 moveDir = player.GetComponent<ThirdPersonController>().motion.normalized;
        float elapsed = 0f;
        CharacterController controller = player.GetComponent<CharacterController>();
        while (elapsed < dodgeDuration)
        {
            controller.Move(moveDir * dodgeSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        isDodging = false;
    }

    private void EnsureElementPoolsInitialized()
    {
        for (int i = 0; i < elementPools.Length; i++)
            elementPools[i].current = maxElementResource[i];
    }
}

