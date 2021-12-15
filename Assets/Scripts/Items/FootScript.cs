using Ennemy;
using Player;
using UnityEngine;

public class FootScript : MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!_player._isAttacking) return;
        var target = other.gameObject;
        if (!target.CompareTag("Ennemy")) return;
        
        var controller = target.GetComponent<EnemyController>();
        controller.TakeKick();
    }
}
