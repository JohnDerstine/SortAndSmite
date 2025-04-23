using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

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
    private VisualTreeAsset pause;
    [SerializeField]
    private Boss boss;
    [SerializeField]
    private Conveyor conveyor;
    [SerializeField]
    private GameObject victoryText;
    [SerializeField]
    private GameObject defeatText;

    [SerializeField]
    private SpriteRenderer tint;

    //Fields
    private GameState currentState;
    private Button start;
    public bool tutorialEnabled;
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
    private bool step42;
    public bool itemSorted;
    private bool step5;
    private bool step6;
    private bool step7;
    public bool boxLeft;
    private bool step8;
    private bool step9;
    private TemplateContainer popUpElement;
    public bool gameplayStarted = false;
    public bool tutorialOver = true;
    private TemplateContainer pauseElement;

    private string step1Text = "This is your landlord, who is trying to evict you! You've made your payments on time and have been nothing but respectful!";
    private string step2Text = "This is his health bar. If you want to live in peace, you'll have to completely drain his health bar.";
    private string step3Text = "Boxes will appear on this conveyor belt, each with a label.";
    private string step4Text = "Items will fall from the sky as your landlord throws them at you! Your goal will be to sort these items into boxes with a matching label.";
    private string step4Text2 = "Some items can be sorted into multiple categories. Sort this item into the box by clicking and dragging it.";
    private string step5Text = "This is the patience bar. You'll lose patience over time, and gain patience by correctly sorting items. If you sort an item wrong, you will lose a large chunk of patience.";
    private string step6Text = "You can tell if an item is sorted correctly, because the patience bar will flash green. Otherwise it will flash red. If then patience runs out, you'll get evicted!";
    private string step7Text = "This is the hold area. You can store up to 3 items here for later. Just click and drag an item into one of the slots to store it. Click and drag a stored item to retrieve it.";
    private string step8Text = "When a box reaches the end of the conveyor belt, it is emptied. You deal damage to the boss for every correctly sorted item in that box.";
    private string step9Text = "The tutorial is now over, good luck sorting!";

    //Properties
    public GameState CurrentState
    {
        get { return currentState; }
        set
        {
            //If we want to do things before changing the state, we can do them here.
            currentState = value;

            if (CurrentState == GameState.Paused)
            {
                foreach (GameObject item in boss.activeItems)
                    item.GetComponent<SortableItem>().SetItemBody(RigidbodyType2D.Static);

                if (tutorialOver)
                {
                    SendAllToBack();
                    TintEverything();
                    pauseElement = pause.Instantiate();
                    pauseElement.style.position = Position.Absolute;
                    pauseElement.Q<Button>("ResumeButton").clicked += TogglePause;
                    pauseElement.Q<Button>("RestartButton").clicked += RestartGame;
                    root.Add(pauseElement);
                }
            }
            else if (CurrentState == GameState.Running)
            {
                foreach (GameObject item in boss.activeItems)
                    item.GetComponent<SortableItem>().SetItemBody(RigidbodyType2D.Dynamic);

                if (tutorialOver)
                {
                    BringAllToFront();
                    ResetObjectOrder();
                    if (pauseElement != null)
                    {
                        root.Remove(pauseElement);
                        pauseElement = null;
                    }
                }
            }
            else if (CurrentState == GameState.Victory)
            {
                mainDocument.rootVisualElement.Clear();
                victoryText.SetActive(true);
            }
            else if (CurrentState == GameState.Defeat)
            {
                mainDocument.rootVisualElement.Clear();
                defeatText.SetActive(true);
            }
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
        if (Input.GetKeyDown(KeyCode.Escape) && tutorialOver)
            TogglePause();
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
        tutorialEnabled = startDocument.rootVisualElement.Q<Toggle>().value;
        startDocument.enabled = false;
        if (tutorialEnabled)
        {
            tutorialOver = false;
            CurrentState = GameState.Paused;
            StartCoroutine(StartTutorial());
        }
        else
            CurrentState = GameState.Running;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #region Tutorial

    private IEnumerator StartTutorial()
    {

        SendAllToBack();
        AddPopup(Screen.width / 2f, Screen.height / 2, step1Text);
        BringToFrontObject(GameObject.Find("Boss").GetComponentInChildren<SpriteRenderer>());
        yield return new WaitWhile(() => !step1);

        ResetObjectOrder();
        TintEverything();
        SendAllToBack();
        BringHealthForward();
        AddPopup(Screen.width / 2, Screen.height / 6, step2Text);
        Debug.Log("Heath Time");
        yield return new WaitWhile(() => !step2);

        BringAllToFront();
        ResetObjectOrder();
        CurrentState = GameState.Running;
        Debug.Log("Waiting for box to spawn");
        yield return new WaitWhile(() => !boxSpawned);

        CurrentState = GameState.Paused;

        SendAllToBack();
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
        AddPopup(Screen.width / 2.1f, Screen.height / 2.4f, step4Text);
        Debug.Log("Item time");
        yield return new WaitWhile(() => !step4);

        AddPopup(Screen.width / 2.1f, Screen.height / 2.4f, step4Text2);
        Debug.Log("Item time 2");
        yield return new WaitWhile(() => !step42);

        ResetObjectOrder();
        SendAllToBack();
        Debug.Log("Waiting for item to be sorted");
        yield return new WaitWhile(() => !itemSorted);

        SendAllToBack();
        ResetObjectOrder();
        BringPatienceForward();
        TintEverything();
        AddPopup(Screen.width / 9, Screen.height / 2, step5Text);
        Debug.Log("Patience Time");
        yield return new WaitWhile(() => !step5);

        AddPopup(Screen.width / 9, Screen.height / 2, step6Text);
        Debug.Log("Patience Time 2");
        yield return new WaitWhile(() => !step6);

        SendAllToBack();
        ResetObjectOrder();
        BringHoldForward();
        TintEverything();
        AddPopup(Screen.width / 1.85f , Screen.height / 2, step7Text);
        Debug.Log("Hold Time");
        yield return new WaitWhile(() => !step7);

        CurrentState = GameState.Running;

        ResetObjectOrder();
        BringAllToFront();
        Debug.Log("Waiting for box to empty");
        yield return new WaitWhile(() => !boxLeft);

        CurrentState = GameState.Paused;

        SendAllToBack();
        ResetObjectOrder();
        BringToFrontObject(conveyor.greenFade);
        AddPopup(Screen.width / 1.6f, Screen.height / 1.25f, step8Text);
        Debug.Log("Damage time");
        yield return new WaitWhile(() => !step8);

        SendAllToBack();
        ResetObjectOrder();
        TintEverything();
        AddPopup(Screen.width / 2.8f, Screen.height / 2f, step9Text);
        Debug.Log("Damage time");
        yield return new WaitWhile(() => !step9);

        ResetObjectOrder();
        BringAllToFront();

        tutorialOver = true;
    }

    private void BringToFrontObject(SpriteRenderer sr)
    {
        currentFocus = sr;
        originalOrder = sr.sortingOrder;
        sr.sortingOrder = 10;
        var childeren = sr.gameObject.GetComponentsInChildren<SpriteRenderer>();
        if (childeren.Length >= 2)
            childeren[1].sortingOrder = 11;
        tint.sortingOrder = 9;
    }

    private void TintEverything()
    {
        tint.sortingOrder = 11;
    }
    private void ResetObjectOrder()
    {
        if (currentFocus != null)
        {
            currentFocus.sortingOrder = originalOrder;
            var childeren = currentFocus.gameObject.GetComponentsInChildren<SpriteRenderer>();
            if (childeren.Length >= 2)
                childeren[1].sortingOrder = originalOrder + 1;
        }
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
        patienceArt.style.unityBackgroundImageTintColor = new Color(1, 1, 1, 1);
        patienceProgress.style.backgroundColor = new Color(0.9058824f, 0.9058824f, 0.9058824f, 1);
        patienceContainer.style.backgroundColor = new Color(0.7372549f, 0.7372549f, 0.7372549f, 1);
        healthIcon.style.unityBackgroundImageTintColor = new Color(1, 1, 1, 1);
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
        patienceProgress.style.backgroundColor = new Color(0.9058824f, 0.9058824f, 0.9058824f, 1);
        patienceContainer.style.backgroundColor = new Color(0.7372549f, 0.7372549f, 0.7372549f, 1);
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
        else if (!step42)
            step42 = true;
        else if (!step5)
            step5 = true;
        else if (!step6)
            step6 = true;
        else if (!step7)
            step7= true;
        else if (!step8)
            step8 = true;
        else if (!step9)
        {
            step9 = true;
            CurrentState = GameState.Running;
        }

        mainDocument.rootVisualElement.Remove(popUpElement);
        popUpElement = null;
    }
    #endregion
}
