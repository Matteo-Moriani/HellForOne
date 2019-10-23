using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public enum Type
    {
        Player,
        Enemy,
        Ally,
        Boss,
        None
    }

    // -TODO- set property?
    [SerializeField]
    public Type type = Type.None;

    // -TODO- set property?
    [SerializeField]
    public int health = 2;

    [SerializeField]
    private int damage = 1;

    [SerializeField]
    private float attackRange = 1.0f;

    [SerializeField]
    private int attackChance = 0;

    [SerializeField]
    private int blockChanceBonus = 0;

    [SerializeField]
    private int knockBackChance = 0;

    [SerializeField]
    private float knockBackUnits = 0f;

    [SerializeField]
    private float knockBackSpeed = 5.0f;

    [SerializeField]
    private int aggro = 0;

    [SerializeField]
    private int crisis = 0;

    [SerializeField]
    private float attackDurationMultiplier = 1.0f;

    public float AttackDurationMultiplier { get => attackDurationMultiplier; private set => attackDurationMultiplier = value; }
    public float AttackRange { get => attackRange; private set => attackRange = value; }
    public int Damage { get => damage; private set => damage = value; }
    public int AttackChance { get => attackChance; private set => attackChance = value; }
    public int BlockChanceBonus { get => blockChanceBonus; private set => blockChanceBonus = value; }
    public int Aggro { get => aggro; set => aggro = value; }
    public int Crisis { get => crisis; set => crisis = value; }
    public int KnockBackChance { get => knockBackChance; set => knockBackChance = value; }
    public float KnockBackUnits { get => knockBackUnits; set => knockBackUnits = value; }

    private bool isProcessingKnockBack = false;

    public void TakeHit(int damage) { 
        health -= damage;    
    }

    public void KnockBack(float units, Transform attackerTransform) { 
        if(!isProcessingKnockBack)
            StartCoroutine(KnockBackCR(units,attackerTransform));
    }

    private IEnumerator KnockBackCR(float units, Transform attackerTransform) {
        isProcessingKnockBack = true;

        float lerpTimer = 0f;

        Vector3 startPosition = this.transform.position;
        Vector3 targetPosition = startPosition + attackerTransform.forward * units; 
           
        while(Vector3.Distance(this.transform.position,targetPosition) > 0.1f) { 
            this.transform.position = Vector3.Lerp(startPosition,targetPosition,lerpTimer * knockBackSpeed);
            
            lerpTimer += Time.deltaTime;

            yield return null;
        }
      
        isProcessingKnockBack = false;
    }
}
