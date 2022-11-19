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

    private bool spawningIsReady;
    // Start is called before the first frame update
    void Start()
    {
        if (XZSpawnPlaneSize.x < 0 && XZSpawnPlaneSize.y < 0)
        {
            spawningIsReady = false;
        }
        else
        {
            levelData = new int[(int)XZSpawnPlaneSize.x, (int)XZSpawnPlaneSize.y];
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
    }

    // Update is called once per frame
    void Update()
    {
        if (!spawningIsReady)
        {
            return;
        }

        // Check if the sampling plane is ready for objects to spawn:
        if (!IsPlaneReadyToSpawn())
        {
            return;
        } 

        // Check if enough time has passed between the last spawn:
        if (!IsItTimeToSpawn(Time.deltaTime))
        {
            return;
        }

        int numberOfTries = 0;
        while (numberOfTries < CandiatePointAttempts)
        {
            int xPositionCandidate = RandomNumbers.GetRandomIntegerInclusive(0, XZSpawnPlaneSize.x - 1);
            int yPositionCandidate = RandomNumbers.GetRandomIntegerInclusive(0, XZSpawnPlaneSize.y - 1);
            Debug.Log(levelData);
            if (levelData[xPositionCandidate, yPositionCandidate] != 1)
            {
                // TODO: have the prefab orientation randomly rotated by 90 degree intervals
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
                    // TODO: fill in the level data based on spawned object radius
                    // todo: poisson disc sampling
                    // todo: add object as spawn point

                    // TEMP: Block 1 tile around each spawned object.
                    if (xPositionCandidate - 1 >= 0)
                    {
                        levelData[xPositionCandidate - 1, yPositionCandidate] = 1;
                    }
                    if (xPositionCandidate + 1 < XZSpawnPlaneSize.x)
                    {
                        levelData[xPositionCandidate + 1, yPositionCandidate] = 1;
                    }
                    if (yPositionCandidate - 1 >= 0)
                    {
                        levelData[xPositionCandidate, yPositionCandidate - 1] = 1;
                    }
                    if (yPositionCandidate < XZSpawnPlaneSize.y)
                    {
                        levelData[xPositionCandidate, yPositionCandidate + 1] = 1;
                    }

                    break;
                }
            }
            numberOfTries++;
        }

        // Increment the spawn index so that the next object spawns next time:
        spawnIndex += 1;
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
}
