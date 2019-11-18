﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushAway : MonoBehaviour
{
    public float repulsionDist = 0.3f;
    //public float pushIntensity = 0.75f;
    public float pushAwaySize = 0.5f;
    public float pushAwayTime = 0.5f;
    private GameObject[] demons;
    
    void Start()
    {
        demons = GameObject.FindGameObjectsWithTag("Demon");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        PushDemons();
    }

    private float HorizDistFromTarget(Vector3 targetPosition) {
        targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        return (targetPosition - transform.position).magnitude - transform.localScale.x/2;
    }

    private void PushDemons() {
        foreach(GameObject demon in demons) {

            if(demon != null) {
                // closest point of demon's collider to the closest point of my collider to the demon center
                if(HorizDistFromTarget(demon.GetComponent<Collider>().ClosestPoint(GetComponent<Collider>().ClosestPoint(demon.transform.position))) < repulsionDist) {
                    //demon.GetComponent<Rigidbody>().AddForce((demon.transform.position - transform.position) * pushIntensity, ForceMode.Impulse);
                    StartCoroutine(pushAwayCoroutine(demon));
                }
            }
            
        }
    }

    private IEnumerator pushAwayCoroutine(GameObject demon) {
        Stats demonStats = demon.GetComponent<Stats>();
        
        if(demonStats != null) {
            if (!demonStats.IsPushedAway) { 
                demonStats.IsPushedAway = true;

                float pushAwayTimeCounter = 0f;

                Rigidbody demonRb = demon.GetComponent<Rigidbody>();


                Vector3 startingVelocity = demonRb.velocity;

                // If pushed away Player cannot move or dash
                if (demonStats.type == Stats.Type.Player)
                {
                    Controller controller = demon.GetComponent<Controller>();
                    Dash dash = demon.GetComponent<Dash>();
                    if (controller != null && dash != null)
                    {
                        controller.enabled = false;
                        dash.enabled = false;
                    }
                    else
                    {
                        Debug.Log(this.transform.root.name + " is trying to pushAway a Player that does not have controller or dash. Player: " + demon.transform.root.name);
                    }
                }

                // If pushed away ally imp cannot move
                if (demonStats.type == Stats.Type.Ally)
                {
                    DemonMovement demonMovement = demon.GetComponent<DemonMovement>();
                    if (demonMovement != null)
                    {
                        demonMovement.CanMove = false;
                    }
                    else
                    {
                        Debug.Log(this.transform.root.name + " is trying to pushAway an ally Imp that does not have demonMovement. Imp: " + demon.transform.root.name);
                    }
                }

                // Calculate PushAway direction
                Vector3 pushAwayDirection = demon.transform.position - transform.position;
                pushAwayDirection.y = 0.0f;
                pushAwayDirection = pushAwayDirection.normalized;

                // PushAway cycle
                do
                {
                    pushAwayTimeCounter += Time.fixedDeltaTime;
                    if(demonRb != null)
                        demonRb.velocity = pushAwayDirection * (pushAwaySize / pushAwayTime);

                    // We are using physics so we need to wait for FixedUpdate
                    yield return new WaitForFixedUpdate();
                } while (pushAwayTimeCounter <= pushAwayTime);

                if (demonStats.type == Stats.Type.Player)
                {
                    Controller controller = demon.GetComponent<Controller>();
                    Dash dash = demon.GetComponent<Dash>();
                    if (controller != null && dash != null)
                    {
                        controller.enabled = true;
                        dash.enabled = true;
                    }
                    else
                    {
                        Debug.Log(this.transform.root.name + " is trying to pushAway a Player that does not have controller or dash. Player: " + demon.transform.root.name);
                    }
                }

                if (demonStats.type == Stats.Type.Ally)
                {
                    DemonMovement demonMovement = demon.GetComponent<DemonMovement>();
                    if (demonMovement != null)
                    {
                        demonMovement.CanMove = true;
                    }
                    else
                    {
                        Debug.Log(this.transform.root.name + " is trying to pushAway an ally Imp that does not have demonMovement. Imp: " + demon.transform.root.name);
                    }
                }

                demonRb.velocity = startingVelocity;

                demonStats.IsPushedAway = false;
            }   
        }
        else{
            Debug.Log(this.transform.root.name + " is trying to pushAway a demon that does not have Stats. Demon: " + demon.transform.root.name);
        }
    }
}
