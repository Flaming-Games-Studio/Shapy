using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public GameObject blockPrefab;
    public int clusters = 3; // Number of clusters per level
    public int blocksPerCluster = 6; // Number of blocks per cluster
    public float clusterSpacing = 0.1f; // Spacing between clusters
    public int startLevel = 1;
    public int maxLevel = 5;

    private int currentLevel;

    void Start()
    {
        currentLevel = startLevel;
        SpawnBlocks();
    }

    void SpawnBlocks()
    {
        // Calculate the center position of the block cluster
        Vector2 centerPosition = transform.position;

        for (int i = 0; i < clusters; i++)
        {
            // Calculate the offset for each cluster
            float clusterOffsetX = i * clusterSpacing;

            // Spawn blocks in each cluster
            for (int j = 0; j < blocksPerCluster; j++)
            {
                // Calculate the offset for each block within a cluster
                float blockOffsetX = j * clusterSpacing;

                // Calculate the final spawn position for each block
                Vector2 spawnPosition = centerPosition + new Vector2(clusterOffsetX + blockOffsetX, 0);

                GameObject block = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);

                // Attach the BlockController script and set block HP based on the current level
                BlockController blockController = block.GetComponent<BlockController>();
                if (blockController != null)
                {
                    blockController.SetBlockHP(currentLevel); // Set block HP based on the level
                }
            }
        }
    }

    void Update()
    {
        // Check if the player has completed the current level and increment the level
        // You can trigger this based on player actions or a timer, for example
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentLevel < maxLevel)
            {
                currentLevel++;
                DestroyExistingBlocks(); // Destroy existing blocks before spawning new ones
                SpawnBlocks(); // Spawn new blocks for the updated level
            }
        }
    }

    void DestroyExistingBlocks()
    {
        // Destroy all existing blocks in the scene
        GameObject[] existingBlocks = GameObject.FindGameObjectsWithTag("Block");
        foreach (GameObject block in existingBlocks)
        {
            Destroy(block);
        }
    }
}
