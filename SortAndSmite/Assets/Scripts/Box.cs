using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    //references
    private PlayerController player;
    private Boss boss;
    private PatienceBar patience;
    private GameController controller;

    //animation
    private SpriteRenderer spriteRenderer;
    public Sprite[] boxOpeningFrames;
    public Sprite[] boxClosingFrames;
    private Coroutine currentAnimation;
    private bool hasPlayedEnterAnimation = false;
    private bool hasPlayedExitAnimation = false;
    private bool isVisibleToCamera = false;
    private float animationStartPause = 1.5f;
    private float animationEndPause = 1.5f;

    private bool open;

    //fields
    private int itemsSorted = 0;
    [SerializeField]
    private string attribute;
    [SerializeField] 
    private float frameDelay = 0.05f;
    [SerializeField, Range(0.1f, 5f)]
    private float animationSpeedMultiplier = 1.5f;

    void Start()
    {
        player = GameObject.Find("Controller").GetComponent<PlayerController>();
        controller = GameObject.Find("Controller").GetComponent<GameController>();
        patience = GameObject.Find("UIDocument").GetComponent<PatienceBar>();
        boss = GameObject.Find("Boss").GetComponent<Boss>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        bool currentlyVisible = viewportPos.x > -.2f && viewportPos.x < 1.2f && viewportPos.y > 0 && viewportPos.y < 1 && viewportPos.z > 0;

        if (currentlyVisible && !isVisibleToCamera)
        {
            isVisibleToCamera = true;
            if (!hasPlayedEnterAnimation)
            {
                hasPlayedEnterAnimation = true;
                hasPlayedExitAnimation = false;
                if (currentAnimation != null)
                    StopCoroutine(currentAnimation);
                currentAnimation = StartCoroutine(PlayAnimation(boxOpeningFrames));
            }
        }
        else if (!currentlyVisible && isVisibleToCamera)
        {
            isVisibleToCamera = false;
            if (!hasPlayedExitAnimation)
            {
                hasPlayedExitAnimation = true;
                hasPlayedEnterAnimation = false;
                if (currentAnimation != null)
                    StopCoroutine(currentAnimation);
                currentAnimation = StartCoroutine(PlayAnimation(boxClosingFrames));
            }
        }
    }

    //Check the player held item, and recently thrown items for collisions
    public void SortItem(SortableItem item)
    {
        Debug.Log("tried");

        if (controller.CurrentState == GameState.Paused && (controller.itemSorted || !controller.tutorialEnabled))
            return;

        if (!open)
            return;

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

        if (!controller.itemSorted)
        {
            controller.itemSorted = true;
            controller.gameplayStarted = true;
        }

        //if (currentAnimation != null)
        //    StopCoroutine(currentAnimation);
        //currentAnimation = StartCoroutine(PlayAnimation(boxClosingFrames));
    }

    //Empty the box to cause damage to the boss
    public void EmptyBox()
    {
        boss.TakeDamage(itemsSorted * 5f);
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.GetComponent<SortableItem>() != null && other.GetComponent<SortableItem>() == player.HeldItem)
    //    {
    //        if (currentAnimation != null)
    //            StopCoroutine(currentAnimation);
    //        currentAnimation = StartCoroutine(PlayAnimation(boxOpeningFrames));
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    if (other.GetComponent<SortableItem>() != null && other.GetComponent<SortableItem>() == player.HeldItem)
    //    {
    //        if (currentAnimation != null)
    //            StopCoroutine(currentAnimation);
    //        currentAnimation = StartCoroutine(PlayAnimation(boxClosingFrames));
    //    }
    //}

    private IEnumerator PlayAnimation(Sprite[] frames)
    {
        open = !open;
        Debug.Log("Playing animation");
        float delay = frameDelay / animationSpeedMultiplier;
        yield return new WaitForSeconds(animationStartPause);
        for (int i = 0; i < frames.Length; i++)
        {
            spriteRenderer.sprite = frames[i];
            yield return new WaitForSeconds(delay);
        }
        yield return new WaitForSeconds(animationEndPause);
    }
}
