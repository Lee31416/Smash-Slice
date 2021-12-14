using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public static class VoxelData 
    {
    	public const int k_ChunkWidth = 16;
    	public const int k_ChunkHeight = 128;
    	public const int k_WorldSizeInChunks = 20;
    	public const int k_ViewDistanceInChunks = 5;
    	public const int k_TextureAtlasSizeInBlocks = 4;
    	public static float NormalizedBlockTextureSize => 1f / k_TextureAtlasSizeInBlocks;
    	public static int WorldSizeInVoxels => k_WorldSizeInChunks * k_ChunkWidth;
    
    	public static readonly Vector3[] VoxelVerts = {
    		new Vector3(0.0f, 0.0f, 0.0f),
    		new Vector3(1.0f, 0.0f, 0.0f),
    		new Vector3(1.0f, 1.0f, 0.0f),
    		new Vector3(0.0f, 1.0f, 0.0f),
    		new Vector3(0.0f, 0.0f, 1.0f),
    		new Vector3(1.0f, 0.0f, 1.0f),
    		new Vector3(1.0f, 1.0f, 1.0f),
    		new Vector3(0.0f, 1.0f, 1.0f),
    	};
    
    	public static readonly Vector3[] FaceChecks = {
    		new Vector3(0.0f, 0.0f, -1.0f),
    		new Vector3(0.0f, 0.0f, 1.0f),
    		new Vector3(0.0f, 1.0f, 0.0f),
    		new Vector3(0.0f, -1.0f, 0.0f),
    		new Vector3(-1.0f, 0.0f, 0.0f),
    		new Vector3(1.0f, 0.0f, 0.0f)
    	};
    
    	public static readonly int[,] VoxelTris = {
            // Back, Front, Top, Bottom, Left, Right
    
    		{0, 3, 1, 2}, // Back Face
    		{5, 6, 4, 7}, // Front Face
    		{3, 7, 2, 6}, // Top Face
    		{1, 5, 0, 4}, // Bottom Face
    		{4, 7, 0, 3}, // Left Face
    		{1, 2, 5, 6} // Right Face
    
    	};
    
    	public static readonly Vector2[] VoxelUvs = {
    		new Vector2 (0.0f, 0.0f),
    		new Vector2 (0.0f, 1.0f),
    		new Vector2 (1.0f, 0.0f),
    		new Vector2 (1.0f, 1.0f)
    	};
    }
}


