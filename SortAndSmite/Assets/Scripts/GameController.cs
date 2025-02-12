using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Different game states (We might not need all of these)
public enum GameState { 
    Menu,
    Running,
    Paused,
    Victory,
    Defeat
}

/// <summary>
///     This class will be used to control the main game systems.
///     i.e. GameStates, Win/Loss Conditions, etc.
/// </summary>
public class GameController : MonoBehaviour
{
    //Fields
    private GameState currentState;

    //Properties
    public GameState CurrentState
    {
        get { return currentState; }
        set
        {
            //If we want to do things before changing the state, we can do them here.
            currentState = value;
        }
    }

    void Start()
    {
        CurrentState = GameState.Running;
    }

    void Update()
    {
        //Might check for certain conditions to change state here
    }

    public void TogglePause()
    {
        if (CurrentState == GameState.Running)
            CurrentState = GameState.Paused;
        else if (CurrentState == GameState.Paused)
            CurrentState = GameState.Running;
    }
}
