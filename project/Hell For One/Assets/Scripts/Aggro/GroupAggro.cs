﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupAggro : MonoBehaviour
{
    GameObject[] groups;
    private GroupBehaviour groupBehaviour;
    private bool shouldStayFixed = false;

    [SerializeField]
    private int groupAggro = 0;
    
    [SerializeField]
    private float tankMultiplier = 1.5f;
    [SerializeField]
    private float supportMultiplier = 1.0f;

    private void Start()
    {
        groupAggro = 0;
        groupBehaviour = GetComponent<GroupBehaviour>();

        groups = GameObject.FindGameObjectsWithTag("Group");
    }

    private void Update()
    {
        if (groupBehaviour.MeleeOrderGiven()) {
            if (shouldStayFixed) {
                ManageLockingAggroInDemons(false);
                UpdateGruopAggro();
                shouldStayFixed = false;
            }   
        }
        if (groupBehaviour.TankOrderGiven()) { 
            ManageLockingAggroInDemons(true);
            shouldStayFixed = true;
            groupAggro = Mathf.CeilToInt( (CalculateAverageAggro() / groups.Length) * tankMultiplier );
            
        }
        if (groupBehaviour.SupportOrderGiven()) {
            ManageLockingAggroInDemons(true);
            shouldStayFixed = true;
            groupAggro = Mathf.CeilToInt((CalculateAverageAggro() / groups.Length) * supportMultiplier);
        }
        if (groupBehaviour.RangeAttackOrderGiven()) {
            if (shouldStayFixed) { 
                ManageLockingAggroInDemons(false);
                UpdateGruopAggro();
                shouldStayFixed = false;
            }   
        }
    }

    public int GetAggro() { 
        return groupAggro;    
    }

    public void UpdateGruopAggro() {
        groupAggro = 0;
        
        if (groupBehaviour == null)
            groupBehaviour = GetComponent<GroupBehaviour>();

        if (groupBehaviour != null)
        {
            foreach (GameObject demon in groupBehaviour.demons)
            {
                Stats stats = demon.GetComponent<Stats>();

                if (stats != null)
                {
                    groupAggro += stats.Aggro;    
                }
                else
                {
                    Debug.Log(this.transform.root.gameObject.name + " GruopAggro.ResetGroupAggro cannot find stats in " + demon.name);
                }
            }
        }
        else
        {
            Debug.Log(this.transform.root.gameObject.name + " GruopAggro.ResetGroupAggro cannot find GroupBehaviour");
        }
    }

    public void ResetGroupAggro() { 
        groupAggro = 0;
        
        if(groupBehaviour == null)
            groupBehaviour = GetComponent<GroupBehaviour>();

        if (groupBehaviour != null) { 
            foreach(GameObject demon in groupBehaviour.demons) { 
                Stats stats = demon.GetComponent<Stats>();
                
                if(stats != null) { 
                    stats.Aggro = 0;   
                }
                else { 
                    Debug.Log(this.transform.root.gameObject.name + " GuopAggro.ResetGroupAggro cannot find stats in " + demon.name);
                }
            }    
        }
        else { 
            Debug.Log(this.transform.root.gameObject.name + " GuopAggro.ResetGroupAggro cannot find GroupBehaviour");    
        }
    }

    public void RaiseGroupAggro(int n) { 
        if(!shouldStayFixed){
            groupAggro += n;
        }     
    }

    public void LowerGroupAggro(int n) {
        if (!shouldStayFixed) {
            if (groupAggro > 0)
            {
                if (groupAggro - n < 0)
                {
                    groupAggro = 0;
                }
                else
                {
                    groupAggro -= n;
                }
            }
        }
    }

    private void ManageLockingAggroInDemons(bool lockAggro) {
        if (groupBehaviour == null)
            groupBehaviour = GetComponent<GroupBehaviour>();

        if (groupBehaviour != null)
        {
            foreach (GameObject demon in groupBehaviour.demons)
            {
                Stats stats = demon.GetComponent<Stats>();

                if (stats != null)
                {
                    if(lockAggro)
                        stats.LockAggro();
                    if(!lockAggro)
                        stats.UnlockAggro();
                }
                else
                {
                    Debug.Log(this.transform.root.gameObject.name + " GruopAggro.ResetGroupAggro cannot find stats in " + demon.name);
                }
            }
        }
        else
        {
            Debug.Log(this.transform.root.gameObject.name + " GruopAggro.ResetGroupAggro cannot find GroupBehaviour");
        }
    }

    private int CalculateAverageAggro() {
        int totalAggro = 0;
        
        if(groups != null) { 
            foreach(GameObject group in groups) { 
                totalAggro += group.GetComponent<GroupAggro>().GetAggro();    
            }
        }
        else { 
            Debug.Log(this.transform.root.gameObject.name + " GroupAggro.CalculateAverageAggro cannot find other groups"); 
        }
        return totalAggro;
    }
}
