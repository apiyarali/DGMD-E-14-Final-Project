using Assets.Source;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component script uses Poisson Disc Sampling to spawn objects on a specified plane.
/// </summary>
public class PoissonPlaceableSpawner : MonoBehaviour
{
    [Header("Essential Objects to Place")]
    [Tooltip("The set of objects which must be placed.")]
    public List<GameObject> EssentialObjectsToSpawn;

    [Header("How Many of Each Essential Object to Place")]
    [Tooltip("The number of each essential object to spawn on the plane.")]
    public int HowManyOfEachEssentialObjectToSpawn = 1;

    [Header("Non-Essential Objects to Place")]
    [Tooltip("The set of objects will be placed to fill the remaining space on the plane.")]
    public List<GameObject> NonEssentialObjectsToSpawn;

    [Header("Spawn Plane Offset")]
    [Tooltip("The offset of the spawn plane from the entity's position.")]
    public Vector3 SpawnPlaneOffset = new Vector3(0, 0, 0);

    [Header("Spawn Plane Size")]
    [Tooltip("The size of the spawn plane.")]
    public Vector2Int XZSpawnPlaneSize = new Vector2Int(-1, -1);

    [Header("Spawn Plane Blocked Center Radius")]
    [Tooltip("The size of the blocked center area in the spawn plane to allow for predetermined object placement in the scene.")]
    public int RadiusOfUnspawnableCenter = 3;

    [Header("Spawn Point Candidate Attempts")]
    [Tooltip("The number of attempts to make in placing objects before concluding that the spawn plane is full.")]
    public int SpawnPointCandiateAttempts = 100;

    [Header("Seconds Between Spawns")]
    [Tooltip("The number of seconds between each object spawn.")]
    public float SecondsBetweenSpawns = 0.25f;

    /// <summary>
    /// The number of seconds since the last object spawn.
    /// </summary>
    private float secondsSinceLastSpawn = 0;

    /// <summary>
    /// A 2D array storying the data of the spawn plane.
    /// </summary>
    private int[,] spawnPlaneData;

    /// <summary>
    /// Whether or not the spawn plane is full.
    /// </summary>
    private bool spawnPlaneIsFull;

    /// <summary>
    /// Whether or not the spawn plane is ready to place objects on it.
    /// </summary>
    private bool spawningIsReady;

    /// <summary>
    /// The current index into the essential objects to spawn list.
    /// </summary>
    private int essentialSpawnIndex = 0;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        spawnPlaneIsFull = false;
        if (XZSpawnPlaneSize.x < 0 && XZSpawnPlaneSize.y < 0)
        {
            spawningIsReady = false;
        }
        else
        {
            spawnPlaneData = new int[(int)XZSpawnPlaneSize.x, (int)XZSpawnPlaneSize.y];
            BlockCenterOfPlaneFromSpawns();
            spawningIsReady = true;
        }

        if (EssentialObjectsToSpawn != null)
        {
            Debug.Log("ObjectsToSpawn count: " + EssentialObjectsToSpawn.Count);
        }
    }

    /// <summary>
    /// Sets the spawn plane size.
    /// </summary>
    /// <param name="size">The size of the plane to spawn objects on.</param>
    public void SetSpawnPlaneSize(Vector2Int size)
    {
        XZSpawnPlaneSize = size;
        spawnPlaneData = new int[XZSpawnPlaneSize.x, XZSpawnPlaneSize.y];
        Debug.Log("Spawn plane size set: (" + XZSpawnPlaneSize.x + ", " + XZSpawnPlaneSize.y + ")");
        spawnPlaneData = new int[XZSpawnPlaneSize.x, XZSpawnPlaneSize.y];
        spawningIsReady = true;
        BlockCenterOfPlaneFromSpawns();
    }

    /// <summary>
    /// Blocks the center of the spawn plane to prevent objects from being placed there.
    /// </summary>
    private void BlockCenterOfPlaneFromSpawns()
    {
        if (spawnPlaneData == null)
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
        spawnPlaneData[centerX, centerZ] = 1;
        for (int x = centerX - RadiusOfUnspawnableCenter; x < centerX + RadiusOfUnspawnableCenter; x++)
        {
            for (int z = centerZ - RadiusOfUnspawnableCenter; z < centerZ + RadiusOfUnspawnableCenter; z++)
            {
                if (x >= 0 && z >= 0 && x < XZSpawnPlaneSize.x && z < XZSpawnPlaneSize.y)
                {
                    spawnPlaneData[x, z] = 1;
                }
            }
        }
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        if (spawnPlaneIsFull)
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

        // Check if enough time has passed between the last spawn:
        if (!IsItTimeToSpawn(Time.deltaTime))
        {
            return;
        }

        // Spawn Essential Objects:
        while(essentialSpawnIndex < this.EssentialObjectsToSpawn.Count)
        {
            for(int i = 0; i < this.HowManyOfEachEssentialObjectToSpawn; i++)
            {
                if (!this.TrySpawnPlaceableObject(this.EssentialObjectsToSpawn[essentialSpawnIndex]))
                {
                    // Return if another object is unable to be spawned.
                    Debug.LogWarning("Not all essential placeable objects may have been spawned.");
                    return;
                }
                
            }
            essentialSpawnIndex++;
        }

        // Spawn Non-Essential Objects:
        bool successfullSpawnedLastObject = true;
        while (successfullSpawnedLastObject)
        {
            int indexOfRandomObjectToSpawn = Random.Range(0, this.NonEssentialObjectsToSpawn.Count - 1);
            successfullSpawnedLastObject = this.TrySpawnPlaceableObject(this.NonEssentialObjectsToSpawn[indexOfRandomObjectToSpawn]);
            PrintRemainingPlaneCapacity();
        }
    }

    /// <summary>
    /// Attempts the place an object on the spawn plane.
    /// </summary>
    /// <param name="placeableObject">The object to place on the plane.</param>
    /// <returns>Returns true if an object is spawned, else returns false.</returns>
    private bool TrySpawnPlaceableObject(GameObject placeableObject)
    {
        if (placeableObject == null)
        {
            Debug.LogError("Placeable object is null and cannot be spawned.");
            return false;
        }

        if (spawnPlaneIsFull)
        {
            return false;
        }

        int numberOfTries = 0;
        while (numberOfTries < SpawnPointCandiateAttempts)
        {
            int xPositionCandidate = RandomNumbers.GetRandomIntegerInclusive(0, XZSpawnPlaneSize.x - 1);
            int yPositionCandidate = RandomNumbers.GetRandomIntegerInclusive(0, XZSpawnPlaneSize.y - 1);
            if (spawnPlaneData[xPositionCandidate, yPositionCandidate] == 0)
            {
                int randomRotation = Random.Range(0, 4) * 90;
                GameObject placedObject = Instantiate(placeableObject,
                    new Vector3(this.gameObject.transform.position.x - (XZSpawnPlaneSize.x / 2) + SpawnPlaneOffset.x + xPositionCandidate,
                    this.gameObject.transform.position.y + SpawnPlaneOffset.y,
                    this.gameObject.transform.position.z - (XZSpawnPlaneSize.y / 2) + SpawnPlaneOffset.z + yPositionCandidate), // Added to Z here because objects are only spawned on the XZ plane.
                    Quaternion.Euler(new Vector3(0, randomRotation, 0)));
                spawnPlaneData[xPositionCandidate, yPositionCandidate] = 1;
                placedObject.transform.parent = this.transform;
                int radius = GetPlaceableRadius(placeableObject);

                // Block tiles around the spawn point based on the radius:
                for (int x = xPositionCandidate - radius; x < xPositionCandidate + radius; x++)
                {
                    for (int z = yPositionCandidate - radius; z < yPositionCandidate + radius; z++)
                    {
                        if (x >= 0 && x < XZSpawnPlaneSize.x && z >= 0 && z < XZSpawnPlaneSize.y)
                        {
                            spawnPlaneData[x, z] = radius;
                        }
                    }
                }

                break;
            }
            numberOfTries++;
        }

        // If the number of tries to place an object failed, then the level is full:
        if (numberOfTries >= SpawnPointCandiateAttempts)
        {
            spawnPlaneIsFull = true;
            Debug.Log("The spawn plane is full.");
            return false;
        }

        PrintRemainingPlaneCapacity();
        return true;
    }

    /// <summary>
    /// Determines whether or not the spawn plane is ready for object placement.
    /// </summary>
    /// <returns>Returns true if the plane is ready, else returns false.</returns>
    private bool IsPlaneReadyToSpawn()
    {
        if (this.EssentialObjectsToSpawn.Count != 0 && this.NonEssentialObjectsToSpawn.Count != 0 && XZSpawnPlaneSize.x > 0 && XZSpawnPlaneSize.y > 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Determines whether or not enough time has passed since the last spawn so that not all spawns happen at once.
    /// </summary>
    /// <param name="deltaTimeInSeconds">The time since last check.</param>
    /// <returns>Returns true if enough time has passed, else returns false.</returns>
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

    /// <summary>
    /// Gets the radius data from a placeable object using it's script component.
    /// </summary>
    /// <param name="placeable">The placeable object to check for radius data.</param>
    /// <returns>Returns the radius value, if found, else returns 2 as default.</returns>
    private int GetPlaceableRadius(GameObject placeable)
    {
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

    /// <summary>
    /// Prints the remaining space in the spawn plane as a fraction of the total spawn plane space.
    /// </summary>
    private void PrintRemainingPlaneCapacity()
    {
        int emptyCells = 0;
        for (int i = 0; i < XZSpawnPlaneSize.x; i++)
        {
            for (int j = 0; j < XZSpawnPlaneSize.y; j++)
            {
                if (spawnPlaneData[i, j] == 0)
                {
                    emptyCells++;
                }
            }
        }
        Debug.Log("Spawn Plane Capacity Remaining: " + emptyCells + "/ " + (XZSpawnPlaneSize.x * XZSpawnPlaneSize.y));
    }
}
