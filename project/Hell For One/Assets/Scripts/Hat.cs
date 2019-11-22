using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hat : MonoBehaviour
{
    private Rigidbody rb;
    private bool onGround = false;
    private float fallingTime = 2f;

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    public void PlayerDied()
    {
        if (!rb)
            rb = gameObject.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;
        StartCoroutine( Falling() );
    }

    /// <summary>
    /// To avoid get picked while falling
    /// </summary>
    public IEnumerator Falling()
    {
        yield return new WaitForSeconds( fallingTime );
        onGround = true;
    }

    public void OnCollisionEnter( Collision collision )
    {
        // If player touches the hat while on the ground
        if ( onGround && collision.gameObject.transform.root.tag == "Player" )
        {
            onGround = false;
            collision.gameObject.transform.root.Find( "Hat" ).gameObject.SetActive( true );
            collision.gameObject.transform.root.GetComponent<TacticsManager>().enabled = true;
            Destroy( gameObject );
        }
    }
}
