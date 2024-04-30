using UnityEngine;

public class Noise
{
    public static float[] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, UnityEngine.Vector2 offset)
    {
        float[] noiseMap = new float[mapWidth * mapHeight];
        var random = new System.Random(seed);
        var randomInt = random.Next(seed);



        if (octaves <1)
        {
            octaves = 1;
        }

        UnityEngine.Vector2[] octaveOffsets = new UnityEngine.Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = random.Next(-100000, 100000) + offset.x;
            float offsetY = random.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new UnityEngine.Vector2(offsetX, offsetY);
        }

        if (scale <= 0f)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int x = 0, y; x < mapWidth; x++)
        {
            for (y = 0; y < mapHeight; y++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; ++i)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight)
                    minNoiseHeight = noiseHeight;

                noiseMap[y * mapWidth + x] = noiseHeight;
            }
        }

        for (int x = 0, y; x < mapWidth; x++ )
        {
            for (y = 0; y < mapHeight; y++)
            {
                noiseMap[y * mapWidth + x] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[y * mapWidth + x]);
            }

        }

        return noiseMap;
    }

    public static float[] GenerateIslandGradientMap(int mapWidth, int mapHeight)
    {
        float[] map = new float[mapWidth * mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for(int y = 0; y < mapHeight; y++)
            {
                float i = x / (float)mapWidth * 2 - 1;
                float j = y / (float)mapHeight * 2 - 1;
                float value = Mathf.Max(Mathf.Abs(i), Mathf.Abs(j));
                float a = 3;
                float b = 2.2f;
                float islandGradientValue = Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));

                map[y * mapWidth + x] = islandGradientValue;
            }
        }
        return map;
    }

}