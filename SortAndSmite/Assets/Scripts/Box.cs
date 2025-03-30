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
    [SerializeField]
    private string attribute;

    void Start()
    {
        //UNCOMMENT WHEN BOSS IS ADDED
        //boss = GameObject.Find("Boss").GetComponent<Boss>();
        player = GameObject.Find("Controller").GetComponent<PlayerController>();
        patience = GameObject.Find("UIDocument").GetComponent<PatienceBar>();
        boss = GameObject.Find("Boss").GetComponent<Boss>();
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

        bounds = gameObject.GetComponent<Collider2D>().bounds; //Update box bounds

        foreach (SortableItem item in items)
        {
            //if the item the player is dragging collides with this box, sort it/not, then destroy the object after.
            if (bounds.Intersects(item.gameObject.GetComponent<Collider2D>().bounds))
            {
                if (item.Attributes.Contains(attribute))
                {
                    itemsSorted++;
                    patience.AdjustPatience(7.5f);
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
    public void EmptyBox()
    {
        boss.TakeDamage(itemsSorted * 5f);

        //Place to play animation/sound/Popups
    }
}
