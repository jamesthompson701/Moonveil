using UnityEngine;
using UnityEngine.InputSystem;

public class AttackManager : MonoBehaviour
{
    InputAction attackAction;
    public int attackChoice = 0;

    public SO_Spells[] combatSpells;
    public SO_Spells[] farmSpells;

    [Header("References")]
    public GameObject player;
    [SerializeField] private Transform castOrigin; // Origin point for spell casting
    [SerializeField] private Camera aimCamera;

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

    private bool attackTriggered = false;
    public bool inCombatArea = false;

    Collider hitCol = null;

    private void Awake()
    {
        if (aimCamera == null) aimCamera = Camera.main;
        attackAction = InputSystem.actions.FindAction("Attack");
    }

    private void Update()
    {
        float attack = attackAction.ReadValue<float>();

        // Set attackChoice and corresponding spell based on key input
        if (Input.GetKeyUp(KeyCode.Alpha1)) attackChoice = 1;
        else if (Input.GetKeyUp(KeyCode.Alpha2)) attackChoice = 2;
        else if (Input.GetKeyUp(KeyCode.Alpha3)) attackChoice = 3;
        else if (Input.GetKeyUp(KeyCode.Alpha4)) attackChoice = 4;

        // Call Attack if Fire2 is pressed and attack is not already triggered
        if (attack == 1 && !attackTriggered && attackChoice > 0)
        {
            attackTriggered = true;

            SO_Spells chosen = null;

            // choose SO based on combat/farm
            if (inCombatArea)
            {
                if (attackChoice <= combatSpells.Length) chosen = combatSpells[attackChoice - 1];
            }
            else
            {
                if (attackChoice <= farmSpells.Length) chosen = farmSpells[attackChoice - 1];
            }

            Cast(chosen);
        }

        // Reset attackTriggered when Fire2 is released
        if (attack == 0) attackTriggered = false;
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

        Transform originT = castOrigin != null ? castOrigin : player.transform;
        Vector3 origin = originT.position;

        Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 aimPoint = ray.origin + ray.direction * aimDistance;
        Vector3 aimNormal = Vector3.up;

        bool hasHit = Physics.Raycast(ray, out RaycastHit hit, aimDistance, aimMask, QueryTriggerInteraction.Ignore);
        if (hasHit)
        {
            aimPoint = hit.point;
            aimNormal = hit.normal;
            hitCol = hit.collider;
        }

        SpellCastContext ctx = new SpellCastContext
        {
            caster = player,
            castOrigin = castOrigin,
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

