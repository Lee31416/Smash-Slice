using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Gameplay
{
    public class GameManagerScript : MonoBehaviour
    {
        [SerializeField] private GameObject ennemyPrefab;
        [SerializeField] private PlayerController _player;
        [SerializeField] private float _enemySpawnChance = 25f;
        [SerializeField] private int _enemySpawnInterval = 5;
        [SerializeField] private int _enemySpawnRange = 10;
        
        private List<GameObject> enemies = new List<GameObject>();
        private List<GameObject> enemiesToRemove = new List<GameObject>();
        private bool _isSpawningEnnemies;
    
        private void Start()
        {
            
        }
    
        private void FixedUpdate()
        {
            StartCoroutine(SpawnEnemy());
    
            foreach (var enemy in enemies)
            {
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
                Destroy(enemy);
            }
            
            enemiesToRemove.Clear();
        }
        
        private IEnumerator SpawnEnemy()
        {
            if (_isSpawningEnnemies) yield break;
    
            _isSpawningEnnemies = true;
            
            yield return new WaitForSeconds(_enemySpawnInterval);
            
            _isSpawningEnnemies = false;
    
            var rdmNumber = Random.Range(0, 100);
            print(rdmNumber);
            if (rdmNumber > _enemySpawnChance) yield break;
    
            var playerX = _player.transform.position.x;
            var playerZ = _player.transform.position.z;
            var randomX = Random.Range(0, _enemySpawnRange);
            var randomZ = Random.Range(0, _enemySpawnRange);
    
            playerX += _enemySpawnRange / 2;
            playerZ += _enemySpawnRange / 2;
            
            var spawnPos = new Vector3(playerX - randomX, 80, playerZ - randomZ);
            print(spawnPos);
    
            var enemy = Instantiate(ennemyPrefab, spawnPos, Quaternion.identity);
            enemies.Add(enemy);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(_player.transform.position, new Vector3(_enemySpawnRange, 2, _enemySpawnRange));
        }
    }
}


