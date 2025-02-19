using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private Camera cam;
    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;
    private Rigidbody2D rb;

    //properties
    public List<string> Attributes
    {
        get { return attributes; }
    }

    void Start()
    {
        cam = Camera.main;
        player = GameObject.Find("Controller").GetComponent<PlayerController>();
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        rb = GetComponent<Rigidbody2D>();

        // Get the object's size based on its collider
        if (TryGetComponent<SpriteRenderer>(out SpriteRenderer sr))
        {
            objectWidth = sr.bounds.extents.x;
            objectHeight = sr.bounds.extents.y;
        }
    }

    private void OnMouseDown()
    {
        player.HeldItem = this;

        // Disable gravity while dragging
        if (rb != null)
            rb.gravityScale = 0;
    }

    void OnMouseDrag()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Clamp x position to prevent dragging outside edges
        float clampedX = Mathf.Clamp(mousePosition.x, -screenBounds.x + objectWidth, screenBounds.x - objectWidth);

        transform.position = new Vector3(clampedX, mousePosition.y, 0f);
    }

    private void OnMouseUp()
    {
        player.HeldItem = null;
        // Restore gravity once released
        if (rb != null)
            rb.gravityScale = 1;
    }

    void Update()
    {

    }
}
