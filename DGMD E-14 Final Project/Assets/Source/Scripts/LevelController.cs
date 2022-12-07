using Assets.Source;
using System;
using UnityEngine;

/// <summary>
/// A script which creates the objects within the game's level.
/// </summary>
public class LevelController : MonoBehaviour
{
    /// <summary>
    /// The state of whether or not the level has been generated.
    /// </summary>
    private bool isLevelGenerated;

    /// <summary>
    /// The transform of the floor which will be positioned according to randomized level parameters.
    /// </summary>
    public Transform FloorTransform = null;

    /// <summary>
    /// The transform of the 'danger' floor which can kill the player if they fall through the world to it.
    /// </summary>
    public Transform DangerFloorTransform = null;

    /// <summary>
    /// The state of whether or not the floor has been generated.
    /// </summary>
    private bool floorIsGenerated;

    /// <summary>
    /// The transform of one of the four walls of the level.
    /// </summary>
    public Transform WallA = null;

    /// <summary>
    /// The transform of one of the four walls of the level.
    /// </summary>
    public Transform WallB = null;

    /// <summary>
    /// The transform of one of the four walls of the level.
    /// </summary>
    public Transform WallC = null;

    /// <summary>
    /// The transform of one of the four walls of the level.
    /// </summary>
    public Transform WallD = null;

    /// <summary>
    /// The state of whether or not the walls have been generated.
    /// </summary>
    private bool wallsAreGenerated;

    /// <summary>
    /// The minimum width of the level.
    /// </summary>
    public decimal MinLevelWidth = 8.0m;

    /// <summary>
    /// The maximum width of the level.
    /// </summary>
    public decimal MaxLevelWidth = 8.0m;

    /// <summary>
    /// The maximum length of the level.
    /// </summary>
    public decimal MinLevelLength = 8.0m;

    /// <summary>
    /// The maximum length of the level.
    /// </summary>
    public decimal MaxLevelLength = 8.0m;

    /// <summary>
    /// The dimensions of the square level.
    /// </summary>
    private Tuple<int, int> levelDimensions;

    /// <summary>
    /// The scale factor of the level to convert its dimensions to world space.
    /// </summary>
    private float levelSizeScaleFactor = 0.25f;

    /// <summary>
    /// The disc sampler for placing objects into the level.
    /// </summary>
    public PoissonPlaceableSpawner placeableObjectSpawner;

    /// <summary>
    /// A scalar to multiply the distance of the walls from normal level size.
    /// </summary>
    public float WallPositionScalar = 5.0f;

    /// <summary>
    /// A scalar to convert the floor plane size to Unity units.
    /// </summary>
    public float floorPlaneConversionFactorToUnityUnits = 2.0f;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        // Determines the Size of the Level:
        decimal levelWidth = RandomNumbers.GetRandomDecimalInclusive(MinLevelWidth, MaxLevelWidth);
        decimal levelHeight = RandomNumbers.GetRandomDecimalInclusive(MinLevelLength, MaxLevelLength);
        levelDimensions = Tuple.Create((int)Math.Floor(levelWidth), (int)Math.Floor(levelHeight));
        Debug.Log("Level size is " + levelDimensions.Item1 + " by " + levelDimensions.Item2);

    }

    /// <summary>
    /// FixedUpdate is called before each internal physics update.
    /// </summary>
    void FixedUpdate()
    {
        if(!isLevelGenerated)
        {
            GenerateLevel();
        }
    }

    /// <summary>
    /// Sets the level state such that it will be regenerated at next update.
    /// </summary>
    public void RegenerateLevel()
    {
        isLevelGenerated = false;
        floorIsGenerated = false;
        wallsAreGenerated = false;
    }

    /// <summary>
    /// Generates the floor, walls, and level objects.
    /// </summary>
    private void GenerateLevel()
    {
        if (!floorIsGenerated && FloorTransform != null)
        {
            // Change Floor Size:
            float levelFloorWidth = levelDimensions.Item1 * levelSizeScaleFactor;
            float levelFloorHeight = levelDimensions.Item2 * levelSizeScaleFactor;
            FloorTransform.localScale = new Vector3(levelFloorWidth, 1.0f, levelFloorHeight);
            DangerFloorTransform.localScale = new Vector3(levelFloorWidth * 2.0f, 1.0f, levelFloorHeight * 2.0f);
            floorIsGenerated = true;
        }

        if(!wallsAreGenerated)
        {
            if (WallA != null)
            {
                WallA.localPosition = new Vector3(levelDimensions.Item1 * levelSizeScaleFactor * WallPositionScalar + 0.5f, WallA.localPosition.y, WallA.localPosition.z);
            }
            if (WallB != null)
            {
                WallB.localPosition = new Vector3(-levelDimensions.Item1 * levelSizeScaleFactor * WallPositionScalar - 0.5f, WallB.localPosition.y, WallB.localPosition.z);
            }
            if (WallC != null)
            {
                WallC.localPosition = new Vector3(WallC.localPosition.x, WallC.localPosition.y, levelDimensions.Item2 * levelSizeScaleFactor * WallPositionScalar + 0.5f);
            }
            if (WallD != null)
            {
                WallD.localPosition = new Vector3(WallD.localPosition.x, WallD.localPosition.y, -levelDimensions.Item2 * levelSizeScaleFactor * WallPositionScalar - 0.5f);
            }
            wallsAreGenerated = true;
        }

        // Set the object spawner parameters:
        if(placeableObjectSpawner != null)
        {
            placeableObjectSpawner.SetSpawnPlaneSize(new Vector2Int((int)((levelDimensions.Item1 + 1) * floorPlaneConversionFactorToUnityUnits), (int)((levelDimensions.Item2 + 1) * floorPlaneConversionFactorToUnityUnits)));
        }

        isLevelGenerated = true;
    }
}

