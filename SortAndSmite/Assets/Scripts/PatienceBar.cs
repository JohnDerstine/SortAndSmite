using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PatienceBar : MonoBehaviour
{
    //refernces
    [SerializeField]
    private GameController gameController;
    [SerializeField]
    private UIDocument document;

    //fields
    private ProgressBar bar;
    private VisualElement root;

    void Start()
    {
        root = document.rootVisualElement;
        bar = root.Q<VisualElement>("left").Q<ProgressBar>("Patience");
    }

    // Reduce patience at a constant rate
    void Update()
    {
        if (gameController.CurrentState != GameState.Running)
            return;

        AdjustPatience(-3 * Time.deltaTime); //Multiply by difficulty/boss modifier??

        if (bar.value <= 0)
            gameController.CurrentState = GameState.Defeat;

        //maybe add flashing when the bar is low?
    }

    //Input positive value to restore patience, negative to reduce
    public void AdjustPatience(float patience)
    {
        bar.value += patience;
        if (bar.value > 100)
            bar.value = 100;
    }
}
