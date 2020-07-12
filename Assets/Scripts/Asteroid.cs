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
    public float VelocityChangeTimer;

    public float ShipKnockbackForce;
    public float AsteroidKnockbackForce;

    public ParticleSystem ControlBurst;
    public GameManager Manager;

    public GameObject ControlCrystalPrefab;
    public GameObject BreakParticlePrefab;

    private void Awake()
    {
        MySpaceObject = GetComponent<SpaceObject>();
        MyRigidBody = GetComponent<Rigidbody>();
        health = Random.Range(MinStartHealth, MaxStartHealth);
        transform.localScale = new Vector3((health + 1) * 3f, (health + 1) * 3f, (health + 1) * 3f);
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
        if (VelocityChangeTimer < 0f)
        {
            // If control levels are unstable, randomly apply forces
            ControlState state = Manager.GetCurrentControlState();
            if (state > ControlState.Stable)
            {
                int stateIntInverse = 5 - (int) state;
                bool bursted = false;
                
                // Changing velocity
                if (Random.Range(0, stateIntInverse) == 0)
                {
                    Vector3 direction = Vector3.zero;
                    if (Random.Range(0, 2) == 0)
                    {
                        direction = (Manager.Player.transform.position - transform.position).normalized;
                    }
                    else
                    {
                        direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized;
                    }
                    float power = Random.Range(20, 40) * MyRigidBody.mass;
                    MyRigidBody.velocity = Vector3.zero;
                    MyRigidBody.AddRelativeForce(direction * power, ForceMode.Impulse);
                    ControlBurst.Play();
                    bursted = true;
                }
                
                // Changing health
                if (state > ControlState.Unstable && Random.Range(0, stateIntInverse) == 0)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        DeductHealth(1, false);
                    }
                    else
                    {
                        DeductHealth(-1, false);
                    }

                    // Don't want to burst twice if it happens to change direction and size
                    if (!bursted)
                    {
                        ControlBurst.Play();
                    }
                }
            }

            VelocityChangeTimer = 2f;
        }
        else
        {
            VelocityChangeTimer -= Time.deltaTime;
        }
        
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
            damage = bullet.IsUpgraded ? bullet.UpgradedDamage : bullet.StandardDamage;
            Destroy(bullet.gameObject);
            Instantiate(bullet.ExplosionPrefab, bullet.transform.position, Quaternion.identity);
            hitByBullet = true;
        }
        
        // If we hit the ship, deduct some health, and knock it back.
        PlayerShip playerShip = other.gameObject.GetComponent<PlayerShip>();
        if (playerShip != null)
        {
            playerShip.DeductHealth(health * 3);
            playerShip.MySpaceObject.MyRigidBody.AddExplosionForce(ShipKnockbackForce, transform.position, 10f);
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
        if (killedByPlayer)
        {
            Manager.Player.Score += IsComet ? 500 : 100;
        }
        if (health <= 0)
        {
            if (killedByPlayer && (IsComet || Random.Range(0, 4) == 0))
            {
                Instantiate(ControlCrystalPrefab, transform.position, Quaternion.identity);
            }
            if (killedByPlayer)
            {
                Manager.Player.Score += IsComet ? 2000 : 400;
            }
            Death();
        }
        transform.localScale = new Vector3((health + 1) * 3f, (health + 1) * 3f, (health + 1) * 3f);
    }

    /// <summary>
    /// Universal destroy method for the asteroid, used in several spots.
    /// </summary>
    private void Death()
    {
        Destroy(gameObject);
    }
}
