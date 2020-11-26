using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupHealthBar : MonoBehaviour
{
    private Image healthBarInside;
    private float maxDemons = 4f;
    // starting number
    private float demonsNum = 0f;
    
    void Start() {
        healthBarInside = gameObject.GetComponent<Image>();
        healthBarInside.fillAmount = demonsNum / maxDemons;
    }

    private void Update() {
        healthBarInside.fillAmount = demonsNum / maxDemons;
    }

    public void SetDemonsNumber(int i) {
        demonsNum = i;
        if (healthBarInside == null)
            healthBarInside = gameObject.GetComponent<Image>();
        healthBarInside.fillAmount = demonsNum / maxDemons;
    }
    
}
