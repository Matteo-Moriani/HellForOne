using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushAway : MonoBehaviour
{
    public float repulsionDist = 0.3f;
    //public float pushIntensity = 0.75f;
    public float pushAwaySize = 0.5f;
    public float pushAwayTime = 0.5f;
    //private GameObject[] demons;

    void Start()
    {
        //demons = GameObject.FindGameObjectsWithTag( "Demon" );
    }

    void FixedUpdate()
    {
        if (BattleEventsHandler.IsInBattle)
        {
            PushDemons();
        }
    }

    private float HorizDistFromTarget(Vector3 targetPosition)
    {
        targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        return (targetPosition - transform.position).magnitude - transform.localScale.x / 2;
    }

    private void PushDemons()
    {
        // TODO - this is not very clean
        foreach (GameObject demon in AlliesManager.Instance.AlliesList)
        {

            if (demon != null)
            {
                // closest point of demon's collider to the closest point of my collider to the demon center
                if (HorizDistFromTarget(demon.GetComponent<Collider>().ClosestPoint(GetComponent<Collider>().ClosestPoint(demon.transform.position))) < repulsionDist)
                {
                    //demon.GetComponent<Rigidbody>().AddForce((demon.transform.position - transform.position) * pushIntensity, ForceMode.Impulse);
                    StartCoroutine(pushAwayCoroutine(demon));
                }
            }

        }
    }

    private IEnumerator pushAwayCoroutine(GameObject demon)
    {
        Stats demonStats = demon.GetComponent<Stats>();

        if (demonStats != null)
        {
            if (!demonStats.IsPushedAway && demonStats.ThisUnitType != Stats.Type.Player)
            {
                demonStats.IsPushedAway = true;

                float pushAwayTimeCounter = 0f;

                Rigidbody demonRb = demon.GetComponent<Rigidbody>();

                Vector3 startingVelocity = demonRb.velocity;

                // If pushed away Player cannot move or dash
                if (demonStats.ThisUnitType == Stats.Type.Player)
                {
                    PlayerController playerController = demon.GetComponent<PlayerController>();
                    Dash dash = demon.GetComponent<Dash>();
                    if (playerController != null && dash != null)
                    {
                        playerController.enabled = false;
                        dash.enabled = false;
                    }
                    //else
                    //{
                    //    Debug.Log(this.transform.root.name + " is trying to pushAway a Player that does not have controller or dash. Player: " + demon.transform.root.name);
                    //}
                }

                // If pushed away ally imp cannot move
                if (demonStats.ThisUnitType == Stats.Type.Ally)
                {
                    AllyImpMovement demonMovement = demon.GetComponent<AllyImpMovement>();
                    if (demonMovement != null)
                    {
                        demonMovement.CanMove = false;
                    }
                    //else
                    //{
                    //    Debug.Log(this.transform.root.name + " is trying to pushAway an ally Imp that does not have demonMovement. Imp: " + demon.transform.root.name);
                    //}
                }

                // Calculate PushAway direction
                Vector3 pushAwayDirection = demon.transform.position - transform.position;
                pushAwayDirection.y = 0.0f;
                pushAwayDirection = pushAwayDirection.normalized;

                // PushAway cycle
                do
                {
                    pushAwayTimeCounter += Time.fixedDeltaTime;
                    if (demonRb != null)
                        demonRb.velocity = pushAwayDirection * (pushAwaySize / pushAwayTime);

                    // We are using physics so we need to wait for FixedUpdate
                    yield return new WaitForFixedUpdate();
                } while (pushAwayTimeCounter <= pushAwayTime);

                // We need this check because the demon that is being pushed away could die during the process
                if (demon != null)
                {
                    // Player can move again
                    if (demonStats.ThisUnitType == Stats.Type.Player)
                    {
                        PlayerController playerController = demon.GetComponent<PlayerController>();
                        Dash dash = demon.GetComponent<Dash>();
                        if (playerController != null && dash != null)
                        {
                            if (demon.tag == "Player")
                            {
                                playerController.enabled = true;
                                dash.enabled = true;
                            }
                            else
                                dash.enabled = true;
                        }
                        //else
                        //{
                        //    Debug.Log(this.transform.root.name + " is trying to pushAway a Player that does not have controller or dash. Player: " + demon.transform.root.name);
                        //}
                    }

                    // Ally can move again
                    if (demonStats.ThisUnitType == Stats.Type.Ally)
                    {
                        AllyImpMovement demonMovement = demon.GetComponent<AllyImpMovement>();
                        if (demonMovement != null)
                        {
                            demonMovement.CanMove = true;
                        }
                        //else
                        //{
                        //    Debug.Log(this.transform.root.name + " is trying to pushAway an ally Imp that does not have demonMovement. Imp: " + demon.transform.root.name);
                        //}
                    }
                }

                // We need this check because the demon that is being pushed away could die during the process
                if (demonRb != null)
                    demonRb.velocity = startingVelocity;

                demonStats.IsPushedAway = false;
            }
        }
        //else
        //{
        //    Debug.Log(this.transform.root.name + " is trying to pushAway a demon that does not have Stats. Demon: " + demon.transform.root.name);
        //}
    }
}
