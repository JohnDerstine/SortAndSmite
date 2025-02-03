using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// All user interactions and controls should happen here.
/// </summary>
public class PlayerController : MonoBehaviour
{
    //references
    private GameController gameController;

    //fields
    private float health;

    void Start()
    {
        gameController = gameObject.GetComponent<GameController>();
    }

    //Check for player inputs
    void Update()
    {
        //Check for Pause/Unpause input
        if (Keyboard.current.IsPressed((float)Key.Escape))
            gameController.TogglePause();

        //Don't alow the player to do anything if the game is paused
        if (gameController.CurrentState == GameState.Paused)
            return;
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;

        //We might want to flash the screen red or something, which can be done here.
    }
}
