﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A general use helper component for any objects in space that will fly around and stuff.
/// </summary>
public class SpaceObject : MonoBehaviour
{
    public Rigidbody MyRigidBody;
    
    /// <summary>
    /// Standard start
    /// </summary>
    void Start()
    {
        MyRigidBody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Standard update loop
    /// </summary>
    void Update()
    {
        
    }

    /// <summary>
    /// Rotates around the Z axis
    /// </summary>
    public void Rotate(float _value)
    {
        transform.Rotate(Vector3.forward, _value);
    }

    /// <summary>
    /// Applies the given force along the X axis
    /// </summary>
    public void MoveForward(float _value)
    {
        MyRigidBody.AddRelativeForce(_value, 0f, 0f, ForceMode.Acceleration);
    }
}
