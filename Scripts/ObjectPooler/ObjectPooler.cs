using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler instance;

    [Tooltip("This array will determine how the pool is built")]
    [SerializeField] private PoolableObject[] poolableObjects;

    [Tooltip("Enable this to make this a global object that is kept across scenes. Disabling this is useful if you want each scene to have its own object pool.")]
    [SerializeField] private bool dontDestroyOnLoad;

    [Tooltip("Enabling this will allow the object pooler to instantiate additional objects into the pool if the respective queue is empty, preventing errors.")]
    [SerializeField] private bool enablePadding;

    [Tooltip("Enabling this will allow the object pooler to dynamically create a pool in the event that a requested poolable object entry was not created at Start. Not recommended to use outside testing.")]
    [SerializeField] private bool unknownEntryHandling;


    private Dictionary<PoolableObject, Queue<GameObject>> objectPool;   //Dictionary that holds all the queues of poolable objects

    //Garbage reducing reusable references
    private GameObject objCache;

    private void Awake()
    {
        Singleton();
    }

    private void Start()
    {
        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);
        BuildObjectPool();
    }

    #region Singleton
    private void Singleton()
    {
        if (instance == null)
            instance = this;
        else
        {
            Debug.Log("More than one instance of ObjectPooler exists, destryoing newest one.");
            Destroy(this);          //Destroy the class, not the gameobject
        }
    }
    #endregion

    #region Pool Building
    private void BuildObjectPool()
    {
        //Create the dictionary object
        objectPool = new Dictionary<PoolableObject, Queue<GameObject>>();

        //Loop through the poolable object array and create an entry in the pool for each
        for (int i = 0; i < poolableObjects.Length; i++)
        {
            //Create a new Queue collection for the current entry
            objectPool.Add(poolableObjects[i], new Queue<GameObject>());

            //For the number specified in the poolable object, instansiate the applicable prefab
            for (int j = 0; j < poolableObjects[i].pooledAmount; j++)
            {
                AddEntry(poolableObjects[i]);
            }
        }
    }

    //The method that adds the poolobject to the appropriate queue within the dictionary
    private void AddEntry(PoolableObject pooledObject)
    {
        //Instansiate a new gameobject at scene origin
        objCache = Instantiate(pooledObject.poolablePrefab, Vector3.zero, Quaternion.identity);

        //Disable the object
        objCache.SetActive(false);

        //Add to the appropriate queue within the pool dictionary
        objectPool[pooledObject].Enqueue(objCache);
    }

    #endregion

    #region Spawning From Pool
    /// <summary>
    /// Spawns an object from the desired object pool.
    /// </summary>
    /// <param name="pooledObject"></param>
    /// <returns></returns>
    public static GameObject SpawnObject(PoolableObject pooledObject)
    {
        //If the static instance is null don't do anything and pop an error
        if(instance == null)
        {
            Debug.LogError("No Instance of ObjectPool exists");
            return null;
        }

        //Ensure the key exists first
        if(instance.objectPool.ContainsKey(pooledObject))
        {

            if(instance.objectPool[pooledObject].Count < 1)
            {
                //If the queue was empty, but paddding was enabled, go ahead and add an additional entry
                if (instance.enablePadding)
                {
                    //Instansiate at scene origin
                    instance.AddEntry(pooledObject);
                }
                else
                {
                    Debug.LogError($"Queue empty for {pooledObject.poolableName}. Consider increasing pool size");
                    return null;
                }
            }
            //Dequeue an object
            instance.objCache = instance.objectPool[pooledObject].Dequeue();
            //Enable/activate the object
            instance.objCache.SetActive(true);
            return instance.objCache;
        }
        else
        {
            //If there wasn't a valid entry and unknown entry handling has been enabled, make an entry
            if(instance.unknownEntryHandling)
            {
                instance.objectPool.Add(pooledObject, new Queue<GameObject>());

                //For the number specified in the poolable object, instansiate the applicable prefab
                for (int i = 0; i < pooledObject.pooledAmount; i++)
                {
                    instance.AddEntry(pooledObject);
                }
                instance.objCache = instance.objectPool[pooledObject].Dequeue();
                instance.objCache.SetActive(true);
                return instance.objCache;
            }
            //If there wasn't a valid entry and unknown entry handling has been disabled, just pop an error
            else
            {
                Debug.LogError($"No entry exists for '{pooledObject.poolableName}' in the object pool");
                return null;
            }
        }
    }
    /// <summary>
    /// Spawns an object from the desired object pool and sets the transform
    /// </summary>
    /// <param name="pooledObject"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static GameObject SpawnObject(PoolableObject pooledObject, Vector3 position, Quaternion rotation)
    {
        //Use the base method to return an object
        instance.objCache = SpawnObject(pooledObject);

        //Apply the postion and rotation to the transform
        instance.objCache.transform.position = position;
        instance.objCache.transform.rotation = rotation;

        //returned the transformed object
        return instance.objCache;
    }
    /// <summary>
    /// Spawns an object from the desired object pool and sets the transform and returns the component of Type type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="pooledObject"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static T SpawnObject<T>(PoolableObject pooledObject, Vector3 position, Quaternion rotation)
    {
        return SpawnObject(pooledObject, position, rotation).GetComponent<T>();
    }
    #endregion

    #region Returning To Pool
    /// <summary>
    /// Returns the gameobject to the specified pool
    /// </summary>
    /// <param name="poolable"></param>
    /// <param name="obj"></param>
    public static void ReturnToPool(PoolableObject poolable, GameObject obj)
    {
        if (instance.objectPool.ContainsKey(poolable))
        {
            instance.objectPool[poolable].Enqueue(obj);
            obj.SetActive(false);
        }
        else
        {
            Debug.LogError($"No entry exists for '{poolable.poolableName}' in the object pool");
        }
    }
    #endregion
}
