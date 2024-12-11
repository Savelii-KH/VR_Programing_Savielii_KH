using UnityEngine;

public class MapGenerator : MonoBehaviour 
{
    public enum DrawMode { NoiseMap, ColourMap, Mesh}
    public DrawMode drawMode;

    const int chunkSize = 241;

    [Range(0, 6)]
    public int levelOfdetail;

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
        float[,] noiseMap = Noise.GenerateNoiseMap(chunkSize, chunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colorMap = new Color[chunkSize * chunkSize];
        for (int y = 0; y < chunkSize; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                float curentHeight = noiseMap[x, y];

                for (int i = 0; i < regions.Length; i++)
                {
                   if (curentHeight <= regions[i].height)
                   {
                        colorMap[y * chunkSize + x] = regions[i].color;
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
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colorMap, chunkSize, chunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, heightMultiplier, meshHeightCurve, levelOfdetail), TextureGenerator.TextureFromColourMap(colorMap, chunkSize, chunkSize));
        }
    }

    private void OnValidate()
    {
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