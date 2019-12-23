using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hat : MonoBehaviour
{
    private Rigidbody rb;
    private BoxCollider bc;
    private bool onGround = false;
    private float fallingTime = 2f;

    void Awake()
    {
        //rb = gameObject.GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();

        if(bc != null)
            bc.enabled = false;
    }

    public void PlayerDied()
    {
        //if ( !rb )
        //    rb = gameObject.GetComponent<Rigidbody>();
        //rb.constraints = RigidbodyConstraints.None;

        rb = this.gameObject.AddComponent<Rigidbody>();

        if(bc != null)
            bc.enabled = true;

        StartCoroutine( Falling() );
    }

    /// <summary>
    /// To avoid get picked while falling
    /// </summary>
    public IEnumerator Falling()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        yield return new WaitForSeconds( fallingTime );
        onGround = true;
        transform.GetChild( 0 ).gameObject.SetActive( true );
    }

    public void OnCollisionEnter( Collision collision )
    {
        // If player touches the hat while on the ground
        if ( onGround && collision.gameObject.transform.root.tag == "Player" )
        {
            onGround = false;
            collision.gameObject.GetComponent<ChildrenObjectsManager>().crown.SetActive( true );
            //collision.gameObject.transform.root.Find("Crown").gameObject.SetActive(true);
            collision.gameObject.transform.root.GetComponent<TacticsManager>().enabled = true;
            Destroy( gameObject );
        }
    }
}
