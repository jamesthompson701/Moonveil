using UnityEngine;
using UnityEngine.InputSystem;

public class SpellManager : MonoBehaviour
{
    /*
    PSEUDOCODE / PLAN (detailed):

    - On Awake:
      - Ensure aimCamera is assigned (fallback to Camera.main).
      - Try to find the InputAction named "Attack" (don't subscribe here).

    - OnEnable:
      - If attackAction is null attempt to find it.
      - If found, subscribe to attackAction.started and attackAction.canceled with the Attack callback.
      - Enable the action to ensure input is received.

    - OnDisable:
      - If attackAction is set, unsubscribe from started and canceled, and disable the action.

    - Attack(InputAction.CallbackContext context):
      - If context.started:
        - Start the hold timer: set timerOn = true and reset timer = 0.
        - Optionally log debug info.
      - If context.canceled (button released):
        - Determine the tier based on timer:
          - tier 1: timer < 2
          - tier 2: 2 <= timer < 4
          - tier 3: 4 <= timer < 6
          - tier 4: timer >= 6
        - Select the correct spell array based on inCombatArea and the computed tier.
        - Validate attackChoice (must be >= 1 and within the selected array length).
        - If valid, retrieve the SO_Spells at index (attackChoice - 1) and call Cast(spell).
        - If invalid or spell null, log a warning.
        - Reset timerOn and timer, and clear attackTriggered if used.
    - Keep remaining methods (ChooseSpell, Timer, Cast) intact but they will now be triggered by the properly-handled Attack action.
    */

    InputAction attackAction;
    public int attackChoice = 0;

    [Header("Spells")]
    [Tooltip("Assign spells in inspector. Make sure to have the same number of tiers for combat and farm, and to keep the same spell types in corresponding tiers.")]
    public SO_Spells[] combatSpellsTier1;
    public SO_Spells[] combatSpellsTier2;
    public SO_Spells[] combatSpellsTier3;
    public SO_Spells[] combatSpellsTier4;
    public SO_Spells[] farmSpellsTier1;
    public SO_Spells[] farmSpellsTier2;
    public SO_Spells[] farmSpellsTier3;
    public SO_Spells[] farmSpellsTier4;

    [Header("References")]
    public GameObject player;
    public Transform hitPt;
    [SerializeField] private Transform attackCastOrigin; // Origin point for spell casting
    [SerializeField] private Camera aimCamera;
    [SerializeField] private Transform farmCastOrigin;

    [Header("Aiming")]
    [SerializeField] private LayerMask aimMask = ~0; // Layer mask for aiming raycast
    [SerializeField] private float aimDistance = 200f; // matches inspector aim distance, Adjust as needed

    [Header("Spawn")]
    [SerializeField] private float spawnOffset = 1.2f; // Offset distance in front of cast origin for spell spawn

    [Header("Spell Speed")]
    public int avgSpeed = 15;
    public int fastSpeed = 25;

    [Header("Farm Spell Settings")]
    [SerializeField] private float farmSpawnOffset = 1.2f;

    private bool timerOn = false;
    [SerializeField] private float timer = 0f;

    private bool attackTriggered = false;
    public bool inCombatArea = false;

    private void Awake()
    {
        if (aimCamera == null) aimCamera = Camera.main;
        // Find the action here but subscribe in OnEnable/OnDisable for lifecycle correctness
        attackAction = InputSystem.actions.FindAction("Attack");
        timer = 0;
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
    }

    // Set attackChoice and corresponding spell based on key input
    public void ChooseSpell()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1)) attackChoice = 1;
        else if (Input.GetKeyUp(KeyCode.Alpha2)) attackChoice = 2;
        else if (Input.GetKeyUp(KeyCode.Alpha3)) attackChoice = 3;
        else if (Input.GetKeyUp(KeyCode.Alpha4)) attackChoice = 4;
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
                    attackTriggered = true;
                    Cast(chosen);
                    attackTriggered = false;
                }
            }

            // Reset timer state
            timerOn = false;
            timer = 0f;
            return;
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
        if (spell == null)
        {
            Debug.LogWarning("No spell assigned for this choice.");
            return;
        }

        if (aimCamera == null)
        {
            Debug.LogWarning("No aim camera assigned.");
            return;
        }

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
}

