using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lance : MonoBehaviour
{
    [SerializeField, Tooltip("The collisions with objects with a tag in this list don't desable the lance.")]
    private List<string> ignoredTags;

    [SerializeField, Range(-1, 5), Tooltip("The lance remains active after a valid collision for this frame number.")]
    private int numberFrames;

    private int actualFrame;
    private bool deactivates;
    private Stats stats;

    private void OnEnable()
    {
        deactivates = false;
        actualFrame = numberFrames;
    }

    private void Start()
    {
        deactivates = false;
        if (ignoredTags == null)
        {
            ignoredTags = new List<string>();
        }
    }

    private void FixedUpdate()
    {
        transform.up = GetComponent<Rigidbody>().velocity;
        if (deactivates)
        {
            actualFrame--;
            if (actualFrame < 0)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Stats stats;

        if (other.tag == "Demon")
        {
            stats = other.GetComponent<Stats>();
            if (stats != null && (stats.type == Stats.Type.Boss || stats.type == Stats.Type.Enemy))
            {
                deactivates = true;
            }
        }
        else if (!ignoredTags.Contains(other.tag))
        {
            deactivates = true;
        }

    }

    /// <summary>
    /// Sets or gets the stats of the lancer.
    /// </summary>
    /// <value>The stats of the lancer.</value>
    public Stats LancerStats
    {
        get
        {
            return stats;
        }
        set
        {
            stats = value;
        }
    }

}
