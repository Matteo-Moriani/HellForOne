using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lancer : MonoBehaviour
{
    [SerializeField, Tooltip("The target of the ranged unit.")]
    private GameObject target;
    [Tooltip("Indicates if the unit can launch to the target or not.")]
    private bool canLaunch;
    [Space]
    [SerializeField, Min(0), Tooltip("The number of lances per second.")]
    private float ratio;
    [SerializeField ,Min(0), Tooltip("The initial speed of the lance.")]
    private float speed;
    [Space]
    [SerializeField, Min(0), Tooltip("The mininum distance of attack of the ranged unit.")]
    private float minDistance;
    [SerializeField, Min(0), Tooltip("The maximum distance of attack of the ranged unit.")]
    private float maxDistance;
    [Space]
    [SerializeField, Tooltip("The spawn position of the lance.")]
    private Vector3 lancePosition;
    [SerializeField, Tooltip("If the lance follow a direct tragectory or not (false is recommended).")]
    private bool direct;
    [SerializeField, Tooltip("The angle on the right or left to adjust the trajectory of the lance.")]
    private float angleCorrection;

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
        if (Time.time - lastShot > timespanShots && target != null && canLaunch)
        {
            if (Launch(target))
            {
                lastShot = Time.time;
            }
        }   
    }

    /// <summary>
    /// It launches a lance to the target if the target is at a distance between minDistance and maxDistance.
    /// </summary>
    /// <param name="target">The object target.</param>
    /// <returns>Returns true if the launch was successful, otherwise returns false.</returns>
    private bool Launch(GameObject target)
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

    /// <summary>
    /// Returns the angle range (with 0° corresponding to the ground) to launch an object from a position to another with a velocity that is "speed".
    /// </summary>
    /// <param name="from">The start position of the launch.</param>
    /// <param name="to">The final position of the launch.</param>
    /// <param name="angle">The varible that will contain the angle in degree.</param>
    /// <returns>Returns true if it is mathematically possible to have a trajectory, otherwise returns false.</returns>
    private bool calculateAngle(Vector3 from, Vector3 to, out float angle)
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

    #region "Properties"
    /// <summary>
    /// "The target of the ranged unit."
    /// </summary>
    public GameObject Target
    {
        get
        {
            return target;
        }
        set
        {
            if (value != null && value.GetComponent<Transform>() != null)
            {
                target = value;
            }
        }
    }


    /// <summary>
    /// Indicates if the unit can launch to the target or not."
    /// </summary>
    public bool CanLaunch
    {
        get
        {
            return canLaunch;
        }
        set
        {
            canLaunch = value;
        }
    }

    /// <summary>
    /// "The number of lances per second."
    /// </summary>
    public float Ratio
    {
        get
        {
            return ratio;
        }
        set
        {
            if (value > 0)
            {
                ratio = value;
            }
        }
    }

    /// <summary>
    /// "The initial speed of the lance."
    /// </summary>
    public float Speed
    {
        get
        {
            return speed;
        }
        set
        {
            if (value > 0)
            {
                speed = value;
            }
        }
    }

    /// <summary>
    /// "The mininum distance of attack of the ranged unit."
    /// </summary>
    public float MinDistance
    {
        get
        {
            return MinDistance;
        }
        set
        {
            if (value >= 0 && value < MaxDistance)
            {
                minDistance = value;
            }
        }
    }

    /// <summary>
    /// "The maximum distance of attack of the ranged unit."
    /// </summary>
    public float MaxDistance
    {
        get
        {
            return maxDistance;
        }
        set
        {
            if (value >= 0 && value > MinDistance)
            {
                maxDistance = value;
            }
        }
    }

    /// <summary>
    /// "The spawn position of the lance."
    /// </summary>
    public Vector3 StartLancePosition
    {
        get
        {
            return lancePosition;
        }
        set
        {
            lancePosition = value;
        }
    }

    /// <summary>
    /// If the lance follow a direct tragectory or not (false is recommended).
    /// </summary>
    public bool DirectTrajectory
    {
        get
        {
            return direct;
        }
        set
        {
            direct = value;
        }
    }

    /// <summary>
    /// The angle on the right or left to adjust the trajectory of the lance.
    /// </summary>
    public float AngleCorrection
    {
        get
        {
            return angleCorrection;
        }
        set
        {
            angleCorrection = value;
        }
    }
    #endregion

}
