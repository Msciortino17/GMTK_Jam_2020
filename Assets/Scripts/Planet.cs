using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Planet : MonoBehaviour
{
    public Transform Sun;
    public float rotateSpeed;

    public float MinRotateSpeed;
    public float MaxRotateSpeed;

    public float RandomSpeedTimer;
    
    public ParticleSystem ControlBurst;

    private void Start()
    {
        RandomSpeedTimer = Random.Range(0f, 10f); // This makes sure they don't all move at the same time.
    }

    private void Update()
    {
        transform.RotateAround(Sun.position, Vector3.forward, rotateSpeed * Time.deltaTime);
        
        if (RandomSpeedTimer < 0f)
        {
            GameManager manager = GameManager.GetReference();
            
            // If control levels are unstable, randomly change speeds
            ControlState state = manager.GetCurrentControlState();
            if (state > ControlState.Stable)
            {
                int stateIntInverse = 5 - (int) state;
                if (Random.Range(0, stateIntInverse) == 0)
                {
                    rotateSpeed = Random.Range((int) state, 3f * (int) state);
                    if (state > ControlState.Extreme)
                    {
                        if (Random.Range(0, 2) == 0)
                        {
                            rotateSpeed *= -1;
                        }
                    }

                    ControlBurst.Play();
                }
            }

            RandomSpeedTimer = 10f;
        }
        else
        {
            RandomSpeedTimer -= Time.deltaTime;
        }
    }
}
