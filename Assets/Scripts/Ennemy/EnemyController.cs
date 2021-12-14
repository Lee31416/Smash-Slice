using System.Collections;
using UnityEngine;

namespace Ennemy
{
    public class EnemyController : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject _coinPrefab;
        
        [Header("References")]
        [SerializeField] private HealthBar _healthBar;
        [SerializeField] private CharacterController _controller;
        
        [Header("Presets")]
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private float _detectionRange = 15f;
        [SerializeField] private int _lootChance = 100;
        [SerializeField] private int _cashDropChance = 50;

        [Header("Debug")]
        [SerializeField] private int currentHealth;
        
        private Transform _target;
        private EnemyMovement _movement;
        private bool _isAlive = true;
        private bool _droppedLoot = false;
        private float _knockBackForce;
    
        private void Awake()
        {
            currentHealth = maxHealth;
            _healthBar.SetMaxHealth(maxHealth);
            _movement = GetComponent<EnemyMovement>();
        }
    
        private void Update()
        {
            _healthBar.SetHealth(currentHealth);
            HandleDeath();
            
            if (!_isAlive) return;
            
            HandleKnockBack();
            HandleTargetTracking();
        }

        private void HandleTargetTracking()
        {
            FindTarget();
            
            if (_target == null) return;
            var distance = Vector3.Distance(_target.position, transform.position);
    
            if (distance >= _detectionRange) return; // TODO check ca
            FaceTarget();
    
            if (distance < 3) return;
            MoveToTarget(distance);
        }

        private void HandleKnockBack()
        {
            if (_knockBackForce == 0) return;
            _controller.Move(new Vector3(0, 0, _knockBackForce));
            _knockBackForce -= .05f;
            if (_knockBackForce < 0) _knockBackForce = 0;
        }

        private void HandleDeath()
        {
            _isAlive = currentHealth > 0;
            _movement._animator.SetBool("IsAlive", _isAlive);

            if (!_isAlive && !_droppedLoot)
            {
                GenerateLoot();
            }
        }

        private void GenerateLoot()
        {
            var rdmNumber = Random.Range(0, 100);

            StartCoroutine(GenerateCash());

            _droppedLoot = true;
        }

        private IEnumerator GenerateCash()
        {
            yield return new WaitForSeconds(2);
            
            for (var i = 0; i < _cashDropChance / 2; i++)
            {
                var rdmNumber = Random.Range(0, 100);

                if (rdmNumber % 2 != 0) continue;
                var vect = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
                var coinInstance = Instantiate(_coinPrefab, vect, Quaternion.identity);
                var rdmForce = Random.Range(500, 750);
                coinInstance.GetComponent<Rigidbody>().AddForce(transform.up * rdmForce);
            }
        }

        private void MoveToTarget(float distance)
        {
            _movement.Move(distance);
        }
    
        private void FindTarget()
        {
            var targets = Physics.OverlapSphere(transform.position, _detectionRange);
            foreach (var target in targets)
            {
                if (target.gameObject.CompareTag("Player"))
                {
                    _target = target.transform;
                }
            }
        }
        
        private void FaceTarget ()
        {
            var direction = (_target.position - transform.position).normalized;
            if (direction.x == 0 || direction.z == 0) return;
            var lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _detectionRange);
        }
    
        public void TakeDamage(int dmg)
        {
            currentHealth -= dmg;
            if (currentHealth <= 0) currentHealth = 0;
        }

        public void TakeKick()
        {
            _knockBackForce = 1f;
        }
        
        
    }
}


