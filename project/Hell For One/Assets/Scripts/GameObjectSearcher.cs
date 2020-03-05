using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectSearcher : MonoBehaviour
{
    [HideInInspector]
    public string searchTag;

    public List<GameObject> actors = new List<GameObject>();

    public GameObject GetFirstChildWithTag()
    {
        foreach (GameObject go in actors )
        {
            if ( go )
                return go;
        }
        return null;
    }

    public void FindObjectWithTag( Transform parent, string _tag )
    {
        actors.Clear();
        //Transform parent = transform;
        GetChildObject( parent, _tag );
    }

    public void GetChildObject( Transform parent, string _tag )
    {
        for ( int i = 0; i < parent.childCount; i++ )
        {
            Transform child = parent.GetChild( i );
            if ( child.tag == _tag )
            {
                actors.Add( child.gameObject );
            }
            if ( child.childCount > 0 )
            {
                GetChildObject( child, _tag );
            }
        }
    }
}