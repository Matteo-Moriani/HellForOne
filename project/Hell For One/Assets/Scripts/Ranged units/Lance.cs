using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lance : MonoBehaviour
{
    [SerializeField]
    private List<string> ignoredTags;

    [SerializeField]
    private int numberFrames;
    private bool deactivates;

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
            numberFrames--;
            if (numberFrames < 0)
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
}
