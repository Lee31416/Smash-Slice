using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.XR;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private float _gravityForce = 14f;
    [SerializeField] private float _jumpForce = 25f;
    [SerializeField] private float _speed = 6f;
    [SerializeField] private float _turnSmoothTime = 0.1f;

    private float _turnSmoothVelocity;
    private float _verticalVelocity;
    private bool _isGrounded;
    private bool _isRunning;
    private bool _isMoving;
    private Animator _animator;
    private bool _isAttacking;

    // Start is called before the first frame update
    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        _animator.SetBool("IsRunning", _isRunning);
        _animator.SetBool("IsMoving", _isMoving);
        
        HandleAttack();
    }

    private void FixedUpdate()
    {
        HandleJumpWithGravity();
        
        HandleIdle();
        HandleMove();
    }

    private void HandleAttack()
    {
        if (!Input.GetMouseButtonDown(0) || _isAttacking) return;
        _isAttacking = true;
        //StartCoroutine(Punch1());
    }

    private IEnumerator Punch1()
    {
        _animator.SetLayerWeight(_animator.GetLayerIndex("Attack Layer"), 1);
        _animator.SetTrigger("Punch1");
        yield return new WaitForSeconds(0.9f);
        _animator.SetLayerWeight(_animator.GetLayerIndex("Attack Layer"), 0);
        _isAttacking = false;
    }

    private void HandleIdle()
    {
        if (!_isMoving)
        {
            _animator.SetFloat("Speed", 0, 0.1f, Time.fixedDeltaTime);
        }
    }

    // Problématique 
    private void HandleJumpWithGravity()
    {
        if (_isGrounded)
        {
            _verticalVelocity = _gravityForce * Time.fixedDeltaTime;
            HandleJump();
        }
        else
        {
            _verticalVelocity -= _gravityForce * Time.fixedDeltaTime;
        }

        var movementVect = new Vector3(0, _verticalVelocity, 0);
        _controller.Move(movementVect * Time.fixedDeltaTime);
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
            _animator.SetFloat("Speed", 1, 0.1f, Time.fixedDeltaTime);
            _controller.Move(direction * speed * Time.deltaTime);
        }
        else
        {
            _isRunning = false;
            _animator.SetFloat("Speed", 0.5f, 0.1f, Time.fixedDeltaTime);
            _controller.Move(direction * _speed * Time.deltaTime);
        }
    }
    
    private void HandleJump()
    {
        if (!Input.GetKeyDown("space")) return;

        _verticalVelocity = _jumpForce;
        var direction = new Vector3(0f, _jumpForce, 0f).normalized;
            
        // _controller.Move(direction * _speed * Time.deltaTime);
    }
}
