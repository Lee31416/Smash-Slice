using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Map
{
    public class World : MonoBehaviour {
    
        [SerializeField] private int seed;
        [SerializeField] private BiomeAttributes biome;
        [SerializeField] private Transform player;
        [SerializeField] private Vector3 spawnPosition;
        
        public Material material;
        public BlockType[] blocktypes;
    
        private Chunk[,] chunks = new Chunk[VoxelData.k_WorldSizeInChunks, VoxelData.k_WorldSizeInChunks];
        private List<ChunkCoord> activeChunks = new List<ChunkCoord>();
        private ChunkCoord _playerChunkCoord;
        private ChunkCoord _playerLastChunkCoord;
        private bool _isCreatingChunks;
        private List<ChunkCoord> _chunksToCreate = new List<ChunkCoord>();
        private BoxCollider _worldCollider;
    
        private void Awake() 
        {
            Random.InitState(seed);

            spawnPosition = new Vector3((VoxelData.k_WorldSizeInChunks * VoxelData.k_ChunkWidth) / 2f, VoxelData.k_ChunkHeight + 2f, (VoxelData.k_WorldSizeInChunks * VoxelData.k_ChunkWidth) / 2f);
            GenerateWorld();
            _playerLastChunkCoord = GetChunkCoordFromVector3(player.position);
            SetWorldBoundaries();
        }
    
        private void SetWorldBoundaries()
        {
            _worldCollider = gameObject.AddComponent<BoxCollider>();
            _worldCollider.size = new Vector3((VoxelData.k_ChunkWidth * VoxelData.k_WorldSizeInChunks) - 48, 120, (VoxelData.k_ChunkWidth * VoxelData.k_WorldSizeInChunks) - 48);
            _worldCollider.center = new Vector3((VoxelData.k_ChunkWidth * VoxelData.k_WorldSizeInChunks) / 2, 60, (VoxelData.k_ChunkWidth * VoxelData.k_WorldSizeInChunks) / 2);
            _worldCollider.isTrigger = true;
        }
    
        public ChunkCoord GetPlayerChunkCoord()
        {
            return _playerChunkCoord;
        }
    
        private void Update() 
        {
            UpdateChunks(player.transform);
        }
    
        private void UpdateChunks(Transform player)
        {
            _playerChunkCoord = GetChunkCoordFromVector3(player.position);
    
            // Only update the chunks if the player has moved from the chunk they were previously on.
            if (!_playerChunkCoord.Equals(_playerLastChunkCoord))
            {
                CheckViewDistance(player);
            }
    
            if (_chunksToCreate.Count > 0 && !_isCreatingChunks)
            {
                StartCoroutine(CreateChunks());
            }
        }
    
        private void GenerateWorld () 
        {
            for (var x = (VoxelData.k_WorldSizeInChunks / 2) - VoxelData.k_ViewDistanceInChunks; x < (VoxelData.k_WorldSizeInChunks / 2) + VoxelData.k_ViewDistanceInChunks; x++) 
            {
                for (var z = (VoxelData.k_WorldSizeInChunks / 2) - VoxelData.k_ViewDistanceInChunks; z < (VoxelData.k_WorldSizeInChunks / 2) + VoxelData.k_ViewDistanceInChunks; z++)
                {
                    chunks[x, z] = new Chunk(new ChunkCoord(x,z), this, true);
                    activeChunks.Add(new ChunkCoord(x, z));
                }
            }
    
       
            player.position = spawnPosition;
        }
    
        private IEnumerator CreateChunks()
        {
            _isCreatingChunks = true;
    
            while (_chunksToCreate.Count > 0)
            {
                chunks[_chunksToCreate[0].x, _chunksToCreate[0].z].Init();
                _chunksToCreate.RemoveAt(0);
                yield return null;
            }
            
            _isCreatingChunks = false;
        }
    
        private ChunkCoord GetChunkCoordFromVector3(Vector3 pos) 
        {
            var x = Mathf.FloorToInt(pos.x / VoxelData.k_ChunkWidth);
            var z = Mathf.FloorToInt(pos.z / VoxelData.k_ChunkWidth);
            return new ChunkCoord(x, z);
        }
    
        private void CheckViewDistance(Transform player) 
        {
            var coord = GetChunkCoordFromVector3(player.position);
            _playerLastChunkCoord = _playerChunkCoord;
            var previouslyActiveChunks = new List<ChunkCoord>(activeChunks);
    
            // Loop through all chunks currently within view distance of the player.
            for (var x = coord.x - VoxelData.k_ViewDistanceInChunks; x < coord.x + VoxelData.k_ViewDistanceInChunks; x++) 
            {
                for (var z = coord.z - VoxelData.k_ViewDistanceInChunks; z < coord.z + VoxelData.k_ViewDistanceInChunks; z++) 
                {
                    // If the current chunk is in the world...
                    if (IsChunkInWorld(new ChunkCoord (x, z))) 
                    {
                        // Check if it active, if not, activate it.
                        if (chunks[x, z] == null)
                        {
                            chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, false);
                            _chunksToCreate.Add(new ChunkCoord(x, z));
                        }
                        else if (!chunks[x, z].IsActive) 
                        {
                            chunks[x, z].IsActive = true;
                        }
                        activeChunks.Add(new ChunkCoord(x, z));
                    }
    
                    // Check through previously active chunks to see if this chunk is there. If it is, remove it from the list.
                    for (var i = 0; i < previouslyActiveChunks.Count; i++) 
                    {
                        if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z)))
                        {
                            previouslyActiveChunks.RemoveAt(i);
                        }
                    }
    
                }
            }
    
            // Any chunks left in the previousActiveChunks list are no longer in the player's view distance, so loop through and disable them.
            foreach (var c in previouslyActiveChunks)
                chunks[c.x, c.z].IsActive = false;
        }
    
        public bool CheckForVoxel(Vector3 pos)
        {
            var thisChunk = new ChunkCoord(pos);
            
            if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.k_ChunkHeight)
            {
                return false;
            }
    
            if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
            {
                return blocktypes[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].isSolid;
            }
    
            return blocktypes[GetVoxel(pos)].isSolid;
        }
    
        public byte GetVoxel(Vector3 pos) 
        {
    
            var yPos = Mathf.FloorToInt(pos.y);
    
            /* IMMUTABLE PASS */
    
            // If outside world, return air.
            if (!IsVoxelInWorld(pos))
                return 0;
    
            // If bottom block of chunk, return bedrock.
            if (yPos == 0)
                return 1;
    
            /* BASIC TERRAIN PASS */
    
            var terrainHeight = Mathf.FloorToInt(biome.terrainHeight * Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.terrainScale)) + biome.solidGroundHeight;
            byte voxelValue = 0;
    
            if (yPos == terrainHeight)
                voxelValue = 3;
            else if (yPos < terrainHeight && yPos > terrainHeight - 4)
                voxelValue = 5;
            else if (yPos > terrainHeight)
                return 0;
            else
                voxelValue = 2;
    
            /* SECOND PASS */
    
            if (voxelValue != 2) return voxelValue;
            foreach (var lode in biome.lodes)
            {
                if (yPos <= lode.minHeight || yPos >= lode.maxHeight) continue;
                if (Noise.Get3DPerlin(pos, lode.noiseOffset, lode.scale, lode.threshold))
                    voxelValue = lode.blockID;
            }
    
            return voxelValue;
        }
    
        private bool IsChunkInWorld(ChunkCoord coord)
        {
            return coord.x > 0 && coord.x < VoxelData.k_WorldSizeInChunks - 1 && coord.z > 0 && coord.z < VoxelData.k_WorldSizeInChunks - 1;
        }
    
        private bool IsVoxelInWorld(Vector3 pos)
        {
            return pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.k_ChunkHeight && pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels;
        }
    
        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<CharacterController>() != null)
            {
                other.transform.Translate(Vector3.forward * -5);
            }
        }
    }
    
    [System.Serializable]
    public class BlockType 
    {
        public string blockName;
        public bool isSolid;
    
        [Header("Texture Values")]
        public int backFaceTexture;
        public int frontFaceTexture;
        public int topFaceTexture;
        public int bottomFaceTexture;
        public int leftFaceTexture;
        public int rightFaceTexture;
    
        public int GetTextureID (int faceIndex) 
        {
            switch (faceIndex) 
            {
                case 0:
                    return backFaceTexture;
                case 1:
                    return frontFaceTexture;
                case 2:
                    return topFaceTexture;
                case 3:
                    return bottomFaceTexture;
                case 4:
                    return leftFaceTexture;
                case 5:
                    return rightFaceTexture;
                default:
                    Debug.Log("Error in GetTextureID; invalid face index");
                    return 0;
            }
        }
    }

}

