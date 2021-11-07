using System;
using System.Collections.Generic;
using UnityEngine;

namespace Camera
{
    public class CameraControl : MonoBehaviour
    {
        private GameObject _player;
        public GameObject target
        {
            get => _player;
            set => _player = value;
        }

        private void FixedUpdate () 
        {
            if (_player == null) return;
            var position = _player.transform.position;
            transform.position = new Vector3 (position.x, position.y, 0);
        }
    }
}