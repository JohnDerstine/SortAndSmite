using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
    //References
    [SerializeField]
    private UIDocument mainDocument;
    [SerializeField]
    private UIDocument startDocument;

    //Fields
    private GameState currentState;
    private Button start;

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

    void Awake()
    {
        CurrentState = GameState.Menu;
        start = startDocument.rootVisualElement.Q<Button>("SortButton");
        start.clickable = new Clickable(e => startGame());
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

    private void startGame()
    {
        startDocument.enabled = false;
        CurrentState = GameState.Running;
    }
}
