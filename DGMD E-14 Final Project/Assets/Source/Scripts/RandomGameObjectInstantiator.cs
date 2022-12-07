using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script component selects and spawns a random game object from a list.
/// </summary>
public class RandomGameObjectInstantiator : MonoBehaviour
{
    [Header("Game Object Set")]
    [Tooltip("The set of game objects (prefabs) to select randomly from and instantiate.")]
    public List<GameObject> ObjectsToSelectFrom = new List<GameObject>();


    [Header("Number of Objects to Create")]
    [Tooltip("The number of times to randomly select an item and create it. Note that each iteration will be a new object.")]
    public int NumberOfObjectsToCreate = 1;

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        if (ObjectsToSelectFrom.Count == 0)
        {
            return;
        }

        for (int i = 0; i < this.NumberOfObjectsToCreate; i++)
        {
            GameObject randomSelection = this.SelectRandomGameObject(this.ObjectsToSelectFrom);
            if (randomSelection != null)
            {
                GameObject createdObject = Instantiate(randomSelection);
                createdObject.transform.parent = this.transform;
                createdObject.transform.position = this.transform.position;
            }
        }
    }

    /// <summary>
    /// Selects a random GameObject from the given list.
    /// </summary>
    /// <param name="gameObjectList">The list of GameObjects.</param>
    /// <returns>Returns a GameObject from the list, if succesful, else returns null.</returns>
    private GameObject SelectRandomGameObject(List<GameObject> gameObjectList)
    {
        if (gameObjectList == null || gameObjectList.Count == 0)
        {
            return null;
        }
        return gameObjectList[Random.Range(0, gameObjectList.Count)];
    }
}
