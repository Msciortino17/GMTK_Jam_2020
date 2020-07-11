using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public float Radius;
    public float Mass;
    
    public SphereCollider MySphereCollider;
    public List<SpaceObject> ObjectsToPull;
    
    // Start is called before the first frame update
    void Start()
    {
        MySphereCollider = GetComponent<SphereCollider>();
        Radius = MySphereCollider.radius;
    }

    private void Update()
    {
        for (int i = 0; i < ObjectsToPull.Count; i++)
        {
            SpaceObject spaceObject = ObjectsToPull[i];
            
            // If the space object is destroyed before it leaves the bounds, do cleanup here.
            if (spaceObject == null)
            {
                ObjectsToPull.RemoveAt(i);
                i--;
                continue;
            }
            
            spaceObject.ApplyGravity(transform.position, Mass * Time.deltaTime);
        }
    }

    /// <summary>
    /// Capture any space objects that come within range
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        SpaceObject spaceObject = other.GetComponent<SpaceObject>();
        if (spaceObject != null)
        {
            ObjectsToPull.Add(spaceObject);
        }
    }

    /// <summary>
    /// Release any space objects that leave
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        SpaceObject spaceObject = other.GetComponent<SpaceObject>();
        if (spaceObject != null)
        {
            ObjectsToPull.Remove(spaceObject);
        }
    }
}
