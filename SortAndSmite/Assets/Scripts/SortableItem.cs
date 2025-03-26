using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This will be the parent class for all items the player has to sort.
/// </summary>

public class SortableItem : MonoBehaviour
{
    //refernces
    private PlayerController player;

    //private fields
    [SerializeField]
    private List<string> attributes = new List<string>(); //List of all the attributes you can sort the item by
    private Vector2 screenBounds;
    private float objectWidth;
    private Rigidbody2D rb;
    private float gravityMax = -1f;
    private Vector2 lastMousePos = Vector2.zero;
    private bool thrown;
    private float thrownTimer = 1f;
    private float baseThrownTimer = 0.25f;

    public bool retrieved;
    public UnityEvent Released;

    //properties
    public List<string> Attributes
    {
        get { return attributes; }
    }

    void Start()
    {
        player = GameObject.Find("Controller").GetComponent<PlayerController>();
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        rb = GetComponent<Rigidbody2D>();

        // Get the object's size based on its collider
        if (TryGetComponent<SpriteRenderer>(out SpriteRenderer sr))
            objectWidth = sr.bounds.extents.x;
    }

    private void OnMouseDown()
    {
        player.HeldItem = this;

        // Disable gravity while dragging
        if (rb != null)
            rb.gravityScale = 0;

        GetComponent<CircleCollider2D>().radius /= 2f;
    }

    void OnMouseDrag()
    {
        Dragging();
    }

    private void OnMouseUp()
    {
        StartCoroutine(ReleaseItem());
    }

    private void Dragging()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Clamp x position to prevent dragging outside edges
        float clampedX = Mathf.Clamp(mousePosition.x, -screenBounds.x + objectWidth, screenBounds.x - objectWidth);

        transform.position = new Vector3(clampedX, mousePosition.y, 0f);
    }

    private IEnumerator ReleaseItem()
    {
        yield return new WaitForEndOfFrame();
        player.HeldItem = null;
        player.recentItems.Add(this);
        // Restore gravity once released
        if (rb != null)
            rb.gravityScale = 1;

        //Add mouse velocity to item to keep realistic and satisfying momentum
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseVelocity = (lastMousePos - mousePosition).normalized * (lastMousePos - mousePosition).magnitude * 50f;
        mouseVelocity = new Vector2(-mouseVelocity.x, -mouseVelocity.y * 1.5f);
        rb.velocity += mouseVelocity;
        thrown = true;
        thrownTimer = baseThrownTimer;

        GetComponent<CircleCollider2D>().radius *= 2f;
    }

    public void RetrieveRelease()
    {
        retrieved = false;
        Released.RemoveAllListeners();
        StartCoroutine(ReleaseItem());
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
        if (transform.position.y < -screenBounds.y - .5f) //Destroy when they exit the screen. .5f is half the height of items.
            Destroy(this.gameObject);

        //If grabbed from holding area, count as dragging.
        if (retrieved)
            Dragging();
        if (Input.GetMouseButtonUp(0) && retrieved)
            Released.Invoke();

        //Timer for deactivting thrown boolean
        //This is so that if the player throws downwards, it gains velocity still,
        //but if the player throws it up it doesn't come down with uncapped velocity.

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
    }
}
