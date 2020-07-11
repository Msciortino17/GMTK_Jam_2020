using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager reference;
    
    // Generation stats
    public float LevelRadius;
    public int NumAsteroids;
    
    // References
    public Transform Asteroids;
    
    // Prefabs
    public GameObject AsteroidPrefab;

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
        SpawnAsteroids();
    }

    private void SpawnAsteroids()
    {
        for (int i = 0; i < NumAsteroids; i++)
        {
            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            Vector3 position = GenerateRandomPositionInBounds(50f);
            GameObject asteroid = Instantiate(AsteroidPrefab, position, rotation, Asteroids);
            asteroid.GetComponent<Asteroid>().Fire(Random.Range(5f, 15f));
        }
    }

    private Vector3 GenerateRandomPositionInBounds(float minRadius = 0)
    {
        float n = Random.Range(-1f, 1f);
        float r = n * 2 * Mathf.PI;
        float d = Random.Range(minRadius, LevelRadius);
        float x = d * Mathf.Cos(r);
        float y = d * Mathf.Sin(r);
        return new Vector3(x, y, 0f);
    }
    
}
