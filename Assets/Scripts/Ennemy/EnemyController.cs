using System.Collections;
using Mirror;
using UnityEngine;

namespace Ennemy
{
    public class EnemyController : NetworkBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject _coinPrefab;
        [SerializeField] private GameObject _greatSwordPrefab;
        [SerializeField] private GameObject _heartPrefab;
        
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

            if (!_isAlive)
            {
                StartCoroutine(WaitForSecondsBeforeRemovingCadaver(3));
            }
            if (!_isAlive) return;
            
            HandleKnockBack();
            HandleTargetTracking();
        }

        private void HandleTargetTracking()
        {
            FindTarget();
            
            
            if (_target == null) return;
            var distance = Vector3.Distance(_target.position, transform.position);
    
            if (distance >= _detectionRange) return;
            FaceTarget();
            if (distance < 3)
            {
                AttackTarget();
            }
            
            if (distance < 3) return;
            MoveToTarget(distance);
        }

        private void AttackTarget()
        {
           _movement._animator.SetTrigger("Attack");
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
            if (!isServer) return;
            
            StartCoroutine(WaitForSecondsBeforeGenloot(2));

            _droppedLoot = true;
        }

        private IEnumerator WaitForSecondsBeforeRemovingCadaver(int seconds)
        {
            yield return new WaitForSeconds(seconds); 
            
            NetworkServer.Destroy(gameObject);
        }
        
        private IEnumerator WaitForSecondsBeforeGenloot(int seconds)
        {
            yield return new WaitForSeconds(seconds); 
            CmdGenerateMoneyAndHeart();
            CmdGenerateWeapon();
        }

        [Command(requiresAuthority = false)]
        private void CmdGenerateMoneyAndHeart()
        {
            for (var i = 0; i < _cashDropChance / 2; i++)
            {
                var rdmNumber = Random.Range(0, 100);

                if (rdmNumber % 2 != 0) continue;
                var vect = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);
                var coinInstance = Instantiate(_coinPrefab, vect, Quaternion.identity);
                NetworkServer.Spawn(coinInstance);
                
            }
            
            var vectHeart = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);
            var heartInstance =  Instantiate(_heartPrefab, vectHeart, Quaternion.identity);
            NetworkServer.Spawn(heartInstance);
        }

        [Command(requiresAuthority = false)]
        private void CmdGenerateWeapon()
        {
            var rdm = Random.Range(0, 100);

            if ((rdm + _lootChance) > 75)
            {
                var vect = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);
                var weaponInstance = Instantiate(_greatSwordPrefab, vect, Quaternion.identity);
                NetworkServer.Spawn(weaponInstance);
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
            if (!_isAlive) return;
            
            currentHealth -= dmg;
            if (currentHealth <= 0) currentHealth = 0;
            _movement._animator.SetTrigger("Hurt");
        }

        public void TakeKick()
        {
            _knockBackForce = 1f;
        }
        
        
    }
}


