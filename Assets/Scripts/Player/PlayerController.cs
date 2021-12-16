using Cinemachine;
using Menu;
using Mirror;
using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerController : NetworkBehaviour
    {
        [Header("GameObjects")]
        [SerializeField] public GameObject _rightHand;
        [SerializeField] public GameObject _rightFoot;
        [SerializeField] private HealthBar _healthBar;
        [SerializeField] public DebugScreen _debugScreen;
        [SerializeField] public TextMeshProUGUI _coinText;
        public GameObject _deathScreen;

        [Header("Presets")]
        [SerializeField] private float _pickupRange;
        [SerializeField] private LayerMask _itemLayer;
        [SerializeField] private Vector3 _equippedWeaponPosition;
        [SerializeField] private Vector3 _equippedWeaponRotation;
        [SerializeField] private int maxHealth = 100;
    
        private GameObject _equippedWeapon;
        public bool _hasWeapon { get; private set; }
        private PlayerMovement _movement;
        public int coinAmount { get; private set; }
        
        [SyncVar]
        private int currentHealth;
        
        [field : SyncVar]
        public bool _isAlive { get; private set; }
        public bool _isAttacking { get; private set; }
        
        private void Start()
        {
            if (!isLocalPlayer) return;
            
            _movement = GetComponent<PlayerMovement>();
            currentHealth = maxHealth;
            _healthBar.SetMaxHealth(maxHealth);
            _deathScreen = GameObject.Find("DeathScreen");
            _debugScreen = GameObject.FindWithTag("PlayerCanvas").GetComponentInChildren<DebugScreen>();
            _debugScreen._player = transform;
            _debugScreen.gameObject.SetActive(!_debugScreen.gameObject.activeSelf);
        }
    
        private void Update()
        {
            if (!isLocalPlayer) return;
            
            _isAlive = currentHealth > 0;
            _movement._animator.SetBool("IsAlive", _isAlive);
            if (!_isAlive)
            {
                _movement._animator.SetTrigger("Die");
                if (_deathScreen != null) _deathScreen.gameObject.SetActive(true);
            }
           
            if (!_isAlive) return;

            _isAttacking = _movement._isAttacking;
            _hasWeapon = _equippedWeapon != null;
            _movement._animator.SetBool("HasWeapon", _hasWeapon);
            _healthBar.SetHealth(currentHealth);
            if (_deathScreen != null) _deathScreen.gameObject.SetActive(false);
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                PickItem();
            }
            
            if (Input.GetKeyDown(KeyCode.Q) && _hasWeapon)
            {
                DropItem();
            }
            
            if (Input.GetKeyDown(KeyCode.F3))
            {
                _debugScreen.gameObject.SetActive(!_debugScreen.gameObject.activeSelf);
            }
            
            /*if (Input.GetKeyDown(KeyCode.X))
            {
                RevivePlayer();
            }*/

            _coinText.text = coinAmount.ToString();
            
            CheckForDownedPlayers();
        }

        private void CheckForDownedPlayers()
        {
            /*var revivablePlayer = Physics.OverlapBox(transform.position, new Vector3(_pickupRange, 2, _pickupRange), Quaternion.identity);
            
            if (revivablePlayer.Length > 0)
            {
                print("Can revive someone");
            }*/
            
            // TODO WIP
        }

        private void RevivePlayer()
        {
            var revivablePlayer = Physics.OverlapBox(transform.position, new Vector3(_pickupRange, 2, _pickupRange), Quaternion.identity);

            foreach (var player in revivablePlayer)
            {
                if (player.tag != "Player") return;

                var controller = player.GetComponent<PlayerController>();
                if (controller._isAlive) return;
                if (coinAmount < 25) return;

                controller.currentHealth = 50;
                coinAmount -= 25;
            }
        }
    
        private void DropItem()
        {
            _equippedWeapon.transform.parent = null;
            _equippedWeapon.GetComponent<Rigidbody>().isKinematic = false;
            _equippedWeapon.GetComponent<Rigidbody>().AddForce(Vector3.forward* 500); //Moving projectile
               
            _equippedWeapon = null;
        }
    
        private void PickItem()
        {
            var pickableItems = Physics.OverlapBox(transform.position, new Vector3(_pickupRange, 2, _pickupRange), Quaternion.identity, _itemLayer);
            foreach (var item in pickableItems)
            {
                var pickedItem = item.gameObject;

                switch (pickedItem.tag)
                {
                    case "Coin":
                    {
                        coinAmount++;
                        NetworkServer.Destroy(pickedItem.gameObject);
                        break; 
                    }
                    case "Heart":
                    {
                        currentHealth += 15;
                        if (currentHealth > maxHealth) currentHealth = maxHealth;
                        NetworkServer.Destroy(pickedItem.gameObject);
                        break;
                    }
                    case "Weapon" when !_hasWeapon:
                    {
                        _equippedWeapon = pickedItem;
                        _equippedWeapon.GetComponent<BoxCollider>().enabled = false;
                        print("just picked up : " + _equippedWeapon.name);
                        break;
                    }
                }
            }

            if (_equippedWeapon != null)
            {
                _equippedWeapon.GetComponent<BoxCollider>().enabled = true;
                _equippedWeapon.GetComponent<Rigidbody>().isKinematic = true;
                _equippedWeapon.transform.parent = _rightHand.transform;
                _equippedWeapon.transform.localPosition = _equippedWeaponPosition;
                _equippedWeapon.transform.localEulerAngles = _equippedWeaponRotation;
            }
        }

        [Command]
        public void CmdTakeDamage(int dmg)
        {
            if (!isServer) return;
            TakeDamage(dmg);
        }

        [ClientRpc]
        private void TakeDamage(int dmg)
        {
            currentHealth -= dmg;
            if (currentHealth <= 0) currentHealth = 0;
            _movement._animator.SetTrigger("Hurt");
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            var vec = new Vector3(transform.position.x, 0.5f, transform.position.z);
            Gizmos.DrawWireCube(vec, new Vector3(_pickupRange, 1, _pickupRange));
        }
    }
}


