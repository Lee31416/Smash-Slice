using System.Collections;
using System.Collections.Generic;
using Ennemy;
using Mirror;
using Player;
using UnityEngine;

public class StaffScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var target = other.gameObject;
        if (!target.CompareTag("Player")) return;
        var controller = target.GetComponent<PlayerController>();
        controller.CmdTakeDamage(10);
    }
}
