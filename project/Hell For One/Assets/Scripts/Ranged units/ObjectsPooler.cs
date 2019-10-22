using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsPooler : MonoBehaviour
{

    [Tooltip("The object to duplicate in the pooler")]
    public GameObject originalObject;

    [Tooltip("The amount of objects in the pooler. At the start, the pooler is fill with this number of objects.")]
    [Min(1)]
    public int amountInPooler;

    [Tooltip("The number of objects added to the pooler if there is no objects available.")]
    [Min(1)]
    public int incrementAmount;

    private List<GameObject> pooler;

    // Start is called before the first frame update
    void Start()
    {
        pooler = new List<GameObject>();

        for (int i = 0; i < amountInPooler; i++)
        {
            addNewObject();
        }
    }

    /// <summary>
    /// Gets an available object from the pooler. If none object is available, the 'incrementAmount' number of new objects are created.
    /// </summary>
    /// <returns>An available object from the pooler.</returns>
    public GameObject GetObject()
    {
        GameObject newObject;

        foreach (GameObject obj in pooler)
        {

            if (!obj.activeSelf)
            {
                activeObject(obj, obj.transform.position, obj.transform.rotation);
                return obj;
            }
        }

        amountInPooler += incrementAmount;

        while (pooler.Count < amountInPooler - 1)
        {
            addNewObject();
        }


        newObject = addNewObject();
        newObject.SetActive(true);

        return newObject;
    }

    /// <summary>
    /// Gets an available object from the pooler at the given position. If none object is available, the 'incrementAmount' number of new objects are created.
    /// </summary>
    /// <param name="position">Position of the object in the world.</param>
    /// <returns>An available object from the pooler.</returns>
    public GameObject GetObject(Vector3 position)
    {
        GameObject newObject;

        foreach (GameObject obj in pooler)
        {

            if (!obj.activeSelf)
            {
                activeObject(obj, position, obj.transform.rotation);
                return obj;
            }
        }

        amountInPooler += incrementAmount;


        while (pooler.Count < amountInPooler - 1)
        {
            addNewObject();
        }

        newObject = addNewObject();
        activeObject(newObject, position, newObject.transform.rotation);
        newObject.SetActive(true);

        return newObject;
    }

    /// <summary>
    /// Gets an available object from the pooler at the given position with the given rotation. If none object is available, the 'incrementAmount' number of new objects are created.
    /// </summary>
    /// <param name="position">Position of the object in the world.</param>
    /// <param name="rotation">Rotation of the object in the world.</param>
    /// <returns>An available object from the pooler.</returns>
    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        GameObject newObject;

        foreach (GameObject obj in pooler)
        {

            if (!obj.activeSelf)
            {
                activeObject(obj, position, rotation);
                return obj;
            }
        }

        amountInPooler += incrementAmount;


        while (pooler.Count < amountInPooler - 1)
        {
            addNewObject();
        }


        newObject = addNewObject();
        activeObject(newObject, position, rotation);
        newObject.SetActive(true);

        return newObject;
    }

    /// <summary>
    /// Gets an available object from the pooler, but not active. If none object is available, the 'incrementAmount' number of new objects are created.
    /// </summary>
    /// <returns>An available object from the pooler not active.</returns>
    public GameObject GetNotActiveObject()
    {
        GameObject newObject;

        foreach (GameObject obj in pooler)
        {

            if (!obj.activeSelf)
            {
                return obj;
            }
        }

        amountInPooler += incrementAmount;

        while (pooler.Count < amountInPooler - 1)
        {
            addNewObject();
        }


        newObject = addNewObject();
        newObject.SetActive(false);

        return newObject;
    }

    public GameObject addNewObject()
    {
        GameObject newObject;

        newObject = Instantiate(originalObject, transform);
        newObject.SetActive(false);
        pooler.Add(newObject);

        return newObject;
    }

    public void activeObject(GameObject obj, Vector3 position, Quaternion quaternion)
    {
        obj.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        obj.transform.position = position;
        obj.transform.rotation = quaternion;
        obj.SetActive(true);
    }
}
