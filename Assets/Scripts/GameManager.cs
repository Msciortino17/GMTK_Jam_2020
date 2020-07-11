using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ControlState
{
    Stable,
    Unstable,
    Extreme,
    Critical,
    OutOfControl
}

public class GameManager : MonoBehaviour
{
    private static GameManager reference;
    
    // Level info
    public float LevelRadius;
    
    // References
    public PlayerShip Player;
    
    // Asteroid spawning
    public Transform Asteroids;
    public GameObject AsteroidPrefab;
    public int NumAsteroids;
    public float AsteroidSpawnTimer;
    public float AsteroidSpawnTime;
    public float MinAsteroidDistance;
    public float MaxAsteroidDistance;
    public float MinAsteroidSpeed;
    public float MaxAsteroidSpeed;
    
    // Comet spawning
    public Transform Comets;
    public GameObject CometPrefab;
    public int NumComets;
    public float CometSpawnTimer;
    public float CometSpawnTime;
    public float MinCometSpeed;
    public float MaxCometSpeed;

    /// <summary>
    /// Universal method to grab a reference.
    /// </summary>
    public static GameManager GetReference()
    {
        if (reference == null)
        {
            reference = GameObject.Find("Manager").GetComponent<GameManager>();
        }

        return reference;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        AsteroidSpawnTimer = 0f;
    }

    private void Update()
    {
        SpawnAsteroids();
        SpawnComets();
    }

    /// <summary>
    /// This will spawn asteroids at a set frequency around the player, and send them flying towards you.
    /// A sphere cast is done so that asteroids don't spawn on top of things and cause craziness with physics.
    /// </summary>
    private void SpawnAsteroids()
    {
        if (AsteroidSpawnTimer <= 0f && CurrentAsteroidCount() < NumAsteroids)
        {
            Vector3 position = GenerateRandomPositionInBounds(Player.transform.position, MinAsteroidDistance, MaxAsteroidDistance);
            if (Physics.SphereCast(position, 20f, Vector3.forward, out RaycastHit hit))
            {
                return;
            }
            
            Vector3 toPlayer = Player.transform.position - position;
            Quaternion rotation =  Quaternion.LookRotation(toPlayer, Vector3.up);
            GameObject asteroid = Instantiate(AsteroidPrefab, position, rotation, Asteroids);
            asteroid.GetComponent<Asteroid>().Fire(Random.Range(MinAsteroidSpeed, MaxAsteroidSpeed));
            
            AsteroidSpawnTimer = AsteroidSpawnTime;
        }
        else
        {
            AsteroidSpawnTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Similar to the SpawnAsteroids method, but uses a different set of parameters that should be configured for
    /// rarer but more faster and prettier comets.
    /// </summary>
    private void SpawnComets()
    {
        if (CometSpawnTimer <= 0f && CurrentCometCount() < NumComets)
        {
            Vector3 position = GenerateRandomPositionInBounds(Player.transform.position, MinAsteroidDistance, MaxAsteroidDistance);
            if (Physics.SphereCast(position, 10f, Vector3.forward, out RaycastHit hit))
            {
                return;
            }
            
            Vector3 toPlayer = Player.transform.position - position;
            Quaternion rotation = Quaternion.LookRotation(toPlayer, Vector3.up);
            GameObject comet = Instantiate(CometPrefab, position, rotation, Comets);
            comet.GetComponent<Asteroid>().Fire(Random.Range(MinCometSpeed, MaxCometSpeed));
            
            CometSpawnTimer = CometSpawnTime;
        }
        else
        {
            CometSpawnTimer -= Time.deltaTime;
        }
    }

    private Vector3 GenerateRandomPositionInBounds(Vector3 center, float minRadius, float maxRadius)
    {
        float n = Random.Range(-1f, 1f);
        float r = n * 2 * Mathf.PI;
        float d = Random.Range(minRadius, maxRadius);
        float x = d * Mathf.Cos(r);
        float y = d * Mathf.Sin(r);
        return new Vector3(x, y, 0f) + center;
    }

    /// <summary>
    /// Returns one of the 5 states depending on the current value of Control the player has.
    /// </summary>
    public ControlState GetCurrentControlState()
    {
        if (Player.Control > 75f)
        {
            return ControlState.Stable;
        }

        if (Player.Control > 50f)
        {
            return ControlState.Unstable;
        }

        if (Player.Control > 25f)
        {
            return ControlState.Extreme;
        }

        if (Player.Control > 0f)
        {
            return ControlState.Critical;
        }

        return ControlState.OutOfControl;
    }

    /// <summary>
    /// Checks how many children the asteroids parent object has to determine asteroid count.
    /// </summary>
    private int CurrentAsteroidCount()
    {
        return transform.Find("Asteroids").childCount;
    }

    /// <summary>
    /// Checks how many children the comets parent object has to determine asteroid count.
    /// </summary>
    private int CurrentCometCount()
    {
        return transform.Find("Comets").childCount;
    }
    
}
