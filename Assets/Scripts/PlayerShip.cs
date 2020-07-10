using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller script for the player's ship.
/// </summary>
public class PlayerShip : MonoBehaviour
{
    public SpaceObject MySpaceObject;

    // Moving forward
    public float StandardAcceleration;
    public float UpgradedAcceleration;
    public bool HasSpeedUpgrade;
    public float CurrentSpeed;
    
    // Rotation
    public float StandardRotationSpeed;
    public float UpgradedRotationSpeed;
    public bool HasRotationUpgrade;
    
    /// <summary>
    /// Standard start
    /// </summary>
    void Start()
    {
        MySpaceObject = GetComponent<SpaceObject>();
    }

    /// <summary>
    /// Standard update loop
    /// </summary>
    void Update()
    {
        CurrentSpeed = MySpaceObject.MyRigidBody.velocity.magnitude;
        DebugText.SetText("Speed: " + CurrentSpeed);
        
        UpdateInput();
    }

    /// <summary>
    /// Update loop to handle all input
    /// </summary>
    private void UpdateInput()
    {
        if (HoldingUp())
        {
            float speed = HasSpeedUpgrade ? UpgradedAcceleration : StandardAcceleration;
            MySpaceObject.MoveForward(speed * Time.deltaTime);
        }

        if (HoldingDown())
        {
            float speed = HasSpeedUpgrade ? UpgradedAcceleration : StandardAcceleration;
            MySpaceObject.MoveForward(-speed * Time.deltaTime);
        }

        if (HoldingLeft())
        {
            float speed = HasRotationUpgrade ? UpgradedRotationSpeed : StandardRotationSpeed;
            MySpaceObject.Rotate(speed * Time.deltaTime);
        }

        if (HoldingRight())
        {
            float speed = HasRotationUpgrade ? UpgradedRotationSpeed : StandardRotationSpeed;
            MySpaceObject.Rotate(-speed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Whether or not the player is holding the accelerate forward key
    /// </summary>
    private bool HoldingUp()
    {
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
    }

    /// <summary>
    /// Whether or not the player is holding the accelerate back key
    /// </summary>
    private bool HoldingDown()
    {
        return Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
    }

    /// <summary>
    /// Whether or not the player is holding the rotate left key
    /// </summary>
    private bool HoldingLeft()
    {
        return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
    }

    /// <summary>
    /// Whether or not the player is holding the rotate right key
    /// </summary>
    private bool HoldingRight()
    {
        return Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
    }
    
}
