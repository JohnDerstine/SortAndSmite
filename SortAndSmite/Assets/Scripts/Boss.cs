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

    //fields
    private float spawnTimer = 3f;
    private float spawnBaseTime = 3f;
    private float health = 100;
    private float maxHealth = 100f;
    public GameObject spawnItem;
    public Transform spawnPoint;

    void Start()
    {
        //Attach components
        gameController = GameObject.Find("Controller").GetComponent<GameController>();
        animator = GetComponent<Animator>();

        //Initialization
        health = maxHealth;
        spawnTimer = spawnBaseTime;
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
        //Spawn item
        if(spawnItem != null && spawnPoint != null)
        {
            Instantiate(spawnItem, spawnPoint.position, Quaternion.identity);
        }
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
