﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectSearcher : MonoBehaviour
{
    [HideInInspector]
    public string searchTag;

    public List<GameObject> actors = new List<GameObject>();


    //void Start()
    //{
    //    if ( searchTag != null )
    //    {
    //        FindObjectwithTag( searchTag );
    //    }
    //}

    public GameObject GetFirstChildWithTag()
    {
        return actors[ 0 ];
    }

    public void FindObjectwithTag( Transform parent, string _tag )
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