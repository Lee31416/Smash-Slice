using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var temp = other.GetComponent<EnemyController>();
        temp.TakeDamage();
    }
}
