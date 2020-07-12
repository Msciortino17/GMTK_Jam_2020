using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

/// <summary>
/// Controller script for the player's ship.
/// </summary>
public class PlayerShip : MonoBehaviour
{
    public SpaceObject MySpaceObject;
    private GameManager Manager;

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
    public float HealthTimer;
    public float Control;
    public int Score;
    
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
    
    // Micro jumps
    public float MicroJumpTimer;
    public float MicroJumpTime;
    
    // Game over
    public bool Dead;
    public GameObject GameOver;
    public GameObject MySprite;
    public CapsuleCollider MyCollider;
    public Text HighScore;
    
    /// <summary>
    /// Standard start
    /// </summary>
    void Start()
    {
        MySpaceObject = GetComponent<SpaceObject>();
        Manager = GameManager.GetReference();
        MyCollider = GetComponent<CapsuleCollider>();

        Health = 100f;
        Control = 100f;
        ZoomedOut = false;
    }

    /// <summary>
    /// Standard update loop
    /// </summary>
    void Update()
    {
        if (Dead)
        {
            MySpaceObject.MyRigidBody.velocity = Vector3.zero;
            return;
        }
        
        CurrentSpeed = MySpaceObject.MyRigidBody.velocity.magnitude;
        // DebugText.SetText("Speed: " + CurrentSpeed);
        DebugText.SetText("Score: " + Score);
        
        UpdateInput();
        
        UpdateMicroJumps();

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

        if (HealthTimer >= 0f)
        {
            HealthTimer -= Time.deltaTime;
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

        if (Input.GetKeyDown(KeyCode.K))
        {
            DeductHealth(20f);
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

        if (Control > 100f)
        {
            Score += 400;
            Control = 100f;
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
        // This gives the players some wiggle in case of weird collision.
        if (HealthTimer > 0f)
        {
            return;
        }

        HealthTimer = 0.5f;
        
        Health -= amount;
        if (Health <= 0f)
        {
            Dead = true;
            MySprite.SetActive(false);
            MyCollider.enabled = false;
            ControlBurst.Play();
            GameOver.SetActive(true);
            HighScore.text = "Your Score: " + Score;
        }
        
        // Update UI
        Vector2 size = HealthBar.sizeDelta;
        size.x = 500f * (Health / 100f);
        HealthBar.sizeDelta = size;
    }

    /// <summary>
    /// At critical levels of control, small jumps in space will occur.
    /// </summary>
    public void UpdateMicroJumps()
    {
        if (Manager.GetCurrentControlState() > ControlState.Extreme)
        {
            if (MicroJumpTimer <= 0f)
            {
                if (Random.Range(0, 2) == 0)
                {
                    Vector3 newPosition = Manager.GenerateRandomPositionInBounds(transform.position, 5, 20);
                    if (!Physics.SphereCast(newPosition, 10f, Vector3.forward, out RaycastHit hit))
                    {
                        transform.position = newPosition;
                        ControlBurst.Play();
                    }
                }
                
                MicroJumpTimer = MicroJumpTime;
            }
            else
            {
                MicroJumpTimer -= Time.deltaTime;
            }
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

    /// <summary>
    /// Explode off of planets, taking a lot of damage
    /// </summary>
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 10)
        {
            DeductHealth(20f);
            MySpaceObject.MyRigidBody.AddExplosionForce(2000f, other.transform.position, 1000f);
        }
    }
}
