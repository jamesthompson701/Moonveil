using UnityEngine;

public class TerrainTreeFinder : MonoBehaviour
{
    [SerializeField] private GameObject treeCollider;
    [SerializeField] private Terrain terrain;
    
    void Start()
    {
        // 1. Get the active terrain in the scene
        //Terrain terrain = Terrain.activeTerrain;
        //if (terrain == null) return;

        TerrainData terrainData = terrain.terrainData;

        // 2. Loop through every tree instance on the terrain
        foreach (TreeInstance tree in terrainData.treeInstances)
        {
            GameObject treeObject = terrainData.treePrototypes[tree.prototypeIndex].prefab;
            Debug.Log(treeObject.transform.Find("TreeCollider") ? "tree collider made using: " + treeObject.transform.Find("TreeCollider").name : "No collider");
            //Debug.Log("tree collider made using: " + treeObject.name);
            //foreach (Transform child in treeObject.transform)
            //{
            //    Debug.Log("Child Name: " + child.gameObject.name);
            //}

            // 3. Scale the normalized (0-1) position by the actual size of the terrain
            Vector3 scaledPos = Vector3.Scale(tree.position, terrainData.size);

            // 4. Add the terrain's world position offset to get the final world position
            Vector3 worldTreePos = scaledPos + terrain.transform.position;

            // Optional: Log the position or spawn a game object at the tree location
            //Debug.Log($"Tree Index: {tree.prototypeIndex} is at World Position: {worldTreePos}");
            if (treeObject.transform.Find("TreeCollider"))
            {
                GameObject colliderInstance =  Instantiate(treeObject.transform.Find("TreeCollider").gameObject, worldTreePos, Quaternion.identity);
                colliderInstance.SetActive(true);
            }
        }

        //foreach (TreePrototype TreeProto in terrainData.treePrototypes)
        //{
        //    Debug.Log("Tree name is: " + TreeProto.prefab.name);
        //}
    }
}
