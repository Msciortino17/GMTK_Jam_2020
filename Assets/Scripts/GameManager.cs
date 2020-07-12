using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

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
    public bool MainMenu;
    
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
    
    // Wormhole spawning
    public WormHole WormHoleRef;
    public GameObject WormHolePrefab;
    public float WormholeSpawnTimer;
    public float WormholeSpawnTime;

    public GameObject ZoomedInCamera;
    public GameObject NormalCamera;
    public GameObject ZoomedOutCamera;
    public GameObject NonZoomedInCameras;

    public float FadeInTimer;
    public Image FadeInRef;

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
        FadeInTimer = 1f;
        AsteroidSpawnTimer = 0f;
    }

    private void Update()
    {
        if (FadeInTimer > 0f)
        {
            if (FadeInRef == null || !FadeInRef.gameObject.activeInHierarchy)
            {
                FadeInTimer = 0f;
                return;
            }
            
            FadeInTimer -= Time.deltaTime;

            Color color = FadeInRef.color;
            color.a = FadeInTimer;
            FadeInRef.color = color;

            if (FadeInTimer <= 0f)
            {
                FadeInRef.gameObject.SetActive(false);
            }
        }
        
        if (MainMenu)
        {
            return;
        }
        
        SpawnAsteroids();

        if (GetCurrentControlState() > ControlState.Stable)
        {
            SpawnComets();
        }

        if (GetCurrentControlState() > ControlState.Unstable)
        {
            SpawnWormhole();
        }
        
    }

    /// <summary>
    /// This will spawn asteroids at a set frequency around the player, and send them flying towards you.
    /// A sphere cast is done so that asteroids don't spawn on top of things and cause craziness with physics.
    /// </summary>
    private void SpawnAsteroids()
    {
        if (AsteroidSpawnTimer <= 0f && CurrentAsteroidCount() < NumAsteroids)
        {
            float minDistance = (GetCurrentControlState() > ControlState.Extreme
                ? MinAsteroidDistance * 0.3f
                : MinAsteroidDistance);
            Vector3 position = GenerateRandomPositionInBounds(Player.transform.position, minDistance, MaxAsteroidDistance);
            if (Physics.SphereCast(position, 20f, Vector3.forward, out RaycastHit hit))
            {
                return;
            }
            
            Vector3 toPlayer = Player.transform.position - position;
            Quaternion rotation =  Quaternion.LookRotation(toPlayer, Vector3.up);
            GameObject asteroidObject = Instantiate(AsteroidPrefab, position, rotation, Asteroids);
            Asteroid asteroid = asteroidObject.GetComponent<Asteroid>();
            asteroid.GetComponent<Asteroid>().Fire(Random.Range(MinAsteroidSpeed, MaxAsteroidSpeed));
            asteroid.ControlBurst.Play();
            
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

    /// <summary>
    /// Spawns the wormhole randomly around the player.
    /// If out of control, will prioritize spawning it along the player's trajectory.
    /// </summary>
    private void SpawnWormhole()
    {
        if (WormholeSpawnTimer <= 0f && WormHoleRef == null)
        {
            ControlState state = GetCurrentControlState();
            Vector3 position = GenerateRandomPositionInBounds(Player.transform.position, 
                MinAsteroidDistance * 3f, MaxAsteroidDistance * 6f);

            if (state == ControlState.OutOfControl)
            {
                position = Player.Trajectory.normalized * 350f;
                position += Player.transform.position;
            }
            WormHoleRef = Instantiate(WormHolePrefab, position, Quaternion.identity, transform).GetComponent<WormHole>();
            
            WormholeSpawnTimer = state == ControlState.OutOfControl ? 5f : WormholeSpawnTime;
        }
        else
        {
            WormholeSpawnTimer -= Time.deltaTime;
        }
    }

    public Vector3 GenerateRandomPositionInBounds(Vector3 center, float minRadius, float maxRadius)
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
        if (MainMenu)
        {
            return ControlState.Stable;
        }
        
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

        if (Player.Control > 0.01f)
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
