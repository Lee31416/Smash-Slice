using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chunk 
{
	private ChunkCoord coord;

    private GameObject chunkObject;
    private MeshRenderer meshRenderer;
	private MeshFilter meshFilter;
	private MeshCollider _meshCollider;
	private int _vertexIndex = 0;
	
	private readonly List<Vector3> _vertices = new List<Vector3> ();
	private readonly List<int> _triangles = new List<int> ();
	private readonly List<Vector2> _uvs = new List<Vector2> ();
	private readonly byte[,,] _voxelMap = new byte[VoxelData.k_ChunkWidth, VoxelData.k_ChunkHeight, VoxelData.k_ChunkWidth];
    private readonly World _world;

    private bool _isActive;
    public bool isVoxelMapPopulated;
    
    public bool IsActive
    {
	    get => _isActive;
	    set
	    {
		    _isActive = value;
		    if (chunkObject != null)
		    {
			    chunkObject.SetActive(value);
		    }
	    }
    }

    private Vector3 Position => chunkObject.transform.position;

    public Chunk(ChunkCoord _coord, World _world, bool generateOnLoad) 
    {
	    coord = _coord;
	    this._world = _world;
	    _isActive = true;

	    if (generateOnLoad)
	    {
		    Init();
	    }
    }

    public void Init()
    {
	    chunkObject = new GameObject();
	    meshFilter = chunkObject.AddComponent<MeshFilter>();
	    meshRenderer = chunkObject.AddComponent<MeshRenderer>();
	    meshRenderer.material = this._world.material;
	    _meshCollider = chunkObject.AddComponent<MeshCollider>();
	    chunkObject.transform.SetParent(this._world.transform);
	    chunkObject.transform.position = new Vector3(coord.x * VoxelData.k_ChunkWidth, 0f, coord.z * VoxelData.k_ChunkWidth);
	    chunkObject.name = "Chunk " + coord.x + ", " + coord.z;

	    PopulateVoxelMap();
	    CreateMeshData();
	    CreateMesh();

	    _meshCollider.sharedMesh = meshFilter.mesh;
    }

	private void PopulateVoxelMap() 
	{
		for (var y = 0; y < VoxelData.k_ChunkHeight; y++) 
		{
			for (var x = 0; x < VoxelData.k_ChunkWidth; x++) 
			{
				for (var z = 0; z < VoxelData.k_ChunkWidth; z++) 
				{
                    _voxelMap[x, y, z] = _world.GetVoxel(new Vector3(x, y, z) + Position);
				}
			}
		}

		isVoxelMapPopulated = true;
	}

	private void CreateMeshData() {
		for (var y = 0; y < VoxelData.k_ChunkHeight; y++) 
		{
			for (var x = 0; x < VoxelData.k_ChunkWidth; x++) 
			{
				for (var z = 0; z < VoxelData.k_ChunkWidth; z++) 
				{
                    if (_world.blocktypes[_voxelMap[x,y,z]].isSolid)
					    AddVoxelDataToChunk (new Vector3(x, y, z));
				}
			}
		}
	}

    private bool IsVoxelInChunk(int x, int y, int z)
    {
	    return x >= 0 && x <= VoxelData.k_ChunkWidth - 1 && y >= 0 && y <= VoxelData.k_ChunkHeight - 1 && z >= 0 && z <= VoxelData.k_ChunkWidth - 1;
    }

    public byte GetVoxelFromGlobalVector3(Vector3 pos)
    {
	    var xCheck = Mathf.FloorToInt(pos.x);
	    var yCheck = Mathf.FloorToInt(pos.y);
	    var zCheck = Mathf.FloorToInt(pos.z);

	    xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
	    zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);

	    return _voxelMap[xCheck, yCheck, zCheck];
    }    
    
	private bool CheckVoxel(Vector3 pos) 
	{
		var x = Mathf.FloorToInt (pos.x);
		var y = Mathf.FloorToInt (pos.y);
		var z = Mathf.FloorToInt (pos.z);

		return !IsVoxelInChunk(x,y,z) ? _world.CheckForVoxel(pos + Position) : _world.blocktypes[_voxelMap[x, y, z]].isSolid;
	}

	private void AddVoxelDataToChunk(Vector3 pos)
	{

		for (var p = 0; p < 6; p++)
		{
			if (CheckVoxel(pos + VoxelData.FaceChecks[p])) continue;
			var blockID = _voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];

			_vertices.Add (pos + VoxelData.VoxelVerts [VoxelData.VoxelTris [p, 0]]);
			_vertices.Add (pos + VoxelData.VoxelVerts [VoxelData.VoxelTris [p, 1]]);
			_vertices.Add (pos + VoxelData.VoxelVerts [VoxelData.VoxelTris [p, 2]]);
			_vertices.Add (pos + VoxelData.VoxelVerts [VoxelData.VoxelTris [p, 3]]);

			AddTexture(_world.blocktypes[blockID].GetTextureID(p));

			_triangles.Add (_vertexIndex);
			_triangles.Add (_vertexIndex + 1);
			_triangles.Add (_vertexIndex + 2);
			_triangles.Add (_vertexIndex + 2);
			_triangles.Add (_vertexIndex + 1);
			_triangles.Add (_vertexIndex + 3);
			_vertexIndex += 4;
		}
	}

	private void CreateMesh() 
	{
		var mesh = new Mesh
		{
			vertices = _vertices.ToArray (),
			triangles = _triangles.ToArray (),
			uv = _uvs.ToArray ()
		};

		mesh.RecalculateNormals ();

		meshFilter.mesh = mesh;

	}

    private void AddTexture(int textureID) 
    {
        float y = textureID / VoxelData.k_TextureAtlasSizeInBlocks;
        var x = textureID - (y * VoxelData.k_TextureAtlasSizeInBlocks);

        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;

        y = 1f - y - VoxelData.NormalizedBlockTextureSize;

        _uvs.Add(new Vector2(x, y));
        _uvs.Add(new Vector2(x, y + VoxelData.NormalizedBlockTextureSize));
        _uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y));
        _uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y + VoxelData.NormalizedBlockTextureSize));
    }
}

public class ChunkCoord 
{
    public int x;
    public int z;

    public ChunkCoord()
    {
	    x = 0;
	    z = 0;
    }
    
    public ChunkCoord (int x, int z) 
    {
        this.x = x;
        this.z = z;
    }

    public ChunkCoord(Vector3 pos)
    {
	    var xCheck = Mathf.FloorToInt(pos.x);
	    var zCheck = Mathf.FloorToInt(pos.z);

	    x = xCheck / VoxelData.k_ChunkWidth;
	    z = zCheck / VoxelData.k_ChunkWidth;
    }

    public bool Equals (ChunkCoord other)
    {
	    if (other == null) return false;
	    return other.x == x && other.z == z;
    }

}
