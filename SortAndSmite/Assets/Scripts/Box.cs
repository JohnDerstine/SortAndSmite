using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    //references
    private PlayerController player;
    private Boss boss;
    private PatienceBar patience;

    //fields
    private Bounds bounds;
    private int itemsSorted = 0;
    private string attribute;
    private List<string> placeHolderAttributes = new List<string>() { "red", "blue", "yellow", "green" };
    private SpriteRenderer sRenderer;

    void Start()
    {
        //UNCOMMENT WHEN BOSS IS ADDED
        //boss = GameObject.Find("Boss").GetComponent<Boss>();
        player = GameObject.Find("Controller").GetComponent<PlayerController>();
        patience = GameObject.Find("UIDocument").GetComponent<PatienceBar>();
        bounds = gameObject.GetComponent<Collider2D>().bounds;
        sRenderer = gameObject.GetComponent<SpriteRenderer>();
        attribute = placeHolderAttributes[Random.Range(0, placeHolderAttributes.Count)];

        //Temporary way to distinguish box type
        switch (attribute)
        {
            case "red":
                sRenderer.color = Color.red;
                break;
            case "blue":
                sRenderer.color = Color.blue;
                break;
            case "yellow":
                sRenderer.color = Color.yellow;
                break;
            case "green":
                sRenderer.color = Color.green;
                break;
        }
    }

    
    void Update()
    {
        if (player.HeldItem == null)
            return;

        //if the item the player is dragging collides with this box, sort it/not, then destroy the object after.
        if (bounds.Intersects(player.HeldItem.gameObject.GetComponent<Collider2D>().bounds))
        {
            if (player.HeldItem.Attributes.Contains(attribute))
            {
                itemsSorted++;
                patience.AdjustPatience(5);
            }
            else
                patience.AdjustPatience(-10);
            Destroy(player.HeldItem.gameObject);
            player.HeldItem = null;
        }
    }

    //When Conveyor belt is added, hook this up for when the box disapears off-screen.
    void EmptyBox()
    {
        boss.TakeDamage(itemsSorted);
        Destroy(gameObject);
    }
}
