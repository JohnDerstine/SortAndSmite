using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

/// <summary>
/// This could be a parent class if we decide that boss mosnters have different ways/patterns
/// in spawning items.
/// </summary>

public class Boss : MonoBehaviour
{
    //references
    GameController gameController;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private UIDocument doc;
    [SerializeField]
    private List<GameObject> items = new List<GameObject>();
    [SerializeField]
    private GameObject damageTextPrefab;
    [SerializeField]
    private Canvas canvas;

    //fields
    private float spawnTimer = 1.5f;
    private float spawnBaseTime = 1.5f;
    private float health = 100;
    private float maxHealth = 100f;
    private GameObject spawnItem;
    private Vector2 spawnPoint;
    private const float spawnHeight = 5f;
    private Vector2 screenBounds;
    private ProgressBar healthBar;
    private Color originalColor;

    void Start()
    {
        //Attach components
        gameController = GameObject.Find("Controller").GetComponent<GameController>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        //Initialization
        originalColor = spriteRenderer.color;
        health = maxHealth;
        spawnTimer = spawnBaseTime;
        healthBar = doc.rootVisualElement.Q<VisualElement>("right").Q<ProgressBar>("Health");

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
        if (dmg <= 0)
            return;

        health -= dmg;
        healthBar.value = health;
        if (healthBar.value < 0)
            healthBar.value = 0;

        //increase spawn rate
        spawnBaseTime -= 0.07f;

        //Play Animation
        if (animator != null)
            animator.SetTrigger("hit"); //Play hit animation if animator is present
        StartCoroutine(FlashColor());
        ShowDamageText(dmg);

    }

    private IEnumerator FlashColor()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(.2f);
        spriteRenderer.color = originalColor;
    }

    private void ShowDamageText(float dmg)
    {
        if (damageTextPrefab != null) 
        {
            GameObject dmgText = Instantiate(damageTextPrefab, canvas.gameObject.transform, false);
            TextMeshProUGUI textBox = dmgText.GetComponent<TextMeshProUGUI>();
            textBox.text = dmg.ToString();
            textBox.fontSize = 1;
            Destroy(dmgText, 1f);
        }
    }

    private void Die()
    {
        if (animator != null)
            animator.SetTrigger("die"); //Death animation

        Destroy(gameObject, 2f); //Wait 2 seconds before destroying boss
    }
}
