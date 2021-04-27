using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using ArenaSystem;

public class Hat : MonoBehaviour
{
    //private Rigidbody rb;
    private Collider collider;
    private bool minFallTimePassed = false;
    private float fallingTime = 1f;
    private Rigidbody rigidbody;
    
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    
        if(collider != null)
            collider.enabled = false;
    }

    public void PlayerDied()
    {
        //if ( !rb )
        //    rb = gameObject.GetComponent<Rigidbody>();
        //rb.constraints = RigidbodyConstraints.None;
    
        //rb = this.gameObject.AddComponent<Rigidbody>();
    
        if(collider != null)
            collider.enabled = true;
    
        StartCoroutine( Falling() );
    }
    
    /// <summary>
    /// To avoid get picked while falling
    /// </summary>
    public IEnumerator Falling()
    {
        yield return new WaitForSeconds( fallingTime );
        minFallTimePassed = true;
        //transform.GetChild( 0 ).gameObject.SetActive( true );
    }

    public void PickupCrown(Collision collision)
    {
        minFallTimePassed = false;
        collision.gameObject.GetComponent<ChildrenObjectsManager>().crown.SetActive( true );
        collision.gameObject.GetComponent<ChildrenObjectsManager>().scepter.SetActive( true );
        collision.gameObject.GetComponent<ChildrenObjectsManager>().spear.SetActive( false );
        collision.gameObject.GetComponent<ChildrenObjectsManager>().shield.SetActive( false );

        //collision.gameObject.transform.root.GetComponent<PlayerInput>().HasHat = true;

        Destroy( gameObject );
    }

    public void PickupCrownNoCollision(GameObject player)
    {
        minFallTimePassed = false;
        player.GetComponent<ChildrenObjectsManager>().crown.SetActive( true );
        player.GetComponent<ChildrenObjectsManager>().scepter.SetActive( true );
        player.GetComponent<ChildrenObjectsManager>().spear.SetActive( false );
        player.GetComponent<ChildrenObjectsManager>().shield.SetActive( false );

        //collision.gameObject.transform.root.GetComponent<PlayerInput>().HasHat = true;

        Destroy( gameObject );
    }

    public void OnCollisionEnter( Collision collision )
    {
        if(minFallTimePassed && collision.gameObject.layer == LayerMask.NameToLayer("Walkable"))
        {
            ParticleSystem[] particles = transform.root.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem particle in particles)
            {
                particle.Play();
            }
        }
        // If player touches the hat while on the ground
        else if ( minFallTimePassed && collision.gameObject.transform.root.tag == "Player" )
        {
            PickupCrown( collision );
        }
        else if(collision.gameObject.transform.root.tag == "Boss" || collision.gameObject.transform.root.tag == "Demon")
        {
            rigidbody.AddForce(new Vector3(2f, 1f, 2f), ForceMode.Impulse);
        }
    }
}
