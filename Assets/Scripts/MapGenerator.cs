using UnityEngine;

public class MapGenerator : MonoBehaviour 
{
    public enum DrawMode { NoiseMap, ColourMap, Mesh}
    public DrawMode drawMode;

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;

    [Range(0.01f, 5)]
    public float lacunarity;

    [Range(0.01f, 100)]
    public float heightMultiplier;
    public AnimationCurve meshHeightCurve;

    public int seed;
    public Vector2 offset;

    public bool autoUpdate;

    public TerrainType[] regions;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colorMap = new Color[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float curentHeight = noiseMap[x, y];

                for (int i = 0; i < regions.Length; i++)
                {
                   if (curentHeight <= regions[i].height)
                   {
                        colorMap[y * mapWidth + x] = regions[i].color;
                        break;
                   }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap) 
        { 
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap)); 
        }
        else if (drawMode == DrawMode.ColourMap) 
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colorMap, mapWidth, mapHeight));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, heightMultiplier, meshHeightCurve), TextureGenerator.TextureFromColourMap(colorMap, mapWidth, mapHeight));
        }
    }

    private void OnValidate()
    {
        if (mapHeight < 1) { mapHeight = 1; }
        if (mapWidth < 1) { mapWidth = 1; }
        if (noiseScale < 0.0001f) { noiseScale = 0.0001f; }
        if (octaves < 1) { octaves = 1; }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}