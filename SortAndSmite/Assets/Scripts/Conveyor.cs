using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    //References
    [SerializeField]
    GameController gameController;

    [SerializeField]
    public SpriteRenderer greenFade;

    //fields
    [SerializeField]
    private List<GameObject> boxPrefabs = new List<GameObject>();

    private List<GameObject> activeBoxes = new List<GameObject>();

    private float boxSpawnTimer = 1f;
    private float baseTimer = 7f;

    public SpriteRenderer activeBox;
    private GameObject lastBox;
    private GameObject lastLastBox;

    Vector2 boxSpawnPoint = new Vector2(-10.4f, -3);
    Vector2 boxDespawnPoint = new Vector2(10.5f, -2.8f);
    float ConverySpeed = 0.9f;

    void Update()
    {
        if (gameController.CurrentState != GameState.Running)
            return;

        boxSpawnTimer -= Time.deltaTime;
        if (boxSpawnTimer <= 0)
        {
            boxSpawnTimer = baseTimer;
            SpawnBox();
        }

        //Move boxes
        for (int i = 0; i < activeBoxes.Count; i++)
        {
            GameObject box = activeBoxes[i];
            box.transform.position = new Vector3(box.transform.position.x + (ConverySpeed * Time.deltaTime), box.transform.position.y, 0);

            //Delete boxes that leave the screen
            if (box.transform.position.x > boxDespawnPoint.x)
            {
                activeBoxes.Remove(box);
                box.GetComponent<Box>().EmptyBox();
                Destroy(box);
                i--;
                greenFade.color = Color.white;
                gameController.boxLeft = true;
            }
        }

        Color transparent = greenFade.color;
        transparent.a -= Time.deltaTime;
        greenFade.color = transparent;

    }

    //Spawn new box at the beginning of the conveyor belt
    private void SpawnBox()
    {
        if (!gameController.boxSpawned)
            StartCoroutine(WaitForConveyor());

        //Make sure the box isn't the same as the last.
        GameObject boxToSpawn;
        do
        {
            if (gameController.tutorialEnabled && !gameController.boxSpawned)
                boxToSpawn = boxPrefabs[3];
            else
                boxToSpawn = boxPrefabs[Random.Range(0, boxPrefabs.Count)];
        }
        while (boxToSpawn == lastBox || boxToSpawn == lastLastBox);
        lastLastBox = lastBox;
        lastBox = boxToSpawn;

        GameObject box = Instantiate(boxToSpawn, boxSpawnPoint, Quaternion.identity);
        activeBox = box.GetComponent<SpriteRenderer>();
        activeBoxes.Add(box);
    }

    private IEnumerator WaitForConveyor()
    {
        yield return new WaitForSeconds(4);
        gameController.boxSpawned = true;
    }
}
