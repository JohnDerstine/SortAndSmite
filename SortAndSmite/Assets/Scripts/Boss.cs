using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This could be a parent class if we decide that boss mosnters have different ways/patterns
/// in spawning items.
/// </summary>

public class Boss : MonoBehaviour
{
    //references
    GameController gameController;
    private Animator animator;

    [SerializeField]
    private List<GameObject> items = new List<GameObject>();

    //fields
    private float spawnTimer = 0.25f;
    private float spawnBaseTime = 0.25f;
    private float health = 100;
    private float maxHealth = 100f;
    private GameObject spawnItem;
    private Vector2 spawnPoint;
    private const float spawnHeight = 5f;
    private Vector2 screenBounds;

    void Start()
    {
        //Attach components
        gameController = GameObject.Find("Controller").GetComponent<GameController>();
        animator = GetComponent<Animator>();

        //Initialization
        health = maxHealth;
        spawnTimer = spawnBaseTime;

        //Get Screen Size
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        Debug.Log(screenBounds.x);
    }

    //Update spawn timer
    void Update()
    {
        if (gameController.CurrentState != GameState.Running)
            return;

        if (health <= 0)
        {
            gameController.CurrentState = GameState.Victory;
            Die();
            return;
        }

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            spawnTimer = spawnBaseTime;
            SpawnItem();
        }
    }

    private void SpawnItem()
    {
        spawnItem = items[Random.Range(0, items.Count)]; //Select random item from list of spawnable items

        //Randmize x value of item spawn, within a certain range based on screen size
        spawnPoint = new Vector2(Random.Range(-screenBounds.x + (screenBounds.x / 2f), screenBounds.x - (screenBounds.x / 2f)), spawnHeight);
        Instantiate(spawnItem, spawnPoint, Quaternion.identity);
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;

        //Play Animation
        if (animator != null)
            animator.SetTrigger("hit"); //Play hit animation if animator is present

    }

    private void Die()
    {
        if (animator != null)
            animator.SetTrigger("die"); //Death animation

        Destroy(gameObject, 2f); //Wait 2 seconds before destroying boss
    }
}
