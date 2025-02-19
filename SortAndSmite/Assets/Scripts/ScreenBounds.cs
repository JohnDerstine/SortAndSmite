using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBounds : MonoBehaviour
{
    void Start()
    {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        CreateBoundary("LeftBoundary", new Vector2(-screenBounds.x, 0));
        CreateBoundary("RightBoundary", new Vector2(screenBounds.x, 0));
    }

    void CreateBoundary(string name, Vector2 position)
    {
        GameObject boundary = new GameObject(name);
        boundary.transform.position = position;

        BoxCollider2D collider = boundary.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.1f, Screen.height * 2);
        collider.isTrigger = false;
    }
}

