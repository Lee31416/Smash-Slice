using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ennemy
{
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField] private CharacterController _controller;
        [SerializeField] private float _gravityForce = 14f;
        [SerializeField] private float _speed = 6f;
        
        private bool _isGrounded;
        public Animator _animator { get; private set; }
        private float _verticalVelocity;
        private bool _isRunning;
        private bool _isMoving;
        
        public void Move(float distance)
        {
            var direction = transform.forward * distance;
            
            _isRunning = false;
            _animator.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
            _controller.Move(direction * _speed * Time.deltaTime);
        }
        
        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
        }
    
        private void Update()
        {
            
            _isMoving = _controller.velocity.x != 0 || _controller.velocity.y != 0 || _controller.velocity.z != 0;
            _isGrounded = _controller.isGrounded;
            _animator.SetBool("IsRunning", _isRunning);
            _animator.SetBool("IsMoving", _isMoving);
            
            HandleIdle();
        }
        
        private void HandleIdle()
        {
            if (!_isMoving)
            {
                _animator.SetFloat("Speed", 0, 0.1f, Time.fixedDeltaTime);
            }
        }
        
        private void FixedUpdate()
        {
            HandleGravity();
        }
    
        private void HandleGravity()
        {
            if (!_isGrounded)
            {
                _verticalVelocity -= _gravityForce * Time.fixedDeltaTime;
            }
    
            var movementVect = new Vector3(0, _verticalVelocity, 0);
            _controller.Move(movementVect * Time.fixedDeltaTime);
        }
    
    }
}


