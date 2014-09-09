using UnityEngine;
using System;
using System.Collections;

/**
 * A general pool object for reusable game objects.
 * 
 * It supports spawning and unspawning game objects that are
 * instantiated from a common prefab. Can be used preallocate
 * objects to avoid calls to Instantiate during gameplay. Can
 * also create objects on demand (which it does if no objects
 * are available in the pool).
 * 
 * Converted JScript version to C#, original here:
 * http://vonlehecreative.com/video-games/unity-resource-gameobjectpool/
 * 
 * C# version by Bart Wttewaall, www.Mediamonkey.nl
 */

public class GameObjectPool : MonoBehaviour
{

    private GameObject _prefab;
    private Stack available;
    private ArrayList all;

    private Action<GameObject> initAction;

    // ---- getters & setters ----
    #region getters & setters

    // returns the prefab being used by the pool.
    public GameObject prefab
    {
        get { return _prefab; }
    }

    // returns the number of active objects.
    public int numActive
    {
        get { return all.Count - available.Count; }
    }

    // returns the number of available objects.
    public int numAvailable
    {
        get { return available.Count; }
    }

    #endregion
    // ---- constructor ----
    #region constructor

    public GameObjectPool(GameObject prefab, uint initialCapacity, Action<GameObject> initAction)
    {
        this._prefab = prefab;
        this.initAction = initAction;

        available = (initialCapacity > 0) ? new Stack((int)initialCapacity) : new Stack();
        all = (initialCapacity > 0) ? new ArrayList((int)initialCapacity) : new ArrayList();
    }

    #endregion
    // ---- public methods ----
    #region public methods

    public GameObject Spawn(Vector3 position, Quaternion rotation)
    {
        GameObject result;

        if (available.Count == 0)
        {

            // create an object and initialize it.
            result = GameObject.Instantiate(prefab, position, rotation) as GameObject;

            // run optional initialization method on the object
            if (initAction != null) initAction(result);

            all.Add(result);

        }
        else
        {
            result = available.Pop() as GameObject;

            // get the result's transform and reuse for efficiency.
            // calling gameObject.transform is expensive.
            Transform resultTrans = result.transform;
            resultTrans.position = position;
            resultTrans.rotation = rotation;

            SetActive(result, true);
        }

        return result;
    }

    public bool Destroy(GameObject target)
    {
        if (!available.Contains(target))
        {
            available.Push(target);

            SetActive(target, false);
            return true;
        }

        return false;
    }

    // Unspawns all the game objects created by the pool.
    public void DestroyAll()
    {
        for (int i = 0; i < all.Count; i++)
        {
            GameObject target = all[i] as GameObject;

            if (target.activeInHierarchy) Destroy(target);
        }
    }

    // Unspawns all the game objects and clears the pool.
    public void Clear()
    {
        DestroyAll();
        available.Clear();
        all.Clear();
    }

    // Applies the provided function to some or all of the pool's game objects.
    public void ForEach(Action<GameObject> action, bool activeOnly)
    {
        for (int i = 0; i < all.Count; i++)
        {
            GameObject target = all[i] as GameObject;

            if (!activeOnly || target.activeInHierarchy) action(target);
        }
    }

    #endregion
    // ---- protected methods ----
    #region protected methods

    // Activates or deactivates the provided game object using the method
    protected void SetActive(GameObject target, bool value)
    {
        target.SetActive(value);
    }
    #endregion
}

