﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public float AccelerationCost;
    
    // Rotation
    public float StandardRotationSpeed;
    public float UpgradedRotationSpeed;
    public bool HasRotationUpgrade;
    public float RotationCost;
    
    // Weapon
    public bool HasWeaponUpgrade;
    public GameObject BulletPrefab;
    public float BulletCost;
    
    // Stats
    public float Health;
    public float Control;
    public int Gold;
    
    // UI
    public RectTransform ControlBar; 
    public RectTransform HealthBar; 
    
    /// <summary>
    /// Standard start
    /// </summary>
    void Start()
    {
        MySpaceObject = GetComponent<SpaceObject>();

        Health = 100f;
        Control = 100f;
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
    /// Update loop to handle all input.
    /// Friction is only applied while accelerating for a smoother experience.
    /// It's less realistic, but it makes it much easier to control.
    /// </summary>
    private void UpdateInput()
    {
        if (HoldingUp() && Control > 0.01f)
        {
            float speed = HasSpeedUpgrade ? UpgradedAcceleration : StandardAcceleration;
            MySpaceObject.MoveForward(speed * Time.deltaTime);
            MySpaceObject.ApplyFriction();
            DeductControl(AccelerationCost * Time.deltaTime);
        }
        
        if (HoldingDown() && CurrentSpeed > 1f && Control > 0.01f)
        {
            float speed = HasSpeedUpgrade ? UpgradedAcceleration : StandardAcceleration;
            MySpaceObject.MoveForward(-speed * 0.25f * Time.deltaTime);
            MySpaceObject.ApplyFriction();
            DeductControl(AccelerationCost * Time.deltaTime);
        }

        if (HoldingLeft() && Control > 0.01f)
        {
            float speed = HasRotationUpgrade ? UpgradedRotationSpeed : StandardRotationSpeed;
            MySpaceObject.Rotate(speed * Time.deltaTime);
            DeductControl(RotationCost * Time.deltaTime);
        }

        if (HoldingRight() && Control > 0.01f)
        {
            float speed = HasRotationUpgrade ? UpgradedRotationSpeed : StandardRotationSpeed;
            MySpaceObject.Rotate(-speed * Time.deltaTime);
            DeductControl(RotationCost * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space) && Control > 0.01f)
        {
            GameObject bullet = Instantiate(BulletPrefab, transform.position, transform.rotation);
            // bullet.transform.rotation = transform.rotation;
            // bullet.transform.position = transform.position;
            bullet.GetComponent<Bullet>().Fire(HasWeaponUpgrade, CurrentSpeed);
            DeductControl(BulletCost);
        }
    }

    /// <summary>
    /// Lower the control without letting it go negative
    /// </summary>
    public void DeductControl(float amount)
    {
        Control -= amount;
        if (Control < 0f)
        {
            Control = 0f;
        }
        
        // Update UI
        Vector2 size = ControlBar.sizeDelta;
        size.x = 500f * (Control / 100f);
        ControlBar.sizeDelta = size;
    }

    /// <summary>
    /// Lower the health without letting it go negative
    /// </summary>
    public void DeductHealth(float amount)
    {
        Health -= amount;
        if (amount < 0f)
        {
            Debug.Log("ded"); // todo reset game
        }
        
        // Update UI
        Vector2 size = HealthBar.sizeDelta;
        size.x = 500f * (Health / 100f);
        HealthBar.sizeDelta = size;
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
