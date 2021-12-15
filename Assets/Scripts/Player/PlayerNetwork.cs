using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Gameplay;
using Map;
using Menu;
using Mirror;
using Player;
using TMPro;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    

    [Header("Prefabs")]
    [SerializeField] private GameObject _cameraPrefab;
    
    [Header("References")]
    [SerializeField] private PlayerController _player;
    
    private CinemachineFreeLook _cameraControl;
    private World _world;
    private DebugScreen _debugScreen;
    private TextMeshProUGUI _coinText;
    private GameManagerScript _gameManager;
    
    private void Start()
    {
        if (isLocalPlayer)
        {
            _cameraControl = Instantiate(_cameraPrefab).GetComponent<CinemachineFreeLook>();
            _cameraControl.m_Follow = gameObject.transform;
            _cameraControl.m_LookAt = gameObject.transform;
            _world = GameObject.FindWithTag("World").GetComponent<World>();
            _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManagerScript>();
            print(_gameManager);
            _world.player = gameObject.transform;
            _gameManager._player = gameObject.GetComponent<PlayerController>();
            //_debugScreen = GameObject.FindWithTag("PlayerCanvas").GetComponentInChildren<DebugScreen>();
            _coinText = GameObject.FindWithTag("PlayerCanvas").GetComponentInChildren<TextMeshProUGUI>();

            //_debugScreen._player = gameObject.transform;
            //_player._debugScreen = _debugScreen;
            _player._coinText = _coinText;
            
            _world.InitMap();
        }
    }

    void Update()
    {
        
    }
}
