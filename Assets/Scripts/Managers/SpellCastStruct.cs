using UnityEngine;

public struct SpellCastContext
{
    public Transform castOrigin;
    public GameObject caster;
    public Camera aimCamera;
    public LayerMask aimMask;
    public float aimDistance;

    public bool inCombatArea;

    public float combatSpawnOffset;
    public float farmSpawnOffset;

    public Vector3 aimPoint;
    public Vector3 aimNormal;
    public bool hasHit;

    public Collider hitCollider;
}
