using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

/// <summary>
/// This will be the parent class for all items the player has to sort.
/// </summary>

public class SortableItem : MonoBehaviour
{
    //refernces
    private PlayerController player;
    private GameController gameController;
    private HoldArea hold;

    //private fields
    [SerializeField]
    private List<string> attributes = new List<string>(); // List of all the attributes you can sort the item by
    private Vector2 screenBounds;
    private float objectWidth;
    private Rigidbody2D rb;
    private float gravityMax = -0.7f;
    private Vector2 lastMousePos = Vector2.zero;
    private bool thrown = false;
    private float thrownTimer = 1f;
    private float baseThrownTimer = 0.25f;
    private Box currentBox = null;
    public bool retrieved;
    public UnityEvent Released;


    // Properties
    public List<string> Attributes => attributes;

    void Start()
    {
        player = GameObject.Find("Controller").GetComponent<PlayerController>();
        gameController = GameObject.Find("Controller").GetComponent<GameController>();
        hold = GameObject.Find("UIDocument").GetComponent<HoldArea>();
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        rb = GetComponent<Rigidbody2D>();

        // Get the object's size based on its collider
        if (TryGetComponent<SpriteRenderer>(out SpriteRenderer sr)) objectWidth = sr.bounds.extents.x;
    }

    private void OnMouseDown()
    {
        if (gameController.CurrentState == GameState.Running || (gameController.CurrentState == GameState.Paused && gameController.tutorialEnabled && !gameController.itemSorted))
        {
            if (rb.bodyType == RigidbodyType2D.Static)
                rb.bodyType = RigidbodyType2D.Dynamic;

            player.HeldItem = this;

            // Disable gravity while dragging
            if (rb != null) rb.gravityScale = 0;

            GetComponent<CircleCollider2D>().radius /= 2f;
        }
    }

    void OnMouseDrag()
    {
        if (gameController.CurrentState == GameState.Running || (gameController.CurrentState == GameState.Paused && gameController.tutorialEnabled && !gameController.itemSorted))
            Dragging();
    }

    private void OnMouseUp()
    {
        if (gameController.CurrentState == GameState.Running || (gameController.CurrentState == GameState.Paused && gameController.tutorialEnabled && !gameController.itemSorted))
            StartCoroutine(ReleaseItem());
    }

    private void Dragging()
    {
        if (gameController.CurrentState == GameState.Running || (gameController.CurrentState == GameState.Paused && gameController.tutorialEnabled && !gameController.itemSorted))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            transform.position = new Vector2(mousePosition.x, mousePosition.y);
        }
    }

    private IEnumerator ReleaseItem()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseVelocity = (lastMousePos - mousePosition).normalized * (lastMousePos - mousePosition).magnitude * 50f;
        mouseVelocity = new Vector2(-mouseVelocity.x, -mouseVelocity.y * 1.5f);

        yield return new WaitForEndOfFrame();

        hold.ShowItem(null);
        player.HeldItem = null;
        player.recentItems.Add(this);
        // Restore gravity once released
        if (rb != null) rb.gravityScale = 1;

        Debug.Log(currentBox);
        if (currentBox != null)
            currentBox.SortItem(this);

        // Add mouse velocity to item to keep realistic and satisfying momentum
        if (rb.bodyType == RigidbodyType2D.Dynamic)
            rb.velocity += mouseVelocity;
        thrown = true;
        thrownTimer = baseThrownTimer;

        GetComponent<CircleCollider2D>().radius *= 2f;
        if (gameController.CurrentState == GameState.Paused)
            rb.bodyType = RigidbodyType2D.Static;
    }

    // Check for collisions with the box
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Box>(out Box box))
        {
            currentBox = box;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Box>(out Box box) && box == currentBox)
        {
            currentBox = null;
        }
    }

    public void RetrieveRelease()
    {
        retrieved = false;
        Released.RemoveAllListeners();
        StartCoroutine(ReleaseItem());
    }

    public void SetItemBody(RigidbodyType2D type)
    {
        rb.bodyType = type;
    }

    void FixedUpdate()
    {
        if (!thrown)
        {
            if (rb.velocity.y < gravityMax)
            {
                rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(rb.velocity.x, gravityMax), 0.1f);
            }
        }
    }

    void Update()
    {
        if (gameController.CurrentState == GameState.Paused)
            return;

        if (transform.position.y < -screenBounds.y - .5f) // Destroy when they exit the screen. .5f is half the height of items.
        {
            Destroy(this.gameObject);
        }

        // If grabbed from holding area, count as dragging.
        if (retrieved)
            Dragging();
        if (Input.GetMouseButtonUp(0) && retrieved)
            Released.Invoke();

        // Timer for deactivting thrown boolean
        // This is so that if the player throws downwards, it gains velocity still,
        // but if the player throws it up it doesn't come down with uncapped velocity.

        if (thrown)
        {
            thrownTimer -= Time.deltaTime;
            if (thrownTimer <= 0f)
            {
                thrown = false;
                player.recentItems.Remove(this);
            }
        }

        lastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnDestroy()
    {
        player.recentItems.Remove(this);
        if (GameObject.Find("Boss").GetComponent<Boss>().activeItem == this.gameObject)
            GameObject.Find("Boss").GetComponent<Boss>().activeItem = null;
        GameObject.Find("Boss").GetComponent<Boss>().activeItems.Remove(this.gameObject);
    }
}
