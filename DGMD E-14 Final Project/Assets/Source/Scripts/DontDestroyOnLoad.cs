using UnityEngine;

/// <summary>
/// A class which prevents the entity it is attached to from being destroyed when the game or scene is reloaded allowing data to be preserved between loads.
/// </summary>
public class DontDestroyOnLoad : MonoBehaviour
{
    /// <summary>
    /// And instance of the component if one is assigned via editor.
    /// </summary>
    public static DontDestroyOnLoad instance = null;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
