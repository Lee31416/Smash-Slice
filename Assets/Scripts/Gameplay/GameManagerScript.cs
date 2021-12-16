using System.Collections;
using System.Collections.Generic;
using Mirror;
using Player;
using UnityEngine;

namespace Gameplay
{
    public class GameManagerScript : NetworkBehaviour
    {
        [SerializeField] private GameObject ennemyPrefab;
        [SerializeField] private float _enemySpawnChance = 25f;
        [SerializeField] private int _enemySpawnInterval = 5;
        [SerializeField] private int _enemySpawnRange = 10;
        
        private List<GameObject> enemies = new List<GameObject>();
        private List<GameObject> enemiesToRemove = new List<GameObject>();
        private bool _isSpawningEnnemies;
        public PlayerController _player;

    
        private void Start()
        {
            
        }
    
        private void FixedUpdate()
        {
            if (_player == null) return;
            if (!_player._isAlive) return;
            
            StartCoroutine(WaitForIntervalBeforeSpawningEnemies());

            CleanEnemiesThatAreTooFar();
        }

        private void CleanEnemiesThatAreTooFar()
        {
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;
                
                var posXDiff = enemy.transform.position.x - _player.transform.position.x;
                if (posXDiff < 0)
                {
                    posXDiff *= -1;
                }
                var posZDiff = enemy.transform.position.z - _player.transform.position.z;
                if (posZDiff < 0)
                {
                    posZDiff *= -1;
                }
                
                if (posXDiff > _enemySpawnRange || posZDiff > _enemySpawnRange)
                {
                    enemiesToRemove.Add(enemy);
                }
            }
    
            foreach (var enemy in enemiesToRemove)
            {
                enemies.Remove(enemy);
                NetworkServer.Destroy(enemy);
            }
            
            enemiesToRemove.Clear();
        }
        
        private IEnumerator WaitForIntervalBeforeSpawningEnemies()
        {
            if (_isSpawningEnnemies) yield break;
    
            _isSpawningEnnemies = true;
            
            yield return new WaitForSeconds(_enemySpawnInterval);
            
            _isSpawningEnnemies = false;

            CmdSpawnEnnemy();
        }
        
        [Command(requiresAuthority = false)]
        private void CmdSpawnEnnemy()
        {
            var rdmNumber = Random.Range(0, 100);
            if (rdmNumber > _enemySpawnChance) return;
    
            var playerX = _player.transform.position.x;
            var playerZ = _player.transform.position.z;
            var randomX = Random.Range(0, _enemySpawnRange);
            var randomZ = Random.Range(0, _enemySpawnRange);
    
            playerX += _enemySpawnRange / 2;
            playerZ += _enemySpawnRange / 2;
            
            var spawnPos = new Vector3(playerX - randomX, 80, playerZ - randomZ);
    
            var enemy = Instantiate(ennemyPrefab, spawnPos, Quaternion.identity);
            NetworkServer.Spawn(enemy);
            enemies.Add(enemy);
        }
        
        private void OnDrawGizmosSelected()
        {
            if (_player == null) return;
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(_player.transform.position, new Vector3(_enemySpawnRange, 2, _enemySpawnRange));
        }
    }
}


