using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CellularAutomaion : MonoBehaviour
{

    public GameObject mainCamera; // Put main camera from the scene here
    public Vector2 gridSize = new Vector2(10, 10); // Try to put same numbers for X and Y like (50,50)
    public GameObject block;
    public GameObject emptyGameObject; // I use this for the empty spaces in the map
    public Transform parentTransform;

    [Range(3, 200)]
    public int SeedIntensity;

    public List<int> surviveNumbers;
    public List<int> bornNumbers;


    private bool[,] gridMap;
    private GameObject[,] gameObjectMap;

    private bool isTheJobDone = false;
    private float timer;
    private bool stopCheck = false;

    // Use this for initialization
    void Start()
    {
        // Setting up the camera position and view size depending on gridSize
        mainCamera.transform.position = new Vector3((gridSize.x / 2) - 1, mainCamera.transform.position.y, (gridSize.x / 2) - 1);
        mainCamera.GetComponent<Camera>().orthographicSize = gridSize.x / 2;

        // Initializing the gridMap and gridObjectMap depending on gridSize
        gridMap = new bool[(int)gridSize.x, (int)gridSize.y];
        gameObjectMap = new GameObject[(int)gridSize.x, (int)gridSize.y];

        // Creating the first seed
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                int rnd = Random.Range(1, SeedIntensity); // Change the the second number for changing the intensity of the first map
                if (rnd == 1) // Spawning the block
                {
                    gameObjectMap[i, j] = Instantiate(block, new Vector3(i, 0, j), block.transform.rotation);
                    gameObjectMap[i, j].transform.parent = parentTransform;
                    gridMap[i, j] = true;
                }
                else // Spawning the empty space
                {
                    gameObjectMap[i, j] = Instantiate(emptyGameObject, new Vector3(i, 0, j), block.transform.rotation);
                    gameObjectMap[i, j].transform.parent = parentTransform;
                    gridMap[i, j] = false;
                }
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        // Reseting the scene
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (stopCheck)
                stopCheck = false;
            else
                stopCheck = true;
        }

        // this is the timer for every step in the celular automation
        timer += Time.deltaTime;
        if (timer > 0.05f && !stopCheck)
        {
            NextStep();
            timer = 0;
        }

    }


    // This is the main function
    // it loops into every cell in gameObjectMap and checks condition for surviving or destroing the block
    void NextStep()
    {
        for (int j = 0; j < gridSize.x; j++)
        {
            for (int k = 0; k < gridSize.y; k++)
            {
                if (gridMap[j, k])
                {
                    if (!SurviveCheck(j, k))
                    {
                        Destroy(gameObjectMap[j, k]);
                        gameObjectMap[j, k] = Instantiate(emptyGameObject, new Vector3(j, 0, k), block.transform.rotation);
                        gameObjectMap[j, k].transform.parent = parentTransform;
                        gridMap[j, k] = false;
                    }
                }
                else
                {
                    if (BornCheck(j, k))
                    {
                        gameObjectMap[j, k] = Instantiate(block, new Vector3(j, 0, k), block.transform.rotation);
                        gameObjectMap[j, k].transform.parent = parentTransform;
                        gridMap[j, k] = true;
                    }
                }
            }
        }
    }

    // Checks if this cell should born or not
    bool BornCheck(int x, int y)
    {
        for (int i = 0; i < bornNumbers.Count; i++)
        {
            if (CheckThisCell(x, y) == bornNumbers[i])
            {
                return true;
            }
        }
        return false;
    }

    // Checks if this cell should survive or not
    bool SurviveCheck(int x, int y)
    {
        for (int i = 0; i < surviveNumbers.Count; i++)
        {
            if (CheckThisCell(x, y) == surviveNumbers[i])
            {
                return true;
            }
        }
        return false;
    }

    // Checks the srounding cell of this position in gridMap
    int CheckThisCell(int x, int y)
    {
        int count = 0;
        if (x > 1) { if (gridMap[x - 1, y]) count++; }
        if (y < gridSize.y - 1 && x > 1) { if (gridMap[x - 1, y + 1]) count++; }
        if (x < gridSize.x - 1) { if (gridMap[x + 1, y]) count++; }
        if (y < gridSize.y - 1) { if (gridMap[x, y + 1]) count++; }
        if (y < gridSize.y - 1 && x < gridSize.x - 1) { if (gridMap[x + 1, y + 1]) count++; }
        if (y > 1) { if (gridMap[x, y - 1]) count++; }
        if (y > 1 && x > 1) { if (gridMap[x - 1, y - 1]) count++; }
        if (y > 1 && x < gridSize.x - 1) { if (gridMap[x + 1, y - 1]) count++; }

        return count;
    }
}
