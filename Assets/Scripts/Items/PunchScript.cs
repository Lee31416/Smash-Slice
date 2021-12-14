using System.Collections;
using System.Collections.Generic;
using Ennemy;
using Player;
using UnityEngine;

public class PunchScript : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!_player._isAttacking) return;
        print("punched : " + other.name);
        
        var target = other.gameObject;

        if (!target.CompareTag("Ennemy")) return;
        var controller = target.GetComponent<EnemyController>();
        controller.TakeDamage(10);
    }
}
