using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour
{
    public bool LockedOn;
    
    public float StandardSpeed;
    public int StandardDamage;

    public float Timer;

    public GameObject ExplosionPrefab;
    
    public AudioClip[] ExplosionSounds;
    public GameObject SoundEffectPrefab;

    /// <summary>
    /// Should be called by the player's ship after orienting it properly.
    /// </summary>
    public void Fire(float extraSpeed)
    {
        float speed = StandardSpeed;
        speed += extraSpeed;
        Rigidbody myRigidBody = GetComponent<Rigidbody>();
        myRigidBody.AddRelativeForce(speed, 0f, 0f, ForceMode.Impulse);
    }

    /// <summary>
    /// Standard update loop. Just destroy itself after sometime.
    /// </summary>
    private void Update()
    {
        Timer -= Time.deltaTime;
        if (Timer < 0f)
        {
            Destroy(gameObject);
            Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        }
    }

    public void ExplosionSound()
    {
        AudioSource sound = Instantiate(SoundEffectPrefab).GetComponent<AudioSource>();
        sound.clip = ExplosionSounds[Random.Range(0, ExplosionSounds.Length)];
        sound.Play();
    }
}
