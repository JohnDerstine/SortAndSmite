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
    [SerializeField]
    private VisualTreeAsset popup;
    [SerializeField]
    private Boss boss;

    [SerializeField]
    private SpriteRenderer tint;

    //Fields
    private GameState currentState;
    private Button start;
    public bool tutorialEnabled = true;
    private SpriteRenderer currentFocus;
    private int originalOrder;
    private int tintOrder = -11;
    private VisualElement root;
    private VisualElement patienceArt;
    private VisualElement patienceContainer;
    private VisualElement healthContainer;
    private VisualElement patienceProgress;
    private VisualElement healthProgress;
    private VisualElement healthIcon;
    private VisualElement holdTint;
    private bool step1;
    private bool step2;
    public bool boxSpawned;
    private bool step3;
    private bool step4;
    public bool itemSorted;
    private bool step5;
    private bool step6;
    private TemplateContainer popUpElement;

    private string step1Text = "This is your landlord, which is trying to evict you! You've made your payments on time and have been nothing but respectful!";
    private string step2Text = "This is his health bar. If you want to live in peace, you'll have to reduce his health to 0.";
    private string step3Text = "Boxes will appear on this conveyor belt containg a label. Yor goal will be to sort items into the box that fit the label.";
    private string step4Text = "Items will fall from the sky as your landlord throws them at you! These are the items you must sort into the boxes. Start by sorting this item into the box.";
    private string step5Text = "Step 5";
    private string step6Text = "Step 6";

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
        root = mainDocument.rootVisualElement;

        holdTint = root.Q("HoldTint");
        healthIcon = root.Q("HpIcon");
        patienceProgress = root.Q("Patience").Q(className: "unity-progress-bar__progress");
        healthProgress = root.Q("Health").Q(className: "unity-progress-bar__progress");
        patienceArt = root.Q("Container");
        healthContainer = root.Q("Health").Q(className: "unity-progress-bar__background");
        patienceContainer = root.Q("Patience").Q(className: "unity-progress-bar__background");
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
        if (tutorialEnabled)
        {
            CurrentState = GameState.Paused;
            StartCoroutine(StartTutorial());
        }
        else
            CurrentState = GameState.Running;
    }

    #region Tutorial

    private IEnumerator StartTutorial()
    {
        SendAllToBack();
        AddPopup(Screen.width / 2, Screen.height / 2, step1Text);
        BringToFrontObject(GameObject.Find("Boss").GetComponentInChildren<SpriteRenderer>());
        yield return new WaitWhile(() => !step1);

        ResetObjectOrder();
        TintEverything();
        SendAllToBack();
        BringHealthForward();
        AddPopup(Screen.width / 2, Screen.height / 6, step2Text);
        Debug.Log("Heath Time");
        yield return new WaitWhile(() => !step2);

        SendAllToBack();
        ResetObjectOrder();
        CurrentState = GameState.Running;
        Debug.Log("Waiting for box to spawn");
        yield return new WaitWhile(() => !boxSpawned);

        CurrentState = GameState.Paused;

        BringToFrontObject(GameObject.Find("ConveyorBelt").GetComponent<Conveyor>().activeBox);
        AddPopup(Screen.width / 9, Screen.height / 2, step3Text);
        Debug.Log("Box Time");
        yield return new WaitWhile(() => !step3);

        SendAllToBack();
        ResetObjectOrder();
        CurrentState = GameState.Running;
        Debug.Log("Waiting for item to fall");
        yield return new WaitForSeconds(1.5f);

        CurrentState = GameState.Paused;

        BringToFrontObject(boss.activeItem.GetComponent<SpriteRenderer>());
        AddPopup(Screen.width / 9, Screen.height / 2, step4Text);
        Debug.Log("Item time");
        yield return new WaitWhile(() => !step4);

        ResetObjectOrder();
        SendAllToBack();
        Debug.Log("Waiting for item to be sorted");
        yield return new WaitWhile(() => !itemSorted);

        SendAllToBack();
        ResetObjectOrder();
        BringPatienceForward();
        AddPopup(Screen.width / 9, Screen.height / 2, step5Text);
        Debug.Log("Patience Time");
        yield return new WaitWhile(() => !step5);

        SendAllToBack();
        BringHoldForward();
        AddPopup(0, 0, step6Text);
        Debug.Log("Hold Time");
        yield return new WaitWhile(() => !step6);

        ResetObjectOrder();
        BringAllToFront();
    }

    private void BringToFrontObject(SpriteRenderer sr)
    {
        currentFocus = sr;
        originalOrder = sr.sortingOrder;
        sr.sortingOrder = 10;
        tint.sortingOrder = 9;
    }

    private void TintEverything()
    {
        tint.sortingOrder = 11;
    }
    private void ResetObjectOrder()
    {
        currentFocus.sortingOrder = originalOrder;
        tint.sortingOrder = tintOrder;
    }

    private void SendAllToBack()
    {
        healthIcon.style.unityBackgroundImageTintColor = new Color(0.4f, 0.4f, 0.4f, 1);
        patienceArt.style.unityBackgroundImageTintColor = new Color(0.4f, 0.4f, 0.4f, 1);
        healthProgress.style.backgroundColor = new Color(0.6f, 0f, 0f);
        patienceProgress.style.backgroundColor = new Color(0.4f, 0.4f, 0.4f);
        patienceContainer.style.backgroundColor = new Color(0.4f, 0.4f, 0.4f);
        healthContainer.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f);
        holdTint.style.backgroundColor = new Color(0, 0, 0, 0.6f);
    }
    private void BringAllToFront()
    {
        holdTint.style.backgroundColor = new Color(0, 0, 0, 0f);
        patienceArt.style.unityBackgroundImageTintColor = new Color(0, 0, 0, 0);
        patienceProgress.style.backgroundColor = new Color(0.9058824f, 0.9058824f, 0.9058824f);
        patienceContainer.style.backgroundColor = new Color(0.9058824f, 0.9058824f, 0.9058824f);
        healthIcon.style.unityBackgroundImageTintColor = new Color(0, 0, 0, 0);
        healthProgress.style.backgroundColor = new Color(1f, 0f, 0f);
        healthContainer.style.backgroundColor = new Color(0.95f, 0.95f, 0.95f);
    }

    private void BringHoldForward()
    {
        holdTint.style.backgroundColor = new Color(0, 0, 0, 0f);
    }

    private void BringPatienceForward()
    {
        patienceArt.style.unityBackgroundImageTintColor = new Color(1, 1, 1, 1);
        patienceProgress.style.backgroundColor = new Color(0.9058824f, 0.9058824f, 0.9058824f);
        patienceContainer.style.backgroundColor = new Color(0.9058824f, 0.9058824f, 0.9058824f);
    }

    private void BringHealthForward()
    {
        healthIcon.style.unityBackgroundImageTintColor = new Color(1, 1, 1, 1);
        healthProgress.style.backgroundColor = new Color(1f, 0f, 0f);
        healthContainer.style.backgroundColor = new Color(0.95f, 0.95f, 0.95f);
    }

    private void AddPopup(float left, float top, string text)
    {
        popUpElement = popup.Instantiate();
        mainDocument.rootVisualElement.Add(popUpElement);
        popUpElement.style.position = Position.Absolute;
        popUpElement.style.left = left;
        popUpElement.style.top = top;
        popUpElement.Q<Label>().text = text;
        popUpElement.Q<Button>().clicked += Advance;
    }

    private void Advance()
    {
        if (!step1)
            step1 = true;
        else if (!step2)
            step2 = true;
        else if (!step3)
            step3 = true;
        else if (!step4)
            step4 = true;
        else if (!step5)
            step5 = true;
        else if (!step6)
        {
            step6 = true;
            CurrentState = GameState.Running;
        }

        mainDocument.rootVisualElement.Remove(popUpElement);
        popUpElement = null;
    }
    #endregion
}
