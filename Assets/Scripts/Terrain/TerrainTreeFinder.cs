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
            treeObject.transform.Find("TreeCollider" + treeObject.name);

            // 3. Scale the normalized (0-1) position by the actual size of the terrain
            Vector3 scaledPos = Vector3.Scale(tree.position, terrainData.size);

            // 4. Add the terrain's world position offset to get the final world position
            Vector3 worldTreePos = scaledPos + terrain.transform.position;

            // Optional: Log the position or spawn a game object at the tree location
            Debug.Log($"Tree Index: {tree.prototypeIndex} is at World Position: {worldTreePos}");

            Instantiate(treeCollider, worldTreePos, Quaternion.identity);
        }

        //foreach (TreePrototype TreeProto in terrainData.treePrototypes)
        //{
        //    Debug.Log("Tree name is: " + TreeProto.prefab.name);
        //}
    }
}
