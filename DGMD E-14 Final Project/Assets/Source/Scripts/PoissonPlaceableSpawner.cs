using Assets.Source;
using System.Collections.Generic;
using UnityEngine;

public class PoissonPlaceableSpawner : MonoBehaviour
{
    public List<GameObject> ObjectsToSpawn;
    public Vector3 SpawnPlaneOffset = new(0, 0, 0);
    public Vector2Int XZSpawnPlaneSize = new(-1, -1);
    private int spawnIndex = 0;
    public int CandiatePointAttempts = 100;
    private int[,] levelData;
    private List<Vector2> spawnPoints = new();

    private bool levelIsFull;
    private bool spawningIsReady;
    // Start is called before the first frame update
    void Start()
    {
        levelIsFull = false;
        if (XZSpawnPlaneSize.x < 0 && XZSpawnPlaneSize.y < 0)
        {
            spawningIsReady = false;
        }
        else
        {
            levelData = new int[(int)XZSpawnPlaneSize.x, (int)XZSpawnPlaneSize.y];
            BlockLevelMiddleFromSpawns();
            spawningIsReady = true;
        }

        if (ObjectsToSpawn != null)
        {
            Debug.Log("ObjectsToSpawn count: " + ObjectsToSpawn.Count);
        }
    }

    public void SetSpawnPlaneSize(Vector2Int size)
    {
        XZSpawnPlaneSize = size;
        levelData = new int[XZSpawnPlaneSize.x, XZSpawnPlaneSize.y];
        Debug.Log("Spawn plane size set: (" + XZSpawnPlaneSize.x + ", " + XZSpawnPlaneSize.y + ")");
        levelData = new int[XZSpawnPlaneSize.x, XZSpawnPlaneSize.y];
        spawningIsReady = true;
        BlockLevelMiddleFromSpawns();
    }

    private void BlockLevelMiddleFromSpawns()
    {
        if (levelData == null)
        {
            Debug.LogError("level is null");
            return;
        }
        if (XZSpawnPlaneSize.x <= 0 || XZSpawnPlaneSize.y <= 0)
        {
            Debug.LogError("level spawn plane size is invalid");
            return;
        }


        int centerX = XZSpawnPlaneSize.x / 2;
        int centerZ = XZSpawnPlaneSize.y / 2;
        Debug.Log("Blocking center of level from spawns at: (" + centerX + ", " + centerZ + ").");
        levelData[centerX, centerZ] = 1;
        for (int x = centerX - 2; x < centerX + 2; x++)
        {
            for (int z = centerZ - 2; z < centerZ + 2; z++)
            {
                if (x >= 0 && z >= 0 && x < XZSpawnPlaneSize.x && z < XZSpawnPlaneSize.y)
                {
                    levelData[x, z] = 1;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (levelIsFull)
        {
            return;
        }

        if (!spawningIsReady)
        {
            return;
        }

        // Check if the sampling plane is ready for objects to spawn:
        if (!IsPlaneReadyToSpawn())
        {
            return;
        }

        // TODO: Spawn plane tiles one time only if flagged to make a plane.

        // Check if enough time has passed between the last spawn:
        if (!IsItTimeToSpawn(Time.deltaTime))
        {
            return;
        }

        int numberOfTries = 0;
        while (numberOfTries < CandiatePointAttempts)
        {
            // TODO: Use previous spawn as spawn points and search randomly within their radius:

            int xPositionCandidate = RandomNumbers.GetRandomIntegerInclusive(1, XZSpawnPlaneSize.x - 1);
            int yPositionCandidate = RandomNumbers.GetRandomIntegerInclusive(1, XZSpawnPlaneSize.y - 1);
            Debug.Log(levelData);
            if (levelData[xPositionCandidate, yPositionCandidate] == 0)
            {
                // TODO: Check for any other objects within the radius of this spawn point
                if (ObjectsToSpawn[spawnIndex] != null)
                {
                    int randomRotation = Random.Range(0, 4) * 90;
                    Instantiate(ObjectsToSpawn[spawnIndex],
                        new Vector3(this.gameObject.transform.position.x - (XZSpawnPlaneSize.x / 2) + SpawnPlaneOffset.x + xPositionCandidate,
                        this.gameObject.transform.position.y + SpawnPlaneOffset.y,
                        this.gameObject.transform.position.z - (XZSpawnPlaneSize.y / 2) + SpawnPlaneOffset.z + yPositionCandidate), // Added to Z here because objects are only spawned on the XZ plane.
                        Quaternion.Euler(new Vector3(0, randomRotation, 0)));
                    levelData[xPositionCandidate, yPositionCandidate] = 1;

                    // TODO: SPAWN THE FIRST OBJECT AT THE CENTER OF THE ROOM AND THEN SAMPLE FROM THERE
                    // todo: poisson disc sampling
                    // todo: add object as spawn point
                    int radius = GetPlaceableRadius(ObjectsToSpawn[spawnIndex]);

                    // Block tiles around the spawn point based on the radius:
                    for (int x = xPositionCandidate - radius; x < xPositionCandidate + radius; x++)
                    {
                        for (int z = yPositionCandidate - radius; z < yPositionCandidate + radius; z++)
                        {
                            if (x >= 0 && x < XZSpawnPlaneSize.x && z >= 0 && z < XZSpawnPlaneSize.y)
                            {
                                levelData[x, z] = radius;
                            }
                        }
                    }

                    break;
                }
            }
            numberOfTries++;
        }

        // TODO: use the spawn points to tell if the level is full when a spawn has failed at every point:
        // If the number of tries to place an object failed, then the level is full:
        if (numberOfTries >= CandiatePointAttempts)
        {
            levelIsFull = true;
            Debug.Log("The spawn plane is full.");
        }
        else
        {
            // Increment the spawn index so that the next object spawns next time:
            spawnIndex += 1;
            if (spawnIndex >= ObjectsToSpawn.Count)
            {
                spawnIndex = 0;
            }
            PrintRemainingPlaneCapacity();
        }
    }

    private bool IsPlaneReadyToSpawn()
    {
        if (spawnIndex < ObjectsToSpawn.Count && XZSpawnPlaneSize.x > 0 && XZSpawnPlaneSize.y > 0)
        {
            return true;
        }
        return false;
    }

    public float SecondsBetweenSpawns = 0.25f;
    private float secondsSinceLastSpawn = 0;
    private bool IsItTimeToSpawn(float deltaTimeInSeconds)
    {
        secondsSinceLastSpawn += deltaTimeInSeconds;
        if (secondsSinceLastSpawn > SecondsBetweenSpawns)
        {
            SecondsBetweenSpawns -= secondsSinceLastSpawn;
            return true;
        }
        return false;
    }

    private int GetPlaceableRadius(GameObject placeable)
    {
        // TODO: Make this default value a public parameter for the user to set.
        int radius = 2;
        PlaceableObject placeableObject = placeable.GetComponent(typeof(PlaceableObject)) as PlaceableObject;

        if (placeableObject != null)
        {
            radius = Mathf.RoundToInt(placeableObject.XZRadiusInUnityUnits + 0.5f);
            if (radius == 0)
            {
                radius = 1;
            }
            return radius;
        }

        Debug.LogError("Radius not found on placeable object.");
        return radius;
    }

    private void PrintRemainingPlaneCapacity()
    {
        int emptyCells = 0;
        for(int i = 0; i < XZSpawnPlaneSize.x; i++)
        {
            for (int j = 0; j < XZSpawnPlaneSize.y; j++)
            {
                if(levelData[i, j] == 0)
                {
                    emptyCells++;
                }
            }
        }
        Debug.Log("Spawn Plane Capacity Remaining: " + emptyCells + "/ " + (XZSpawnPlaneSize.x * XZSpawnPlaneSize.y));
    }
}
