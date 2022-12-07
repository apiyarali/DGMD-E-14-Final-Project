using System.Collections.Generic;
using UnityEngine;

public class RandomGameObjectInstantiator : MonoBehaviour
{
    [Header("Prefab Set:")]
    [Tooltip("The set of game objects to select randomly from and instantiate")]
    public List<GameObject> ObjectsToSelectFrom = new List<GameObject>();
    public int NumberOfObjectsToCreate = 1;

    void Start()
    {
        if(ObjectsToSelectFrom.Count == 0)
        {
            return;
        }

        for(int i = 0; i < this.NumberOfObjectsToCreate; i++)
        {
            GameObject randomSelection = this.SelectRandomGameObject(this.ObjectsToSelectFrom);
            GameObject createdObject = Instantiate(randomSelection);
            createdObject.transform.parent = this.transform;
            createdObject.transform.position = this.transform.position;
        }
    }

    private GameObject SelectRandomGameObject(List<GameObject> gameObjectList)
    {
        if (gameObjectList == null || gameObjectList.Count == 0)
        {
            return null;
        }
        return gameObjectList[Random.Range(0, gameObjectList.Count)];
    }
}
