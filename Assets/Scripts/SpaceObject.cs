using System.Collections;
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

    /// <summary>
    /// Gently move towards the given point with the given power.
    /// </summary>
    public void ApplyGravity(Vector3 center, float power)
    {
        Vector3 toCenter = center - transform.position;
        toCenter.Normalize();
        MyRigidBody.AddForce(toCenter * power);
    }

    /// <summary>
    /// Attempts to slow down the object to a speed of 0.
    /// The faster its going, the more force will be used.
    /// </summary>
    public void ApplyFriction()
    {
        float x = MyRigidBody.velocity.x * 0.5f;
        float y = MyRigidBody.velocity.y * 0.5f;
        MyRigidBody.AddForce(-x, -y, 0f);
    }
    
}
