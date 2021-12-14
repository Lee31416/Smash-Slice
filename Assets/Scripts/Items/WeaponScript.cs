using Ennemy;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        print("hit : " + other.name);
        
        var target = other.gameObject;

        if (!target.CompareTag("Ennemy")) return;
        var controller = target.GetComponent<EnemyController>();
        controller.TakeDamage(25);
    }
}
