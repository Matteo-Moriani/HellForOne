using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that manage the attack mechanic 
/// </summary>
public class Attack : MonoBehaviour
{
    [SerializeField]
    private Weapon weapon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Melee attack
        if (Input.GetMouseButton(0) || Input.GetButton("Fire1")) { 
            weapon.MeleeAttack();    
        }    
    }
}
