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
    private Vector3 offset;
    private Camera cam;

    //properties
    public List<string> Attributes
    {
        get { return attributes; }
    }

    void Start()
    {
        cam = Camera.main;
        player = GameObject.Find("Controller").GetComponent<PlayerController>();
    }

    private void OnMouseDown()
    {
        //Calculate the offset between mouse position and offset position
        offset = transform.position - GetMouseWorldPos();
        player.HeldItem = this;
    }

    void OnMouseDrag()
    {
        //Move object to follow mouse
        transform.position = GetMouseWorldPos() + offset;
    }

    private void OnMouseUp()
    {
        player.HeldItem = null;
    }

    void Update()
    {

    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = cam.WorldToScreenPoint(transform.position).z; //Maintain object's Z position
        return cam.ScreenToWorldPoint(mousePoint);
    }
}
