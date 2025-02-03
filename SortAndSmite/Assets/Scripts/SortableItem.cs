using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This will be the parent class for all items the player has to sort.
/// </summary>

public class SortableItem : MonoBehaviour
{
    //fields
    private List<string> attributes = new List<string>(); //List of all the attributes you can sort the item by

    //properties
    public List<string> Attributes
    {
        get { return attributes; }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
