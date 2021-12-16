using System;
using Ennemy;
using Mirror;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    private void Update()
    {
        if (gameObject.transform.position.y < -100)
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var target = other.gameObject;

        if (!target.CompareTag("Ennemy")) return;
        var controller = target.GetComponent<EnemyController>();
        controller.TakeDamage(25);
    }
}
