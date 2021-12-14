using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform _cam;

    private void Awake()
    {
        _cam = GameObject.Find("Main Camera").transform;
    }

    private void Update()
    {
        transform.LookAt(transform.position + _cam.forward);
    }
}
