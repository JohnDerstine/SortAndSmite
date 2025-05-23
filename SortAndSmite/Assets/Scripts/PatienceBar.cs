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
    private VisualElement container;
    private VisualElement progress;
    private VisualElement root;
    private Vector3 barDefault;
    private Vector3 containerDefault;
    Color baseColor;

    void Start()
    {
        root = document.rootVisualElement;
        bar = root.Q<VisualElement>("left").Q<ProgressBar>("Patience");
        barDefault = bar.transform.position;
        container = root.Q<VisualElement>("Container");
        containerDefault = container.transform.position;
        progress = bar.Q(className: "unity-progress-bar__progress");
        baseColor = new Color(0.9058824f, 0.9058824f, 0.9058824f , 1);
    }

    // Reduce patience at a constant rate
    void Update()
    {
        if (gameController.CurrentState != GameState.Running)
            return;
        if ((gameController.tutorialEnabled && !gameController.gameplayStarted))
            return;

        AdjustPatience(-2.75f * Time.deltaTime); //Multiply by difficulty/boss modifier??

        if (bar.value <= 0)
            gameController.CurrentState = GameState.Defeat;
        else if (bar.value <= 25)
        {
            Vector3 randVector = new Vector3();
            randVector.x = Random.Range(-5, 5);
            randVector.y = Random.Range(-5, 5);
            bar.transform.position = Vector3.Lerp(bar.transform.position, randVector, 0.5f);
            container.transform.position = Vector3.Lerp(bar.transform.position, randVector, 0.25f);
        }
        else
        {
            bar.transform.position = barDefault;
            container.transform.position = containerDefault;
        }
    }

    //Input positive value to restore patience, negative to reduce
    public void AdjustPatience(float patience)
    {
        bar.value += patience;
        if (bar.value > 100)
            bar.value = 100;

        if (patience > 0)
            StartCoroutine(FlashColor(Color.green));
        else if (patience < -1)
            StartCoroutine(FlashColor(Color.red));
    }

    private IEnumerator FlashColor(Color color)
    {
        float time = 0;
        progress.style.backgroundColor = color;
        while (progress.style.backgroundColor != baseColor)
        {
            StyleColor fadeColor = new StyleColor(Color.Lerp(color, baseColor, time));
            progress.style.backgroundColor = fadeColor;
            time += Time.deltaTime * 3f;
            yield return null;
        }
        progress.style.backgroundColor = baseColor;
    }
}
