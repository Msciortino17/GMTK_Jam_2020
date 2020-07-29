using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour
{
	public bool IsComet;
	public SpaceObject MySpaceObject;
	public Rigidbody MyRigidBody;
	public int health;
	public int MinStartHealth;
	public int MaxStartHealth;
	public float DespawnTimer;

	public float ShipKnockbackForce;
	public float AsteroidKnockbackForce;

	public ParticleSystem ControlBurst;
	public GameManager Manager;

	public GameObject BreakParticlePrefab;

	private void Awake()
	{
		MySpaceObject = GetComponent<SpaceObject>();
		MyRigidBody = GetComponent<Rigidbody>();
		health = Random.Range(MinStartHealth, MaxStartHealth);
		AdjustScaleOnHealth();
		DespawnTimer = 5f;
		Manager = GameManager.GetReference();
	}

	/// <summary>
	/// Should be called by the manager after setting things up
	/// </summary>
	public void Fire(float speed)
	{
		Rigidbody myRigidBody = GetComponent<Rigidbody>();
		myRigidBody.AddRelativeForce(0f, 0f, speed * myRigidBody.mass, ForceMode.Impulse);
	}

	// Update is called once per frame
	void Update()
	{
		if (DespawnTimer < 0f)
		{
			GameManager manager = GameManager.GetReference();

			// Do a distance check and despawn if far away.
			Vector3 toPlayer = manager.Player.transform.position - transform.position;
			float distance = toPlayer.magnitude;
			if (distance > manager.MinAsteroidDistance)
			{
				Death();
			}
			else
			{
				DespawnTimer = 5f;
			}
		}
		else
		{
			DespawnTimer -= Time.deltaTime;
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		int damage = 1;
		bool hitByBullet = false;

		// Take damage from bullets, and destroy the bullet.
		Bullet bullet = other.gameObject.GetComponent<Bullet>();
		if (bullet != null)
		{
			damage = bullet.StandardDamage;
			bullet.ExplosionSound();
			Destroy(bullet.gameObject);
			Instantiate(bullet.ExplosionPrefab, bullet.transform.position, Quaternion.identity);
			hitByBullet = true;
		}

		// If we hit the ship, deduct some health, and knock it back.
		PlayerShip playerShip = other.gameObject.GetComponent<PlayerShip>();
		if (playerShip != null)
		{
			playerShip.DeductHealth(health * 3);
			playerShip.ShieldBurst.Play();
			playerShip.MySpaceObject.MyRigidBody.AddExplosionForce(ShipKnockbackForce, transform.position, 10f);
			playerShip.ShieldBounceAudioSource.Play();
		}

		MyRigidBody.AddExplosionForce(AsteroidKnockbackForce, other.transform.position, 100f);
		DeductHealth(damage, hitByBullet);
	}

	/// <summary>
	/// The scale should equal the health
	/// </summary>
	private void DeductHealth(int amount, bool killedByPlayer)
	{
		health -= amount;
		Instantiate(BreakParticlePrefab, transform.position, Quaternion.identity);
		if (health <= 0)
		{
			Death();
		}

		AdjustScaleOnHealth();
	}

	private void AdjustScaleOnHealth()
	{
		int healthMod = health;
		if (!IsComet)
		{
			healthMod += 1;
		}
		transform.localScale = new Vector3(healthMod * 3f, healthMod * 3f, healthMod * 3f);
	}

	/// <summary>
	/// Universal destroy method for the asteroid, used in several spots.
	/// </summary>
	private void Death()
	{
		Destroy(gameObject);
	}
}
