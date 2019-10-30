using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanceAttackColliderManager : MonoBehaviour
{
    [SerializeField]
    private GameObject attackCollider;
    private void OnEnable()
    {
        if(attackCollider != null) { 
            attackCollider.SetActive(true);    
        }
        else { 
            Debug.Log(this.gameObject.name + " LanceAttackColliderManager - AttackCollider is null");    
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        if (attackCollider != null)
        {
            attackCollider.SetActive(false);
        }
        else
        {
            Debug.Log(this.gameObject.name + " LanceAttackColliderManager - AttackCollider is null");
        }
    }
}
