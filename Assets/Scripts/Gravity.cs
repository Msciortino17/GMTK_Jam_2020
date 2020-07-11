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
        foreach (SpaceObject spaceObject in ObjectsToPull)
        {
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
