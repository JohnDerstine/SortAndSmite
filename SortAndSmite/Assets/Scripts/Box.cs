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
    private int itemsSorted = 0;
    [SerializeField]
    private string attribute;

    void Start()
    {
        player = GameObject.Find("Controller").GetComponent<PlayerController>();
        patience = GameObject.Find("UIDocument").GetComponent<PatienceBar>();
        boss = GameObject.Find("Boss").GetComponent<Boss>();
    }

    //Check the player held item, and recently thrown items for collisions
    public void SortItem(SortableItem item)
    {
        if (item.Attributes.Contains(attribute))
        {
            itemsSorted++;
            patience.AdjustPatience(7.5f);
        }
        else
        {
            patience.AdjustPatience(-10);
        }

        if (player.recentItems.Contains(item))
            player.recentItems.Remove(item);

        Destroy(item.gameObject);
    }

    //Empty the box to cause damage to the boss
    public void EmptyBox()
    {
        boss.TakeDamage(itemsSorted * 5f);
    }
}
