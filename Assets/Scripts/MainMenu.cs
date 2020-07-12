using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject CreditsRef;
    
    /// <summary>
    /// Starts the game.
    /// </summary>
    public void Play()
    {
        SceneManager.LoadScene("Gameplay");
    }

    /// <summary>
    /// Toggles the credits menu
    /// </summary>
    public void Credits()
    {
        CreditsRef.SetActive(!CreditsRef.activeInHierarchy);
    }
}
