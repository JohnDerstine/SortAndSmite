using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This will be the parent class for all items the player has to sort.
/// </summary>

public class SortableItem : MonoBehaviour
{
    //private fields
    private List<string> attributes = new List<string>(); //List of all the attributes you can sort the item by
    private Vector3 offset;
    private Camera cam;
    
    //public fields
    public Renderer rend;

    //properties
    public List<string> Attributes
    {
        get { return attributes; }
    }

    void Start()
    {
        rend = GetComponent<Renderer>();
        cam = Camera.main;
    }

    private void OnMouseDown()
    {
        //Calculate the offset between mouse position and offset position
        offset = transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        //Move object to follow mouse
        transform.position = GetMouseWorldPos() + offset;
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
