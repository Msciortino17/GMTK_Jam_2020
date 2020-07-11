using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public Transform Sun;
    public float rotateSpeed;

    private void Update()
    {
        transform.RotateAround(Sun.position, Vector3.forward, rotateSpeed * Time.deltaTime);
    }
}
