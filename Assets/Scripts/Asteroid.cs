using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour
{
    public int health;

    private void Start()
    {
        health = Random.Range(3, 10);
        UpdateSizeOnHealth();
    }

    /// <summary>
    /// Should be called by the manager after setting things up
    /// </summary>
    public void Fire(float speed)
    {
        Rigidbody myRigidBody = GetComponent<Rigidbody>();
        myRigidBody.AddRelativeForce(speed, 0f, 0f, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        // todo put random stuff in here when control is low
    }

    /// <summary>
    /// Blow up the asteroid when hit by bullets
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        Bullet bullet = other.gameObject.GetComponent<Bullet>();
        if (bullet != null)
        {
            health -= bullet.IsUpgraded ? bullet.UpgradedDamage : bullet.StandardDamage;
            if (health <= 0)
            {
                // todo spawn loot here
                Destroy(gameObject);
            }
            UpdateSizeOnHealth();
            Destroy(bullet.gameObject);
        }
    }

    /// <summary>
    /// The scale should equal the health
    /// </summary>
    private void UpdateSizeOnHealth()
    {
        transform.localScale = new Vector3(health, health, 1f);
    }
}
