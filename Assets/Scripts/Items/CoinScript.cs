using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    private void Update()
    {
        if (gameObject.transform.position.y < -100) Destroy(gameObject);
    }
}
