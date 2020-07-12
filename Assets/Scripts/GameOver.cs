using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public GameObject MoreStuff;
    public Color SolidBlack;
    public Color Transparent;
    public Image Backdrop;

    public float PreTimer;
    public float PreTime;
    public bool Preing; // it's late... no time to name variables

    public float FadeTimer;
    public float FadeTime;
    public bool fading;

    private void Start()
    {
        FadeTimer = FadeTime;
        PreTimer = PreTime;
        fading = false;
        Preing = true;
    }

    private void Update()
    {
        if (Preing)
        {
            PreTimer -= Time.deltaTime;
            if (PreTimer < 0f)
            {
                fading = true;
                Preing = false;
            }
        }
        
        if (fading)
        {
            FadeTimer -= Time.deltaTime;
            float ratio = FadeTimer / FadeTime;
            // DebugText.SetText("alpha: " + Backdrop.color.a);

            Color color = Backdrop.color;
            color.a = 1f - ratio;
            Backdrop.color = color;
            
            // Backdrop.color = Color.Lerp(SolidBlack, Transparent, ratio);
            
            if (FadeTimer < 0f)
            {
                fading = false;
                MoreStuff.SetActive(true);
            }
        }
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
