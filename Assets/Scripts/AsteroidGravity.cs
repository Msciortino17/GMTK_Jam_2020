using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidGravity : MonoBehaviour
{
    public float Radius;
    public float Mass;
    
    public SphereCollider MySphereCollider;
    public List<Rigidbody> BulletsToPull;
    
    // Start is called before the first frame update
    void Start()
    {
        MySphereCollider = GetComponent<SphereCollider>();
        Radius = MySphereCollider.radius;
    }

    private void Update()
    {
        for (int i = 0; i < BulletsToPull.Count; i++)
        {
            Rigidbody bullet = BulletsToPull[i];
            
            // If the space object is destroyed before it leaves the bounds, do cleanup here.
            if (bullet == null)
            {
                BulletsToPull.RemoveAt(i);
                i--;
                continue;
            }
            
            Vector3 toCenter = transform.position - bullet.transform.position;
            toCenter.Normalize();
            bullet.AddForce(toCenter * Mass);
        }
    }

    /// <summary>
    /// Capture any space objects that come within range
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        Bullet bullet = other.GetComponent<Bullet>();
        if (bullet != null && !bullet.LockedOn)
        {
            BulletsToPull.Add(bullet.GetComponent<Rigidbody>());
            bullet.LockedOn = true;
        }
    }

    /// <summary>
    /// Release any space objects that leave
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        Bullet bullet = other.GetComponent<Bullet>();
        if (bullet != null)
        {
            BulletsToPull.Remove(bullet.GetComponent<Rigidbody>());
            bullet.LockedOn = false;
        }
    }
}
