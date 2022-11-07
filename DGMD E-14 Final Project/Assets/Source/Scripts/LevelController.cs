using Assets.Source;
using System;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private bool isLevelGenerated;

    // Properties related the floor of the level
    public Transform FloorTransform = null;
    public Transform DangerFloorTransform = null;
    private bool floorIsGenerated;

    // TODO: Ceiling and walls
    public decimal MinLevelWidth = 4.0m;
    public decimal MaxLevelWidth = 16.0m;
    public decimal MinLevelLength = 8.0m;
    public decimal MaxLevelLength = 24.0m;
    private Tuple<int, int> levelDimensions;
    private float levelSizeScaleFactor = 0.25f;
    private int[,] levelData;

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
        levelData = new int[levelDimensions.Item1, levelDimensions.Item2];
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

        isLevelGenerated = true;
    }
}

