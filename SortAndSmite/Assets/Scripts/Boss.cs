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

    //fields
    private float spawnTimer = 3f;
    private float spawnBaseTime = 3f;
    private float health;

    void Start()
    {
        gameController = GameObject.Find("Controller").GetComponent<GameController>();
    }

    //Update spawn timer
    void Update()
    {
        if (gameController.CurrentState != GameState.Running)
            return;

        if (health <= 0)
            gameController.CurrentState = GameState.Victory;

        if (spawnTimer >= 0f)
        {
            spawnTimer = spawnBaseTime;
            SpawnItem();
        }
        else
            spawnTimer -= Time.deltaTime;
    }

    private void SpawnItem()
    {
        //Spawn item
    }

    private void TakeDamage(float dmg)
    {
        health -= dmg;

        //Play Animation?
    }
}
