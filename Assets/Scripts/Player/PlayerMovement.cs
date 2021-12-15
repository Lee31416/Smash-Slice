using System.Collections;
using Mirror;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : NetworkBehaviour
    {
        [SerializeField] private CharacterController _controller;
        [SerializeField] private PlayerController _playerCtrl;
        [SerializeField] private float _gravityForce = 14f;
        [SerializeField] private float _jumpForce = 25f;
        [SerializeField] private float _speed = 6f;
        [SerializeField] private float _turnSmoothTime = 0.1f;
    
        private float _turnSmoothVelocity;
        private float _verticalVelocity;
        private bool _isGrounded;
        private bool _isRunning;
        private bool _isMoving;
        private bool _isAlive;
        public Animator _animator { get; private set; }
        public bool _isAttacking { get; private set; }
    
        private void Start()
        {
            _animator = GetComponentInChildren<Animator>();
        }
    
        private void Update()
        {
            if (!isLocalPlayer) return;
            _isAlive = _playerCtrl._isAlive;
            if (!_isAlive) return;
            
            _isMoving = _controller.velocity.x != 0 || _controller.velocity.y != 0 || _controller.velocity.z != 0;
            _isGrounded = _controller.isGrounded;
            _animator.SetBool("IsRunning", _isRunning);
            _animator.SetBool("IsMoving", _isMoving);
            _animator.SetBool("IsGrounded", _isGrounded);
            _animator.SetFloat("AirVelocity", _verticalVelocity);

            if (_isGrounded) _verticalVelocity = 0;
            HandleAttack();
            HandleKick();
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer) return;
            if (!_isAlive) return;
            
            HandleJumpWithGravity();
            HandleMove();
            HandleIdle();
        }
    
        private void HandleAttack()
        {
            if (!Input.GetMouseButtonDown(0) || _isAttacking) return;
            _isAttacking = true;
            _playerCtrl._rightHand.GetComponent<SphereCollider>().enabled = true;
            StartCoroutine(Attack());
           
        }
        
        private void HandleKick()
        {
            if (!Input.GetKeyDown(KeyCode.F) || _isAttacking) return;
            _isAttacking = true;
            _playerCtrl._rightFoot.GetComponent<CapsuleCollider>().enabled = true;
            StartCoroutine(Kick());
        }
    
        private IEnumerator Attack()
        {
            if (_playerCtrl._hasWeapon)
            {
                _animator.SetTrigger("Attack");
                yield return new WaitForSeconds(0.9f);
                _isAttacking = false;
            }
            else
            {
                _animator.SetTrigger("Punch");
                yield return new WaitForSeconds(0.9f);
                _isAttacking = false;
            }
            
            _playerCtrl._rightHand.GetComponent<SphereCollider>().enabled = false;
        }
        
        private IEnumerator Kick()
        {
            if (_playerCtrl._hasWeapon)
            {
                _animator.SetTrigger("KickWithWeapon");
                yield return new WaitForSeconds(0.9f);
                _isAttacking = false;
            }
            else
            {
                _animator.SetTrigger("Kick");
                yield return new WaitForSeconds(0.9f);
                _isAttacking = false;
            }
            
            _playerCtrl._rightFoot.GetComponent<CapsuleCollider>().enabled = false;
        }
    
        private void HandleIdle()
        {
            if (!_isMoving)
            {
                _animator.SetFloat("Speed", 0, 0.1f, Time.fixedDeltaTime);
            }
        }
    
        private void HandleJumpWithGravity()
        {
            if (_isGrounded)
            {
                HandleJump();
            }
            else
            {
                _verticalVelocity -= _gravityForce * Time.deltaTime;
            }
    
            var movementVect = new Vector3(0, _verticalVelocity, 0);
            _controller.Move(movementVect * Time.deltaTime);
        }
    
        private void HandleMove()
        {
            var h = Input.GetAxisRaw("Horizontal");
            var v = Input.GetAxisRaw("Vertical");
            var direction = transform.right * h + transform.forward * v;
    
            if (!(direction.magnitude >= 0.1f)) return;
            
            var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
                
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _isRunning = true;
                var speed = _speed * 2.5f;
                _animator.SetFloat("Speed", 1f, 0.1f, Time.deltaTime);
                _controller.Move(direction * speed * Time.deltaTime);
            }
            else
            {
                _isRunning = false;
                _animator.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
                _controller.Move(direction * _speed * Time.deltaTime);
            }
            
        }
        
        private void HandleJump()
        {
            if (!Input.GetKeyDown("space")) return;
    
            _verticalVelocity = _jumpForce;
            var direction = new Vector3(0f, _jumpForce, 0f).normalized;
            _animator.SetTrigger("Jump");
                
            _controller.Move(direction * _speed * Time.deltaTime);
        }
    }
}


