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
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        // First, generate the planets
        List<GameObject> generatedPlanets = new List<GameObject>();
        for (int i = 0; i < NumPlanets; i++)
        {
            // Create the planet
            GameObject planet = Instantiate(PlanetPrefab, Planets);

            int safety = 50;
            bool generated = true;
            do
            {
                // Generate a random position
                planet.transform.position = GenerateRandomPositionInBounds(20f);
            
                // Make sure it's not near another planet by comparing distance against others generated
                foreach (GameObject generatedPlanet in generatedPlanets)
                {
                    float distance = (generatedPlanet.transform.position - planet.transform.position).magnitude;
                    if (distance < 25f)
                    {
                        generated = false;
                    }
                }

                // For the first planet...
                if (generatedPlanets.Count == 0)
                {
                    generated = true;
                }

                safety--;

            } while (!generated && safety > 0);
            
            // Add it to the list
            generatedPlanets.Add(planet);
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
