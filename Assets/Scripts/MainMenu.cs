using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject CreditsRef;
    public Image FadeOutRef;
    public bool Playing;
    public float Fadetimer;

    public bool Tutorialing;
    public GameObject HowToPlayUIRef;
    public GameObject HowToPlayGameObjectRef;
    public Transform HowToRotateClockWise;
    public Transform HowToRotateCounterClockWise;
    public Transform HowToAccelerate;
    public Vector3 HowToAccelerateStartPosition;
    public Transform HowToZoomOut;
    public Transform HowToShoot;
    public GameObject BulletPrefab;
    public float ShootTimer;

    private void Start()
    {
        HowToAccelerateStartPosition = HowToAccelerate.position;
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    public void Play()
    {
        Playing = true;
        Fadetimer = 1f;
        FadeOutRef.gameObject.SetActive(true);
    }

    /// <summary>
    /// Toggles the credits menu
    /// </summary>
    public void Credits()
    {
        CreditsRef.SetActive(!CreditsRef.activeInHierarchy);
    }

    public void HowToPlay()
    {
        HowToPlayUIRef.SetActive(!HowToPlayUIRef.activeInHierarchy);
        HowToPlayGameObjectRef.SetActive(!HowToPlayGameObjectRef.activeInHierarchy);
        Tutorialing = !Tutorialing;
        
    }

    private void Update()
    {
        if (Playing)
        {
            Fadetimer -= Time.deltaTime;

            Color color = FadeOutRef.color;
            color.a = 1f - Fadetimer;
            FadeOutRef.color = color;

            if (Fadetimer <= 0f)
            {
                SceneManager.LoadScene("Gameplay");
            }
            
        }

        if (Tutorialing)
        {
            HowToRotateClockWise.Rotate(0f, 0f, Time.deltaTime * -90f);
            HowToRotateCounterClockWise.Rotate(0f, 0f, Time.deltaTime * 90f);
            HowToAccelerate.position = HowToAccelerateStartPosition + new Vector3(0f, Mathf.PingPong(Time.time, 4f) - 2f);
            float scale = Mathf.PingPong(Time.time * 0.2f, 0.75f) + 0.25f;
            HowToZoomOut.localScale = new Vector3(scale, scale * 1.5f, scale);

            ShootTimer -= Time.deltaTime;
            if (ShootTimer <= 0f)
            {
                ShootTimer = 1f;
                GameObject bullet = Instantiate(BulletPrefab, HowToShoot.transform.position, HowToShoot.transform.rotation);
                bullet.GetComponent<Bullet>().Fire(false, 0f);
            }
        }
    }
}
