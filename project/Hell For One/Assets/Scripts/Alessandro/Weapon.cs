using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that rappresents a weapon in the game.
/// Must have a rigidBody and a collider set to trigger
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Weapon : MonoBehaviour
{
    [SerializeField]
    private GameObject owner;
    
    private bool isAttacking;

    private Rigidbody rb;
    private BoxCollider collider;

    // Start is called before the first frame update
    private void Awake()
    {
        isAttacking = false;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        collider = GetComponent<BoxCollider>();
        collider.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttacking) { 
            this.transform.position = owner.transform.position;  
            isAttacking = false;
        }
    }
    
    public void MeleeAttack() { 
        isAttacking = true;
        this.transform.Translate(0.0f,0.0f,this.transform.localScale.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy") {
            Debug.Log("You hit " + other.gameObject.name);    
        }
    }
}
