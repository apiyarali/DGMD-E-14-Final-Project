using UnityEngine;

/// <summary>
/// This component script stores values related to placing objects within the game.
/// </summary>
public class PlaceableObject : MonoBehaviour
{
    [Header("XZ-Plane Radius in Unity Units")]
    [Tooltip("The object's radius on the level's XZ-plane in Unity Units.")]
    public float XZRadiusInUnityUnits;
}
