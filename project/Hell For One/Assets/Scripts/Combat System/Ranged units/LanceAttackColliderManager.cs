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
        //else { 
        //    Debug.Log(this.gameObject.name + " LanceAttackColliderManager - AttackCollider is null");    
        //}
    }

    private void OnDisable()
    {
        if (attackCollider != null)
        {
            attackCollider.SetActive(false);
        }
        //else
        //{
        //    Debug.Log(this.gameObject.name + " LanceAttackColliderManager - AttackCollider is null");
        //}
    }
}
