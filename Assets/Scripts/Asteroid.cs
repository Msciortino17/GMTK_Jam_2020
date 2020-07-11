using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour
{
    public SpaceObject MySpaceObject;
    public Rigidbody MyRigidBody;
    public int health;
    public int MinStartHealth;
    public int MaxStartHealth;
    public float Timer;

    public float ShipKnockbackForce;
    public float AsteroidKnockbackForce;

    private void Awake()
    {
        MySpaceObject = GetComponent<SpaceObject>();
        MyRigidBody = GetComponent<Rigidbody>();
        health = Random.Range(MinStartHealth, MaxStartHealth);
        transform.localScale = new Vector3(health * 3f, health * 3f, health * 3f);
        Timer = 10f;
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
        // todo put random stuff in here when control is low
        
        // Every 15 seconds, do a distance check and despawn if far away.
        if (Timer < 0f)
        {
            GameManager manager = GameManager.GetReference();
            Vector3 toPlayer = manager.Player.transform.position - transform.position;
            float distance = toPlayer.magnitude;
            if (distance > manager.MinAsteroidDistance)
            {
                Death();
            }
            else
            {
                Timer = 5f;
            }
        }
        else
        {
            Timer -= Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        int damage = 1;
        
        // Take damage from bullets, and destroy the bullet.
        Bullet bullet = other.gameObject.GetComponent<Bullet>();
        if (bullet != null)
        {
            damage = bullet.IsUpgraded ? bullet.UpgradedDamage : bullet.StandardDamage;
            Destroy(bullet.gameObject);
        }
        
        // If we hit the ship, deduct some health, and knock it back.
        PlayerShip playerShip = other.gameObject.GetComponent<PlayerShip>();
        if (playerShip != null)
        {
            playerShip.DeductHealth(health * 3);
            playerShip.MySpaceObject.MyRigidBody.AddExplosionForce(ShipKnockbackForce, transform.position, 10f);
        }

        MyRigidBody.AddExplosionForce(AsteroidKnockbackForce, other.transform.position, 100f);
        DeductHealth(damage);
    }

    /// <summary>
    /// The scale should equal the health
    /// </summary>
    private void DeductHealth(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            // todo spawn loot here
            Death();
        }
        transform.localScale = new Vector3(health * 3f, health * 3f, health * 3f);
    }

    /// <summary>
    /// Universal destroy method for the asteroid, used in several spots.
    /// </summary>
    private void Death()
    {
        Destroy(gameObject);
    }
}
