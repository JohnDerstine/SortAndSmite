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

    public List<GameObject> activeItems = new List<GameObject>();
 
    //fields
    private float spawnTimer = 5.0f;
    private float spawnBaseTime = 3f;
    private float health = 100;
    private float maxHealth = 100f;
    private GameObject spawnItem;
    private GameObject lastItem;
    private Vector2 spawnPoint;
    private const float spawnHeight = 5f;
    private Vector2 screenBounds;
    private ProgressBar healthBar;
    private Color originalColor;
    private Vector3 healthDefault;
    private int itemCount = 0;
    public GameObject activeItem;

    void Start()
    {
        //Attach components
        gameController = GameObject.Find("Controller").GetComponent<GameController>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        //Initialization
        originalColor = spriteRenderer.color;
        health = maxHealth;
        healthBar = doc.rootVisualElement.Q<VisualElement>("Center").Q<ProgressBar>("Health");
        healthDefault = healthBar.transform.position;

        //Get Screen Size
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
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
        do
        {
            if (gameController.tutorialEnabled && itemCount == 0)
                spawnItem = items[3];
            else
                spawnItem = items[Random.Range(0, items.Count)]; //Select random item from list of spawnable items
        }
        while (spawnItem == lastItem);
        lastItem = spawnItem;

        //Randmize x value of item spawn, within a certain range based on screen size
        //If tutorial enabled, spawn it in middle
        if (gameController.tutorialEnabled && itemCount == 0)
            spawnPoint = new Vector2(0, spawnHeight);
        else
            spawnPoint = new Vector2(Random.Range(-screenBounds.x + (screenBounds.x / 2f), screenBounds.x - (screenBounds.x / 2f)), spawnHeight);
        

        activeItem = Instantiate(spawnItem, spawnPoint, Quaternion.identity);
        if (gameController.tutorialEnabled && itemCount == 0)
            activeItem.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        itemCount++;
        activeItems.Add(activeItem);
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
        spawnBaseTime -= 0.075f;

        //Play Animation
        if (animator != null)
            animator.SetTrigger("hit"); //Play hit animation if animator is present
        //StartCoroutine(FlashColor());
        StartCoroutine(ShakeHealthBar());
        ShowDamageText(dmg);

    }

    //Shake the healthbar randomly over time after taking damage
    private IEnumerator ShakeHealthBar()
    {
        float shakeTimer = 0.35f;
        while (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            Vector3 randVector = new Vector3();
            randVector.x = Random.Range(-15, 15);
            randVector.y = Random.Range(-15, 15);
            healthBar.transform.position = Vector3.Lerp(healthBar.transform.position, randVector, 0.75f);
            yield return null;
        }
        healthBar.transform.position = healthDefault;
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
            textBox.color = Color.red;
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
