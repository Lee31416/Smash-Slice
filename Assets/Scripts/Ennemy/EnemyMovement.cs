using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private float _gravityForce = 14f;
    [SerializeField] private float _speed = 6f;
    
    private bool _isGrounded;
    private Animator _animator;
    private float _verticalVelocity;
    
    public void Move(float distance)
    {
        var direction = transform.forward * distance;
        _controller.Move(direction * _speed * Time.deltaTime);
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        _isGrounded = _controller.isGrounded;
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
