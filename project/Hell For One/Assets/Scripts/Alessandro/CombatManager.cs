using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField]
    private GameObject attackCollider;
    [SerializeField]
    private GameObject blockCollider;
    [SerializeField]
    private GameObject idleCollider;
    
    void Start()
    {
        // -TODO- add if null coditions.
        // and if true init GO.

        attackCollider.SetActive(false);
        blockCollider.SetActive(false);
        idleCollider.SetActive(true);
    }

    void Update()
    {
        
    }

    public void StartBlock() {
        blockCollider.SetActive(true);    
    }
    
    public void StopBlock() { 
        blockCollider.SetActive(false);    
    }

    public void Attack() { 
        StartCoroutine(AttackCoroutine());    
    }

    private IEnumerator AttackCoroutine() { 
        attackCollider.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        attackCollider.SetActive(false);
        yield return null;
    }
}
