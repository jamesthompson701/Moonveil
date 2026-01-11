using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public int attackChoice = 0;
    public Rigidbody[] attackSpells; // Array of spell prefabs
    public Rigidbody[] farmSpells; // Array of farming spell prefabs

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
    [SerializeField] private float farmWaterSpeed = 2;
    [SerializeField] private float farmAirSpeed = 10;

    private bool attackTriggered = false; // Flag to track if attack is triggered

    public bool inCombatArea = false;

    private void Awake()
    {
        if (aimCamera == null) aimCamera = Camera.main;
    }

    private void Update()
    {
        float attack = Input.GetAxisRaw("Fire2");

        // Set attackChoice and corresponding spell based on key input
        if (Input.GetKeyUp(KeyCode.Alpha1)) attackChoice = 1;
        else if (Input.GetKeyUp(KeyCode.Alpha2)) attackChoice = 2;
        else if (Input.GetKeyUp(KeyCode.Alpha3)) attackChoice = 3;
        else if (Input.GetKeyUp(KeyCode.Alpha4)) attackChoice = 4;

        // Call Attack if Fire2 is pressed and attack is not already triggered
        if (attack == 1 && !attackTriggered && attackChoice > 0 && attackChoice <= attackSpells.Length)
        {
            attackTriggered = true; // Set flag to true to prevent multiple triggers
            if (inCombatArea) Attack(attackChoice, player, attackSpells[attackChoice - 1], avgSpeed);
            else if (!inCombatArea) Attack(attackChoice, player, farmSpells[attackChoice - 1], fastSpeed);
        }

        // Reset attackTriggered when Fire2 is released
        if (attack == 0) attackTriggered = false;
    }

    public void Attack(int attack, GameObject playerObject, Rigidbody spell, int speedMult)
    {
        if (spell == null)
        {
            Debug.LogWarning("No spell assigned for this attack choice.");
            return;
        }

        if (aimCamera == null)
        {
            Debug.LogWarning("No aim camera assigned.");
            return;
        }

        Vector3 origin = castOrigin != null
            ? castOrigin.position
            : playerObject.transform.position + Vector3.up * 1.5f;

        // Aim ray only really needed for COMBAT (and earth placement)
        Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 aimPoint = ray.origin + ray.direction * aimDistance;
        Vector3 aimNormal = Vector3.up;
        bool hasHit = Physics.Raycast(ray, out RaycastHit hit, aimDistance, aimMask, QueryTriggerInteraction.Ignore);

        if (hasHit)
        {
            aimPoint = hit.point;
            aimNormal = hit.normal;
        }

        Rigidbody clone;

        if (inCombatArea)
        {
            // Combat uses camera aim direction
            Vector3 dir = (aimPoint - origin).normalized;
            Vector3 spawnPosition = origin + dir * spawnOffset;
            Quaternion spawnRotation = Quaternion.LookRotation(dir);

            switch (attack)
            {
                case 1: // Fire
                    clone = Instantiate(spell, spawnPosition, spawnRotation);
                    clone.linearVelocity = dir * speedMult;
                    Destroy(clone.gameObject, 3.0f);
                    break;

                case 2: // Earth (spawn on top, grow upward)
                    {
                        if (!hasHit)
                        {
                            Debug.Log("Earth attack needs a ground hit to place correctly.");
                            return;
                        }

                        // Align earth "up" to the surface normal
                        Quaternion earthRot = Quaternion.FromToRotation(Vector3.up, aimNormal);

                        // Find half-height of the prefab so we can place it ON TOP of the ground
                        float halfHeight = 0.5f;
                        Collider prefabCol = spell.GetComponent<Collider>();
                        if (prefabCol != null)
                            halfHeight = prefabCol.bounds.extents.y;

                        // Start position: on the surface, pushed out by half height so it isn't inside the ground
                        Vector3 earthPos = aimPoint + aimNormal * halfHeight;

                        clone = Instantiate(spell, earthPos, earthRot);
                        clone.linearVelocity = Vector3.zero;

                        // Grow upward: increase Y and push up by half the added amount so bottom stays planted
                        float addY = 2.0f;
                        float maxY = 5.0f;

                        Vector3 s = clone.transform.localScale;
                        float oldY = s.y;

                        s.y = Mathf.Min(s.y + addY, maxY);
                        clone.transform.localScale = s;

                        float deltaY = s.y - oldY;
                        if (deltaY > 0f)
                        {
                            // move up along the surface normal so growth doesn't sink into the ground
                            clone.transform.position += aimNormal * (deltaY * 0.5f);
                        }

                        Destroy(clone.gameObject, 1.0f);
                        break;
                    }

                case 3: // Water
                    clone = Instantiate(spell, spawnPosition, spawnRotation);
                    clone.linearVelocity = dir * speedMult;
                    Destroy(clone.gameObject, 5.0f);
                    break;

                case 4: // Air
                    clone = Instantiate(spell, spawnPosition, spawnRotation);
                    clone.linearVelocity = dir * speedMult;
                    Destroy(clone.gameObject, 3.0f);
                    break;
            }
        }
        else
        {
            // FARM: spawn in front of castOrigin, direction based on castOrigin looking
            Transform spawnOrigin = castOrigin != null ? castOrigin : playerObject.transform;

            Vector3 farmDir = spawnOrigin.forward.normalized;

            Vector3 farmSpawnPos = spawnOrigin.position + farmDir * farmSpawnOffset;

            // "Rotation should not change" -> keep prefab rotation:
            Quaternion farmRot = Quaternion.LookRotation(farmDir, Vector3.up);

            switch (attack)
            {
                case 1: // Fire farm - not implemented
                    clone = Instantiate(spell, farmSpawnPos, farmRot);
                    Debug.Log("No fire spell for farming implemented");
                    break;

                case 2: // Earth farm - in front of cast origin
                    clone = Instantiate(spell, farmSpawnPos, farmRot);
                    Destroy(clone.gameObject, 1.0f);
                    break;

                case 3: // Water farm - slower
                    clone = Instantiate(spell, farmSpawnPos, farmRot);
                    clone.linearVelocity = farmDir * farmWaterSpeed;
                    Destroy(clone.gameObject, 1.0f);
                    break;

                case 4: // Air farm
                    clone = Instantiate(spell, farmSpawnPos, farmRot);
                    clone.linearVelocity = farmDir * farmAirSpeed;
                    Destroy(clone.gameObject, 0.5f);
                    break;
            }
        }
    }
}
