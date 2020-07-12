using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Controller script for the player's ship.
/// </summary>
public class PlayerShip : MonoBehaviour
{
    public SpaceObject MySpaceObject;

    public bool DontLowerControl; // just for testing

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
    public ControlBar ControlBar; 
    public RectTransform HealthBar; 
    
    // Cameras 
    public GameObject NormalCamera;
    public GameObject ZoomedOutCamera;
    public bool ZoomedOut;
    public float ZoomOutCost;
   
    // Particles
    public ParticleSystem ControlBurst;
    public ParticleSystem StarsUnstable;
    public ParticleSystem StarsExtreme;
    public ParticleSystem StarsCritical;
    public ParticleSystem StarsOoC;
    
    // Trajectory calculation
    public Vector3 Trajectory;
    public Vector3 PrevPosition;
    public float TrajectoryTimer;
    
    /// <summary>
    /// Standard start
    /// </summary>
    void Start()
    {
        MySpaceObject = GetComponent<SpaceObject>();

        Health = 100f;
        Control = 100f;
        ZoomedOut = false;
    }

    /// <summary>
    /// Standard update loop
    /// </summary>
    void Update()
    {
        CurrentSpeed = MySpaceObject.MyRigidBody.velocity.magnitude;
        DebugText.SetText("Speed: " + CurrentSpeed);
        
        UpdateInput();

        if (ZoomedOut)
        {
            DeductControl(ZoomOutCost * Time.deltaTime);
        }

        if (TrajectoryTimer < 0f)
        {
            Vector3 position = transform.position;
            Trajectory = transform.position - PrevPosition;
            PrevPosition = position;
            TrajectoryTimer = 0.5f;
        }
        else
        {
            TrajectoryTimer -= Time.deltaTime;
        }
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

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            NormalCamera.SetActive(false);
            ZoomedOutCamera.SetActive(true);
            ZoomedOut = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            NormalCamera.SetActive(true);
            ZoomedOutCamera.SetActive(false);
            ZoomedOut = false;
        }

        // todo - remove this eventually
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // todo - remove this eventually
        if (Input.GetKeyDown(KeyCode.T))
        {
            Control -= 5;
            if (Control < 0f)
            {
                Control = 0f;
            }
        
            // Update UI
            ControlBar.UpdateSize(Control);
            
            UpdateStarParticles();
        }
    }

    /// <summary>
    /// Lower the control without letting it go negative
    /// </summary>
    public void DeductControl(float amount)
    {
        if (DontLowerControl)
        {
            return;
        }
        
        Control -= amount;
        if (Control < 0f)
        {
            Control = 0f;
        }
        
        // Update UI
        ControlBar.UpdateSize(Control);
        
        UpdateStarParticles();
    }

    public void UpdateStarParticles()
    {
        StarsUnstable.gameObject.SetActive(Control < 76f);
        StarsExtreme.gameObject.SetActive(Control < 51f);
        StarsCritical.gameObject.SetActive(Control < 26f);
        StarsOoC.gameObject.SetActive(Control <= 1f);
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
