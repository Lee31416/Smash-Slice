using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugScreen : MonoBehaviour {

    [SerializeField] private World world;
    [SerializeField] private Text text;

    private float _frameRate;
    private float _timer;
    private int _halfWorldSizeInVoxels;
    private int _halfWorldSizeInChunks;

    private void Start() {
        text = GetComponent<Text>();

        _halfWorldSizeInVoxels = VoxelData.WorldSizeInVoxels / 2;
        _halfWorldSizeInChunks = VoxelData.k_WorldSizeInChunks / 2;
    }

    private void Update() {

        var debugText = "Debug Screen\n";
        debugText += _frameRate + " fps\n\n";
        debugText += "x : " + (Mathf.FloorToInt(world.GetPlayer().transform.position.x) - _halfWorldSizeInVoxels + "\n");
        debugText += "y : " + (Mathf.FloorToInt(world.GetPlayer().transform.position.y)) + "\n";
        debugText += "z : " + (Mathf.FloorToInt(world.GetPlayer().transform.position.z) - _halfWorldSizeInVoxels) + "\n";
        debugText += "Chunk : " + (world.GetPlayerChunkCoord().x - _halfWorldSizeInChunks) + " / " + (world.GetPlayerChunkCoord().z - _halfWorldSizeInChunks);

        text.text = debugText;

        if (_timer > 1f) {

            _frameRate = (int)(1f / Time.unscaledDeltaTime);
            _timer = 0;

        } else
            _timer += Time.deltaTime;

    }
}
