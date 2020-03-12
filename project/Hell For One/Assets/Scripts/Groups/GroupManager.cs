using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Class that manages the group mechanic
/// </summary>
public class GroupManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Field that indicate wich group this is")]
    private Group thisGroupName = Group.None;

    [SerializeField]
    [Tooltip("The color of this group")]
    private Color groupColor = Color.white;

    /// <summary>
    /// Enum that lists the aviable groups
    /// </summary>
    public enum Group
    {
        GroupAzure,
        GroupPink,
        GroupGreen,
        GroupYellow,
        None
    }

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

    private Action<GameObject> OnImpJoined;
    
    [SerializeField]
    [Tooltip("Here only for testing")]
    private GameObject[] imps;
    [SerializeField]
    [Tooltip("Here only for testing")]
    private int impsInGroupNumber = 0;
    
    private int maxImpNumber = 4;

    // TODO - optimize this
    public Material groupColorMat;

    // TODO - Manage HUD in another script
    public GameObject healthBar;
    private GroupHealthBar groupHealthBar;

    private void Awake()
    {
        groupHealthBar = healthBar.GetComponent<GroupHealthBar>();
    }

    private void OnEnable()
    {
        RegisterOnImpJoined(OnImpJoinedLocalHandler);
    }

    private void OnDisable()
    {
        UnregisterOnImpJoined(OnImpJoinedLocalHandler);
    }

    private void Start()
    {
        imps = new GameObject[maxImpNumber];
    }

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

    /// <summary>
    /// Add a demon to this group
    /// </summary>
    /// <param name="demon">The demon to add</param>
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
            

            RaiseOnImpJoined(imp);

            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnImpJoinedLocalHandler(GameObject imp) { 
        Reincarnation reincarnation = imp.GetComponent<Reincarnation>();

        reincarnation.RegisterOnReincarnation(RemoveImp);

        groupHealthBar.SetDemonsNumber(ImpsInGroupNumber);
    }

    public void RemoveImp(GameObject imp) {
        if(impsInGroupNumber > 0) {
            impsInGroupNumber--;
            int impIndex = System.Array.IndexOf(imps, imp);
            imps[impIndex] = null;

            groupHealthBar.SetDemonsNumber(impsInGroupNumber);
        }
        else { 
            Debug.LogError(this.name + " " + this.gameObject.name + " is trying to decrease impsInGroupNumber but it's already zero" );   
        }

        Reincarnation reincarnation = imp.GetComponent<Reincarnation>();

        reincarnation.UnregisterOnReincarnation(RemoveImp);
    }

    public void RegisterOnImpJoined(Action<GameObject> method) { 
        OnImpJoined += method;    
    }

    public void UnregisterOnImpJoined(Action<GameObject> method) { 
        OnImpJoined -= method;   
    }

    private void RaiseOnImpJoined(GameObject imp) { 
        if(OnImpJoined != null) { 
            OnImpJoined(imp);
        }    
    }
}
