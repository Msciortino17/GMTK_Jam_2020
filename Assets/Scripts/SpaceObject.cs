using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A general use helper component for any objects in space that will fly around and stuff.
/// </summary>
public class SpaceObject : MonoBehaviour
{
    public Rigidbody MyRigidBody;
    public GameManager MyGameManager;
    public float WrapperTimer;
    
    /// <summary>
    /// Standard awake
    /// </summary>
    void Awake()
    {
        MyGameManager = GameManager.GetReference();
        MyRigidBody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Standard update loop
    /// </summary>
    void Update()
    {
        UpdateBounds();
    }

    /// <summary>
    /// Keeps the space object within the bounds of the level by wrapping position
    /// </summary>
    private void UpdateBounds()
    {
        if (WrapperTimer <= 0f)
        {
            float distanceSqr = transform.position.sqrMagnitude;
            float bounds = MyGameManager.LevelRadius;
            bounds *= bounds;
            if (distanceSqr > bounds)
            {
                Vector3 position = transform.position;
                position.x = -position.x;
                position.y = -position.y;
                transform.position = position;
                WrapperTimer = 0.5f;
                // todo - really want a noticeable effect for this so it's not too jarring.
            }
        }
        else
        {
            WrapperTimer -= Time.deltaTime;
        }
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
    /// Doesn't go too crazy to allow some resistance.
    /// </summary>
    public void ApplyFriction()
    {
        float x = MyRigidBody.velocity.x * 0.25f;
        float y = MyRigidBody.velocity.y * 0.25f;
        MyRigidBody.AddForce(-x, -y, 0f);
    }

    /// <summary>
    /// Attempts to slow down the object to a speed of 0.
    /// The faster its going, the more force will be used.
    /// </summary>
    public void ApplyHeavyFriction()
    {
        float x = MyRigidBody.velocity.x * 0.75f;
        float y = MyRigidBody.velocity.y * 0.75f;
        MyRigidBody.AddForce(-x, -y, 0f);
    }
    
}
