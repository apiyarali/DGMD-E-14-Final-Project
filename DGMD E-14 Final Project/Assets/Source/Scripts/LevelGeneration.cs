using Assets.Source;
using System;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    // Properties related the floor of the level
    public Transform FloorTransform = null;
    public Transform DangerFloorTransform = null;
    private bool floorIsGenerated = false;

    // TODO: Ceiling and walls
    public decimal MinLevelWidth = 1.0m;
    public decimal MaxLevelWidth = 4.0m;
    public decimal MinLevelLength = 2.0m;
    public decimal MaxLevelLength = 4.0m;
    private Tuple<int, int> levelDimensions;
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
        if(!floorIsGenerated && FloorTransform != null)
        {
            // TODO: Generate the floor with tiles
            // Change Floor Size:
            FloorTransform.localScale = new Vector3(levelDimensions.Item1, 1.0f, levelDimensions.Item2);
            DangerFloorTransform.localScale = new Vector3(levelDimensions.Item1 * 2.0f, 1.0f, levelDimensions.Item2 * 2.0f);
        }
    }
}
