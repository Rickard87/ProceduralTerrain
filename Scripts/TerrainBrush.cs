public void LowerTerrainAtPosition2(Vector3 position, float loweringPercentage, float radius, Terrain terrain)
{
    GameObject clone = Instantiate(nodePrefab, position, Quaternion.identity);
    clone.name = "lowerClone";
    loweringPercentage /= 100; // Turning percentage into decimals
    float minHeight = 0.5f;

    TerrainData terrainData = terrain.terrainData;

    int centerX = Mathf.RoundToInt((position.x / terrainData.size.x) * terrainData.heightmapResolution);
    int centerZ = Mathf.RoundToInt((position.z / terrainData.size.z) * terrainData.heightmapResolution);

    int radiusInPixelsX = Mathf.RoundToInt(radius * (terrainData.alphamapWidth / terrainData.size.x));
    int radiusInPixelsZ = Mathf.RoundToInt(radius * (terrainData.alphamapHeight / terrainData.size.z));

    float[,] heights = terrainData.GetHeights(centerX - radiusInPixelsX, centerZ - radiusInPixelsZ, radiusInPixelsX * 2, radiusInPixelsZ * 2);
    Color[] brushPixels = funnyBrush.GetPixels();
    Debug.Log(radiusInPixelsX);

    for (int x = 0; x < radiusInPixelsX * 2; x++)
    {
        for (int z = 0; z < radiusInPixelsZ * 2; z++)
        {
            float distanceX = (x - radiusInPixelsX) / (terrainData.alphamapWidth / terrainData.size.x);
            float distanceZ = (z - radiusInPixelsZ) / (terrainData.alphamapHeight / terrainData.size.z);
            float distance = Mathf.Sqrt(distanceX * distanceX + distanceZ * distanceZ);

            float falloff = Mathf.Clamp01(1 - distance / radius);
            float deltaHeight = falloff * loweringPercentage * brushPixels[(int)(distance * 2)].grayscale;


            float newHeight = Mathf.Clamp(heights[x, z] - deltaHeight, minHeight, 1f);

            //newHeight = Mathf.Max(newHeight, minHeight); //new
            Debug.Log(newHeight + " new height"); //1 is all the way down? percentage not working here. Might need to look at grey scale so the for-loops work because it's not supposed to be one value everywhere, in a perfect world.

            heights[x, z] = 0.5f; //setting to 0.5f works. or 0.8f for example. Lowers by 20%
            GameObject linedraw = Instantiate(nodePrefab, new Vector3(position.x, newHeight, position.z), Quaternion.identity);
        }
    }

    terrainData.SetHeights(centerX - radiusInPixelsX, centerZ - radiusInPixelsZ, heights);
    terrain.Flush();
}
