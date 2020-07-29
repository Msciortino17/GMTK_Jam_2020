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

	// Weapons
	public GameObject BulletPrefab;
	public float BulletCost;
	public float WeaponTimer;

	// Stats
	public float Health;
	public float HealthTimer;
	public float Control;

	// Particles
	public ParticleSystem ControlBurst;
	public ParticleSystem ShieldBurst;
	public ParticleSystem StarsUnstable;
	public ParticleSystem StarsExtreme;
	public ParticleSystem StarsCritical;
	public ParticleSystem StarsOoC;

	// Trajectory calculation
	public Vector3 Trajectory;
	public Vector3 PrevPosition;
	public float TrajectoryTimer;

	// Game over
	public bool Dead;
	public GameObject GameOver;
	public GameObject MySprite;
	public CapsuleCollider MyCollider;
	public Text HighScore;

	// Sounds
	public AudioClip[] BlasterSounds;
	public AudioSource BlasterAudioSource;
	public AudioSource ShieldBounceAudioSource;

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
		DebugText.SetText("Health: " + Health + ", Control: " + Control);

		UpdateInput();

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

		if (WeaponTimer > 0f)
		{
			WeaponTimer -= Time.deltaTime;
		}

		// Steady depletion of shields when out of control.
		if (Control <= 0.01f)
		{
			DeductHealth(Time.deltaTime * 0.25f, true);
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

		if (Input.GetKeyDown(KeyCode.Space) && Control > 0.01f && WeaponTimer <= 0f)
		{
			GameObject bullet = Instantiate(BulletPrefab, transform.position, transform.rotation);
			// bullet.transform.rotation = transform.rotation;
			// bullet.transform.position = transform.position;
			bullet.GetComponent<Bullet>().Fire(CurrentSpeed);
			DeductControl(BulletCost);
			PlayBlaster();
			WeaponTimer = 0.25f;
		}
	}

	public void PlayBlaster()
	{
		BlasterAudioSource.clip = BlasterSounds[Random.Range(0, BlasterSounds.Length)];
		BlasterAudioSource.Play();
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
			Control = 100f;
		}

		UpdateStarParticles();
	}

	public void UpdateStarParticles()
	{
		StarsUnstable.gameObject.SetActive(Control < 76f);
		StarsExtreme.gameObject.SetActive(Control < 51f);
		StarsCritical.gameObject.SetActive(Control < 26f);
		StarsOoC.gameObject.SetActive(Control <= 0.1f);
	}

	/// <summary>
	/// Lower the health without letting it go negative
	/// </summary>
	public void DeductHealth(float amount, bool overrideTimer = false)
	{
		// This gives the players some wiggle in case of weird collision.
		if (!overrideTimer && HealthTimer > 0f)
		{
			return;
		}

		HealthTimer = 0.5f;

		Health -= amount;
		if (Health <= 0f)
		{
			Health = 0f;
			Dead = true;
			MySprite.SetActive(false);
			MyCollider.enabled = false;
			ControlBurst.Play();
			GameOver.SetActive(true);
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
			ShieldBurst.Play();
			MySpaceObject.MyRigidBody.AddExplosionForce(2000f, other.transform.position, 1000f);
			ShieldBounceAudioSource.Play();
		}
	}
}
