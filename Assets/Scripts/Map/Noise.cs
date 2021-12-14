using UnityEngine;

namespace Map
{
    public static class Noise  
    {
        public static float Get2DPerlin(Vector2 position, float offset, float scale) 
        {
            return Mathf.PerlinNoise((position.x + 0.1f) / VoxelData.k_ChunkWidth * scale + offset, (position.y + 0.1f) / VoxelData.k_ChunkWidth * scale + offset);
        }
    
        public static bool Get3DPerlin(Vector3 position, float offset, float scale, float threshold) 
        {
            var x = (position.x + offset + 0.1f) * scale;
            var y = (position.y + offset + 0.1f) * scale;
            var z = (position.z + offset + 0.1f) * scale;
    
            var AB = Mathf.PerlinNoise(x, y);
            var BC = Mathf.PerlinNoise(y, z);
            var AC = Mathf.PerlinNoise(x, z);
            var BA = Mathf.PerlinNoise(y, x);
            var CB = Mathf.PerlinNoise(z, y);
            var CA = Mathf.PerlinNoise(z, x);
    
            return (AB + BC + AC + BA + CB + CA) / 6f > threshold;
        }
    }
}


