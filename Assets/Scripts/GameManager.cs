using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Generation stats
    public float LevelRadius;
    public int NumPlanets;
    
    // References
    public Transform Planets;
    
    // Prefabs
    public GameObject PlanetPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
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
