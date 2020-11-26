using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hat : MonoBehaviour
{
    //private Rigidbody rb;
    private Collider collider;
    private bool onGround = false;
    private float fallingTime = 2f;
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
            //collision.gameObject.transform.root.GetComponent<TacticsManager>().enabled = true;
            collision.gameObject.transform.root.GetComponent<PlayerInput>().HasHat = true;
            Destroy( gameObject );
        }
        else if(collision.gameObject.transform.root.tag == "Boss" || collision.gameObject.transform.root.tag == "Demon")
        {
            rigidbody.AddForce(new Vector3(1f, 1f, 1f), ForceMode.Impulse);
        }
    }
}
