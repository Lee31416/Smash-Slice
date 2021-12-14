using Map;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class DebugScreen : MonoBehaviour 
    {
        [SerializeField] private Text text;
        [SerializeField] private Transform _player;
    
        private float _frameRate;
        private float _timer;
        private int _halfWorldSizeInVoxels;
        private int _halfWorldSizeInChunks;
        private World _world;
    
        private void Awake() 
        {
            text = GetComponent<Text>();
            _world = GameObject.Find("World").GetComponent<World>();
            
            _halfWorldSizeInVoxels = VoxelData.WorldSizeInVoxels / 2;
            _halfWorldSizeInChunks = VoxelData.k_WorldSizeInChunks / 2;
        }
    
        private void Update() 
        {
            if (_player == null) return;
            
            var debugText = "Debug Screen\n";
            debugText += _frameRate + " fps\n\n";
            debugText += "x : " + (Mathf.FloorToInt(_player.position.x) - _halfWorldSizeInVoxels + "\n");
            debugText += "y : " + (Mathf.FloorToInt(_player.position.y)) + "\n";
            debugText += "z : " + (Mathf.FloorToInt(_player.position.z) - _halfWorldSizeInVoxels) + "\n";
            debugText += "Chunk : " + (_world.GetPlayerChunkCoord().x - _halfWorldSizeInChunks) + " / " + (_world.GetPlayerChunkCoord().z - _halfWorldSizeInChunks);
    
            text.text = debugText;
    
            if (_timer > 1f) 
            {
                _frameRate = (int)(1f / Time.unscaledDeltaTime);
                _timer = 0;
    
            }
            else
            {
                _timer += Time.deltaTime;
            }
        }
    }
}


