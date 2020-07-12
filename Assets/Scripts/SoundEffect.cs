using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    public float DestroyTime;

    // Update is called once per frame
    void Update()
    {
        DestroyTime -= Time.deltaTime;
        if (DestroyTime < 0f)
        {
            Destroy(gameObject);
        }
    }
}
