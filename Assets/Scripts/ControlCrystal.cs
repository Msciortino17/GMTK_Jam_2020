using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCrystal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerShip player = other.gameObject.GetComponent<PlayerShip>();
        if (player != null)
        {
            player.DeductControl(-20f);
            player.ControlBurst.Play();
            player.Score += 100;
            player.PlayBlaster();
            Destroy(gameObject);
        }
    }
}
