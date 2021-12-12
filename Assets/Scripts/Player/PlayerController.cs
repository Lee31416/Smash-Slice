using UnityEngine;
using UnityEngine.XR;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject _debugScreen;
    [SerializeField] private float _pickupRange;
    [SerializeField] private LayerMask _itemLayer;
    [SerializeField] private GameObject _rightHand;
    [SerializeField] private Vector3 _equippedWeaponPosition;
    [SerializeField] private Vector3 _equippedWeaponRotation;

    private GameObject _equippedWeapon;
    private bool _hasWeapon { get; set; }
    
    private PlayerMovement _movement;

    private void Start()
    {
        _movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        _hasWeapon = _equippedWeapon != null;
        _movement._animator.SetBool("HasWeapon", _hasWeapon);
        
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
            _debugScreen.SetActive(!_debugScreen.activeSelf);
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
