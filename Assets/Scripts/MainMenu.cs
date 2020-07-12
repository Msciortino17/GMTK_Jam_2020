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
    }
}
