using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    //References
    [SerializeField]
    GameController gameController;

    //fields
    [SerializeField]
    private List<GameObject> boxPrefabs = new List<GameObject>();

    private List<GameObject> activeBoxes = new List<GameObject>();

    private float boxSpawnTimer = 1f;
    private float baseTimer = 5f;

    private GameObject lastBox;

    Vector2 boxSpawnPoint = new Vector2(-8, -2.8f);
    Vector2 boxDespawnPoint = new Vector2(10.5f, -2.8f);
    float ConverySpeed = 1.5f;

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
            }
        }

    }

    //Spawn new box at the beginning of the conveyor belt
    private void SpawnBox()
    {
        //Make sure the box isn't the same as the last.
        GameObject boxToSpawn;
        do
        {
            boxToSpawn = boxPrefabs[Random.Range(0, boxPrefabs.Count)];
        }
        while (lastBox == boxToSpawn);
        lastBox = boxToSpawn;

        GameObject box = Instantiate(boxToSpawn, boxSpawnPoint, Quaternion.identity);
        activeBoxes.Add(box);
    }
}
