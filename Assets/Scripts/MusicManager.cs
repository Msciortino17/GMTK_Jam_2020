using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource NormalMusic;
    public AudioSource OoCMusic;
    public AudioSource CurrentMusic;
    public AudioSource QuietMusic;
    public bool OoC;
    public bool Lowering;
    public bool Rising;
    public GameManager Manager;

    public float TransitionRate;
    
    public float MaxVolumeNormal;
    public float MaxVolumeOoC;
    
    // Start is called before the first frame update
    void Start()
    {
        Manager = GameManager.GetReference();
    }

    // Update is called once per frame
    void Update()
    {
        if (OoC)
        {
            if (Manager.GetCurrentControlState() != ControlState.OutOfControl)
            {
                OoC = false;
                CurrentMusic = NormalMusic;
                QuietMusic = OoCMusic;
                Lowering = true;
            }
        }
        else
        {
            if (Manager.GetCurrentControlState() == ControlState.OutOfControl)
            {
                OoC = true;
                CurrentMusic = OoCMusic;
                QuietMusic = NormalMusic;
                Lowering = true;
            }
        }

        if (Lowering)
        {
            QuietMusic.volume -= TransitionRate * Time.deltaTime;
            if (QuietMusic.volume <= 0f)
            {
                QuietMusic.volume = 0f;
                Lowering = false;
                Rising = true;
            }
        }

        if (Rising)
        {
            CurrentMusic.volume += TransitionRate * Time.deltaTime;
            float max = OoC ? MaxVolumeOoC : MaxVolumeNormal;
            if (CurrentMusic.volume >= max)
            {
                CurrentMusic.volume = max;
                Rising = false;
            }
        }
        
    }
}
