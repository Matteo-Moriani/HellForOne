using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lance : MonoBehaviour
{
    [SerializeField]
    private List<string> ignoredTags;

    private void FixedUpdate()
    {
        transform.up = GetComponent<Rigidbody>().velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        Stats stats;

        if (other.tag == "Demon")
        {
            stats = other.GetComponent<Stats>();
            if (stats != null && (stats.type == Stats.Type.Boss || stats.type == Stats.Type.Enemy))
            {
                gameObject.SetActive(false);
            }
        }
        else if (!ignoredTags.Contains(other.tag))
        {
            gameObject.SetActive(false);
        }
        
    }
}
