using Cinemachine;
using Menu;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject _cameraPrefab;
        
        [Header("Presets")]
        [SerializeField] private float _pickupRange;
        [SerializeField] private LayerMask _itemLayer;
        [SerializeField] private GameObject _rightHand;
        [SerializeField] private Vector3 _equippedWeaponPosition;
        [SerializeField] private Vector3 _equippedWeaponRotation;
        [SerializeField] private DebugScreen _debugScreen;
        [SerializeField] private HealthBar _healthBar;
        [SerializeField] private int maxHealth = 100;
    
        private GameObject _equippedWeapon;
        private bool _hasWeapon { get; set; }
        private CinemachineFreeLook _cameraControl;
        private PlayerMovement _movement;

        private int currentHealth;
    
        private void Awake()
        {
            _movement = GetComponent<PlayerMovement>();
            _cameraControl = Instantiate(_cameraPrefab).GetComponent<CinemachineFreeLook>();
            _cameraControl.m_Follow = gameObject.transform;
            _cameraControl.m_LookAt = gameObject.transform;
            currentHealth = maxHealth;
            _healthBar.SetMaxHealth(maxHealth);
        }
    
        private void Update()
        {
            _hasWeapon = _equippedWeapon != null;
            _movement._animator.SetBool("HasWeapon", _hasWeapon);
            _healthBar.SetHealth(currentHealth);
            
            if (Input.GetKeyDown(KeyCode.F) && !_hasWeapon)
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
        }
    
        private void DropItem()
        {
            _equippedWeapon.transform.parent = null;
            _equippedWeapon.GetComponent<Rigidbody>().isKinematic = false;
            _equippedWeapon.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * 10);
    
            _equippedWeapon = null;
        }
    
        private void PickItem()
        {
            var pickableItems = Physics.OverlapBox(transform.position, new Vector3(_pickupRange, _pickupRange, _pickupRange), Quaternion.identity, _itemLayer);
            foreach (var item in pickableItems)
            {
                var pickedItem = item.gameObject;
                var weaponScript = pickedItem.GetComponent<WeaponScript>();
                if (weaponScript != null)
                {
                    _equippedWeapon = pickedItem;
                    print("just picked up : " + _equippedWeapon.name);
                }
            }
    
            _equippedWeapon.GetComponent<Rigidbody>().isKinematic = true;
            _equippedWeapon.transform.parent = _rightHand.transform;
            _equippedWeapon.transform.localPosition = _equippedWeaponPosition;
            _equippedWeapon.transform.localEulerAngles = _equippedWeaponRotation;
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(_pickupRange, 2, _pickupRange));
        }
    }
}


