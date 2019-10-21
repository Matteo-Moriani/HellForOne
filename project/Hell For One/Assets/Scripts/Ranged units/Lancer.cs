using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lancer : MonoBehaviour
{
    [Tooltip("The target of the ranged unit.")]
    public GameObject target;
    [Tooltip("The spawn position of the lance.")]
    public Vector3 lancePosition;

    [Tooltip("The initial speed of the lance.")]
    public float speed;
    [Tooltip("If the lance follow a direct tragectory or not.")]
    public bool direct;

    public float angleCorrection;
    [Tooltip("The maximum distance of attack of the ranged unit.")]
    public float maxDistance;
    [Tooltip("The mininum distance of attack of the ranged unit.")]
    public float minDistance;
    [Tooltip("The number of lances per second.")]
    public float ratio;
    

    GameObject lance;
    ObjectsPooler lances;
    private float lastShot;
    private float timespanShots;

    // Start is called before the first frame update
    void Start()
    {
        lances = GetComponent<ObjectsPooler>();
        lastShot = Time.time;
        timespanShots = 1f / ratio;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Time.time - lastShot > timespanShots)
        {
            if (Launch(target))
            {
                lastShot = Time.time;
            }
        }   
    }

    public bool Launch(GameObject target)
    {
        float distance;
        float alpha;
        if (target == null)
        {
            Debug.LogError("Target cannot be null");
        }

        distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance < minDistance || distance > maxDistance)
        {
            return false;
        }

        alpha = 0;

        if (!calculateAngle(transform.position + lancePosition, target.transform.position, out alpha))
        {
            return false;
        }

        transform.forward = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z) - transform.position;
        lance = lances.GetNotActiveObject();
        lance.GetComponent<Rigidbody>().velocity = Vector3.zero;
        lance.transform.localPosition = lancePosition;
        lance.transform.forward = new Vector3(target.transform.position.x, lance.transform.position.y, target.transform.position.z) - lance.transform.position;
        lance.transform.localRotation = Quaternion.Euler(90f - alpha, angleCorrection, 0);
        lance.SetActive(true);
        lance.GetComponent<Rigidbody>().AddForce(lance.transform.up * (speed), ForceMode.VelocityChange);

        return true;
    }

    bool calculateAngle(Vector3 from, Vector3 to, out float angle)
    {
        float x, y, g, v;
        float tempResult;

        x = Vector3.Distance(new Vector3(from.x, 0, from.z), new Vector3(to.x, 0, to.z));

        y = to.y - from.y;
        g = -Physics.gravity.y;
        v = speed;

        tempResult = g * x * x + 2 * y * v * v;
        tempResult *= g;
        tempResult = v * v * v * v - tempResult;
        if (tempResult < 0)
        {
            angle = 0;
            return false;
            
        }
        
        tempResult = Mathf.Sqrt(tempResult);

        if (direct)
        {
            tempResult = v * v - tempResult;
        }
        else
        {
            tempResult = v * v + tempResult;
        }
        
        tempResult = Mathf.Atan2(tempResult, g * x);

        angle = tempResult * Mathf.Rad2Deg;

        Debug.Log("x: " + x.ToString() + " alpha:" + angle.ToString());
        return true;
    }
}
