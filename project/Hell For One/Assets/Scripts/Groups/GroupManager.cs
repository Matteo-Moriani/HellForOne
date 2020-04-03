using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Class that manages the group mechanic
/// </summary>
public class GroupManager : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Enum that lists the aviable groups
    /// </summary>
    public enum Group
    {
        GroupAzure,
        GroupPink,
        GroupGreen,
        GroupYellow,
        All,
        None
    }

    [SerializeField]
    [Tooltip("Field that indicate wich group this is")]
    private Group thisGroupName = Group.None;

    [SerializeField]
    [Tooltip("The color of this group")]
    private Color groupColor = Color.white;
    
    private GameObject[] imps;
    
    private int impsInGroupNumber = 0;
    private int maxImpNumber = 4;
    
    #endregion
    
    #region properties

    /// <summary>
    /// Property that idicates wich group this is
    /// </summary>
    public Group ThisGroupName { get => thisGroupName; private set => thisGroupName = value; }
    
    /// <summary>
    /// Max imps that this group can hace
    /// </summary>
    public int MaxImpNumber { get => maxImpNumber; private set => maxImpNumber = value; }
    
    /// <summary>
    /// Imps in this group
    /// </summary>
    public GameObject[] Imps { get => imps; private set => imps = value; }
    
    /// <summary>
    /// Number of imps in this group
    /// </summary>
    public int ImpsInGroupNumber { get => impsInGroupNumber; private set => impsInGroupNumber = value; }

    #endregion

    #region Delegates and events

    public delegate void OnImpJoined(GroupManager sender, GameObject impJoined);
    public event OnImpJoined onImpJoined;

    public delegate void OnImpRemoved(GroupManager sender, GameObject impRemoved);
    public event OnImpRemoved onImpRemoved;
    
    #region Methods

    private void RiseOnImpJoined(GameObject impJoined)
    {
        onImpJoined?.Invoke(this,impJoined);
    }

    private void RiseOnImpRemoved(GameObject impRemoved)
    {
        onImpRemoved?.Invoke(this,impRemoved);
    }

    #endregion
    
    #endregion
    
    // TODO - optimize this
    public Material groupColorMat;

    // TODO - Manage HUD in another script
    public GameObject healthBar;
    private GroupHealthBar groupHealthBar;

    #region Unity methods

    private void Awake()
    {
        groupHealthBar = healthBar.GetComponent<GroupHealthBar>();
        imps = new GameObject[maxImpNumber];
    }

    #endregion

    #region Methods

    /// <summary>
    /// Tells if this group is empty
    /// </summary>
    /// <returns>True if this group is empty, false otherwise</returns>
    public bool IsEmpty()
    {
        foreach (GameObject imp in Imps)
        {
            if (imp != null)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Returns a random imp from this group
    /// </summary>
    /// <returns>A random imp</returns>
    public GameObject GetRandomImp()
    {
        GameObject imp = null;

        bool found = false;
        while (!found)
        {
            int index = UnityEngine.Random.Range(0, imps.Length);
            if (imps[index] != null)
            {
                imp = imps[index];
                found = true;
            }
        }

        return imp;
    }

    // TODO - Merge this with group finder?
    /// <summary>
    /// Add a demon to this group
    /// </summary>
    /// <param name="imp">The demon to add</param>
    /// <returns>Returns true if the demon is added, false otherwise</returns>
    public bool AddDemonToGroup(GameObject imp)
    {
        int firstEmpty = -1;

        for (int i = 0; i < imps.Length; i++)
        {
            if (!imps[i])
            {
                firstEmpty = i;
                break;
            }
        }

        if (firstEmpty >= 0)
        {
            imps[firstEmpty] = imp;
            
            if(impsInGroupNumber < maxImpNumber) {
                impsInGroupNumber++;
            }
            else { 
                Debug.LogError(this.name + " " + this.gameObject.name + " is trying to increase impsInGroupNumber over maxImpNumber");    
            }
            
            imp.GetComponent<Reincarnation>().onLateReincarnation += OnLateReincarnation;
            imp.GetComponent<Stats>().onLateDeath += OnLateDeath;

            groupHealthBar.SetDemonsNumber(ImpsInGroupNumber);

            return true;
        }
        else
        {
            return false;
        }
    }

    private void RemoveImp(GameObject imp) {
        if(impsInGroupNumber > 0) {
            impsInGroupNumber--;
            int impIndex = System.Array.IndexOf(imps, imp);
            imps[impIndex] = null;

            groupHealthBar.SetDemonsNumber(impsInGroupNumber);
        }
        else { 
            Debug.LogError(this.name + " " + this.gameObject.name + " is trying to decrease impsInGroupNumber but it's already zero" );   
        }

        imp.GetComponent<Reincarnation>().onLateReincarnation -= OnLateReincarnation;
        imp.GetComponent<Stats>().onLateDeath -= OnLateDeath;
        
        RiseOnImpRemoved(imp);
    }
    
    #endregion

    #region Event handlers

    private void OnLateDeath(Stats sender)
    {
        RemoveImp(sender.gameObject);
    }

    private void OnLateReincarnation(GameObject newPlayer)
    {
        RemoveImp(newPlayer);
    }

    #endregion
}
