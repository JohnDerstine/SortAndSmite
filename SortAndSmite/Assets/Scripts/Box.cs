using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    //references
    private PlayerController player;
    private Boss boss;
    private PatienceBar patience;

    //animation
    private SpriteRenderer spriteRenderer;
    private Sprite[] boxOpeningFrames;
    private Sprite[] boxClosingFrames;
    private Coroutine currentAnimation;

    //fields
    private int itemsSorted = 0;
    [SerializeField]
    private string attribute;
    [SerializeField] 
    private float frameDelay = 0.05f;

    void Start()
    {
        player = GameObject.Find("Controller").GetComponent<PlayerController>();
        patience = GameObject.Find("UIDocument").GetComponent<PatienceBar>();
        boss = GameObject.Find("Boss").GetComponent<Boss>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("HeldItem"))
        {
            if (currentAnimation != null)
                StopCoroutine(currentAnimation);
            currentAnimation = StartCoroutine(PlayAnimation(boxOpeningFrames));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("HeldItem"))
        {
            if (currentAnimation != null)
                StopCoroutine(currentAnimation);
            currentAnimation = StartCoroutine(PlayAnimation(boxClosingFrames));
        }
    }

    private IEnumerator PlayAnimation(Sprite[] frames)
    {
        for (int i = 0; i < frames.Length; i++)
        {
            spriteRenderer.sprite = frames[i];
            yield return new WaitForSeconds(frameDelay);
        }
    }
}
