using Assets.Source;
using System;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private bool isLevelGenerated;

    // Properties related the floor, walls, and ceiling of the level
    public Transform FloorTransform = null;
    public Transform DangerFloorTransform = null;
    private bool floorIsGenerated;
    public Transform WallA = null;
    public Transform WallB = null;
    public Transform WallC = null;
    public Transform WallD = null;
    private bool wallsAreGenerated;

    // TODO: Ceiling and walls
    [SerializeField]
    public decimal MinLevelWidth = 8.0m;
    [SerializeField]
    public decimal MaxLevelWidth = 8.0m;
    [SerializeField]
    public decimal MinLevelLength = 8.0m;
    [SerializeField]
    public decimal MaxLevelLength = 8.0m;
    private Tuple<int, int> levelDimensions;
    private float levelSizeScaleFactor = 0.25f;

    public PoissonPlaceableSpawner placeableObjectSpawner;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: Find the floor, if null.
        // TODO: Find the danger floor, if null.

        // Determines the Size of the Level:
        // TODO: validate the min/max level size inputs.
        decimal levelWidth = RandomNumbers.GetRandomDecimalInclusive(MinLevelWidth, MaxLevelWidth);
        decimal levelHeight = RandomNumbers.GetRandomDecimalInclusive(MinLevelLength, MaxLevelLength);
        levelDimensions = Tuple.Create((int)Math.Floor(levelWidth), (int)Math.Floor(levelHeight));
        Debug.Log("Level size is " + levelDimensions.Item1 + " by " + levelDimensions.Item2);

    }

    void FixedUpdate()
    {
        if(!isLevelGenerated)
        {
            GenerateLevel();
        }
    }

    public void RegenerateLevel()
    {
        isLevelGenerated = false;
        floorIsGenerated = false;
        wallsAreGenerated = false;
    }

    private void GenerateLevel()
    {
        if (!floorIsGenerated && FloorTransform != null)
        {
            // TODO: Generate the floor with tiles
            // Change Floor Size:
            float levelFloorWidth = levelDimensions.Item1 * levelSizeScaleFactor;
            float levelFloorHeight = levelDimensions.Item2 * levelSizeScaleFactor;
            FloorTransform.localScale = new Vector3(levelFloorWidth, 1.0f, levelFloorHeight);
            DangerFloorTransform.localScale = new Vector3(levelFloorWidth * 2.0f, 1.0f, levelFloorHeight * 2.0f);
            floorIsGenerated = true;
        }

        // TODO: Remove this hard-coded value
        float wallPositionScalar = 5.0f;
        if(!wallsAreGenerated)
        {
            if (WallA != null)
            {
                WallA.localPosition = new Vector3(levelDimensions.Item1 * levelSizeScaleFactor * wallPositionScalar + 0.5f, WallA.localPosition.y, WallA.localPosition.z);
            }
            if (WallB != null)
            {
                WallB.localPosition = new Vector3(-levelDimensions.Item1 * levelSizeScaleFactor * wallPositionScalar - 0.5f, WallB.localPosition.y, WallB.localPosition.z);
            }
            if (WallC != null)
            {
                WallC.localPosition = new Vector3(WallC.localPosition.x, WallC.localPosition.y, levelDimensions.Item2 * levelSizeScaleFactor * wallPositionScalar + 0.5f);
            }
            if (WallD != null)
            {
                WallD.localPosition = new Vector3(WallD.localPosition.x, WallD.localPosition.y, -levelDimensions.Item2 * levelSizeScaleFactor * wallPositionScalar - 0.5f);
            }
            wallsAreGenerated = true;
        }

        // Set the object spawner parameters:
        if(placeableObjectSpawner != null)
        {
            // TODO: convert plane size to Unity units
            float conversionFactor = 2.0f;
            placeableObjectSpawner.SetSpawnPlaneSize(new Vector2Int((int)((levelDimensions.Item1 + 1) * conversionFactor), (int)((levelDimensions.Item2 + 1) * conversionFactor)));
        }

        isLevelGenerated = true;
    }
}

