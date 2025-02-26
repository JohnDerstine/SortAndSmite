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
        boss = GameObject.Find("Boss").GetComponent<Boss>();
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

    //Check the player held item, and recently thrown items for collisions
    void Update()
    {
        List<SortableItem> items = new List<SortableItem>();

        if (player.HeldItem == null && player.recentItems.Count == 0)
            return;
        else if (player.recentItems.Count != 0)
        {
            foreach (SortableItem item in player.recentItems)
                items.Add(item);
        }
        else
            items.Add(player.HeldItem);

        foreach (SortableItem item in items)
        {
            //if the item the player is dragging collides with this box, sort it/not, then destroy the object after.
            if (bounds.Intersects(item.gameObject.GetComponent<Collider2D>().bounds))
            {
                if (item.Attributes.Contains(attribute))
                {
                    itemsSorted++;
                    patience.AdjustPatience(7.5f);
                    boss.TakeDamage(5); //temporary
                }
                else
                    patience.AdjustPatience(-10);
                if (player.recentItems.Contains(item))
                    player.recentItems.Remove(item);
                else
                    player.HeldItem = null;
                Destroy(item.gameObject);
            }
        }
    }

    //When Conveyor belt is added, hook this up for when the box disapears off-screen.
    void EmptyBox()
    {
        boss.TakeDamage(itemsSorted);
        Destroy(gameObject);
    }
}
