using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WormHoleState
{
    Dormant,
    Slowing,
    Pulling,
    Teleport
}

public class WormHole : MonoBehaviour
{
    public PlayerShip PlayerRef;
    public WormHoleState CurrentState;
    public GameManager Manager;
    public float Timer;
    
    // Start is called before the first frame update
    void Start()
    {
        CurrentState = WormHoleState.Dormant;
        Manager = GameManager.GetReference();
        PlayerRef = Manager.Player;
        Timer = 15f;
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentState == WormHoleState.Dormant)
        {
            Timer -= Time.deltaTime;
            if (Timer <= 0f)
            {
                Manager.WormHoleRef = null;
                Destroy(gameObject);
            }
        }
        
        // Slow down the player once they're caught within range
        if (CurrentState == WormHoleState.Slowing)
        {
            PlayerRef.MySpaceObject.ApplyHeavyFriction();
            if (PlayerRef.MySpaceObject.MyRigidBody.velocity.magnitude < 2f)
            {
                CurrentState = WormHoleState.Pulling;
                Manager.ZoomedInCamera.SetActive(true);
                Manager.NonZoomedInCameras.SetActive(false);
            }
        }
        
        // Start directly pulling them in
        if (CurrentState == WormHoleState.Pulling)
        {
            PlayerRef.MySpaceObject.ApplyHeavyFriction();
            PlayerRef.MySpaceObject.ApplyGravity(transform.position, 20f);
            Vector3 toPlayer = PlayerRef.transform.position - transform.position;
            float distance = toPlayer.sqrMagnitude;
            if (distance < 100f)
            {
                CurrentState = WormHoleState.Teleport;
            }
        }
        
        // Once close enough, teleport them to a random location in the level and refill control.
        if (CurrentState == WormHoleState.Teleport)
        {
            GameManager manager = GameManager.GetReference();
            Vector3 position = manager.GenerateRandomPositionInBounds(Vector3.zero, 100f, manager.LevelRadius);
            if (!Physics.SphereCast(position, 20f, Vector3.forward, out RaycastHit hit))
            {
                PlayerRef.transform.position = position;
                PlayerRef.ControlBurst.Play();
                PlayerRef.MySpaceObject.MyRigidBody.velocity = Vector3.zero;
                CurrentState = WormHoleState.Dormant;
                Manager.ZoomedInCamera.SetActive(false);
                Manager.NonZoomedInCameras.SetActive(true);
                PlayerRef.Control = 100f;
                PlayerRef.ControlBar.UpdateSize(PlayerRef.Control);
                PlayerRef.UpdateStarParticles();
                Manager.WormHoleRef = null;
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerShip player = other.GetComponent<PlayerShip>();
        if (player != null)
        {
            CurrentState = WormHoleState.Slowing;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerShip player = other.GetComponent<PlayerShip>();
        if (player != null)
        {
            CurrentState = WormHoleState.Dormant;
            Manager.ZoomedInCamera.SetActive(false);
            Manager.NonZoomedInCameras.SetActive(true);
        }
    }
}
