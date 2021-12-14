using Ennemy;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var temp = other.GetComponent<EnemyController>();
        temp.TakeDamage();
    }
}
