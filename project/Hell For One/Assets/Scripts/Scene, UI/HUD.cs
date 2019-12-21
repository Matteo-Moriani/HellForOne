﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    private GameObject panelAzure, panelPink, panelGreen, panelYellow;
    private Image azureImage, pinkImage, greenImage, yellowImage, bossFaceAzure, bossFacePink, bossFaceGreen, bossFaceYellow;
    private Sprite meleeSprite, rangeSprite, tankSprite, supportSprite, fullHPSprite, halfHPSprite, fullCrownSprite, halfCrownSprite;
    private GroupBehaviour groupAzure, groupPink, groupGreen, groupYellow;
    private TacticsManager tacticsManager;
    private Vector3 defaultScale = new Vector3( 1f, 1f, 1f );
    private Vector3 enlargedScale = new Vector3( 1.5f, 1.5f, 1f );
    private GroupBehaviour[] groupBehaviours = new GroupBehaviour[ 4 ];
    private Dictionary<GroupBehaviour, Image> dict = new Dictionary<GroupBehaviour, Image>();
    private Image[] healthPoolArray = new Image[ 17 ];
    private int impsCount = 1;
    private int healthIconsCount = 17;
    private AlliesManager alliesManager;
    GameObject healthPool;
    private Vector3 groupPanelCorrectionVector = new Vector3( +36.25f, -22.50f, 0f );
    private Vector2 panelPosition = new Vector2( -36.25f, 22.33f );
    private Vector2 xCorrection = new Vector2( 145f, 0f );

    private GameObject player;
    private CombatEventsManager playerCombatEventsManager;

    /// <summary>
    /// Used to update the health pool if imps die or join the horde
    /// </summary>
    public void ResizeHealthPool()
    {
        // An Imp died
        if ( alliesManager.AlliesList.Count < impsCount - 1 )
        {
            impsCount = alliesManager.AlliesList.Count + 1;

            for ( int i = alliesManager.AlliesList.Count; i < healthPool.transform.childCount; i++ )
            {
                healthPool.transform.GetChild( i + 1 ).gameObject.SetActive( false );
            }

            for ( int i = 0; i < alliesManager.AlliesList.Count; i++ )
            {
                healthPool.transform.GetChild( i ).gameObject.SetActive( true );
            }
        }

        // An Imp joined
        //if ( alliesManager.AlliesList.Count >= impsCount - 1 )
        if ( alliesManager.AlliesList.Count >= impsCount )
        {
            impsCount = alliesManager.AlliesList.Count + 1;

            for ( int i = 0; i < alliesManager.AlliesList.Count + 1; i++ )
            {
                healthPool.transform.GetChild( i ).gameObject.SetActive( true );
            }
        }
    }

    /// <summary>
    /// Halves the health icons if imps take damage
    /// </summary>
    public void CheckImpsHealth()
    {
        if ( GameObject.FindGameObjectWithTag( "Player" ).GetComponent<Stats>().health < 4 )
            healthPoolArray[ 0 ].overrideSprite = halfCrownSprite;
        else
            healthPoolArray[ 0 ].overrideSprite = fullCrownSprite;

        for ( int i = 1; i < alliesManager.AlliesList.Count + 1; i++ )
        {
            if ( alliesManager.AlliesList[ i - 1 ].GetComponent<Stats>().health < 4 )
                healthPoolArray[ i ].overrideSprite = halfHPSprite;
            else if ( !healthPoolArray[ i ] )
            {
                healthPoolArray[ i ] = healthPool.transform.GetChild( i ).gameObject.GetComponent<Image>();
                healthPoolArray[ i ].overrideSprite = fullHPSprite;
            }
            else
                healthPoolArray[ i ].overrideSprite = fullHPSprite;
        }
    }

    public void ActivateBossFace(TacticsManager.Group group )
    {
        switch ( group )
        {
            case TacticsManager.Group.GroupAzure:
                bossFaceAzure.enabled = true;
                bossFaceGreen.enabled = false;
                bossFacePink.enabled = false;
                bossFaceYellow.enabled = false;
                break;
            case TacticsManager.Group.GroupGreen:
                bossFaceAzure.enabled = false;
                bossFaceGreen.enabled = true;
                bossFacePink.enabled = false;
                bossFaceYellow.enabled = false;
                break;
            case TacticsManager.Group.GroupPink:
                bossFaceAzure.enabled = false;
                bossFaceGreen.enabled = false;
                bossFacePink.enabled = true;
                bossFaceYellow.enabled = false;
                break;
            case TacticsManager.Group.GroupYellow:
                bossFaceAzure.enabled = false;
                bossFaceGreen.enabled = false;
                bossFacePink.enabled = false;
                bossFaceYellow.enabled = true;
                break;
        }
    }

    public void DeactivateBossFace() {
        bossFaceAzure.enabled = false;
        bossFaceGreen.enabled = false;
        bossFacePink.enabled = false;
        bossFaceYellow.enabled = false;
    }

    void Start()
    {
        GameObject panel = transform.GetChild( 0 ).gameObject;
        panelAzure = panel.transform.GetChild( 0 ).gameObject;
        panelPink = panel.transform.GetChild( 1 ).gameObject;
        panelGreen = panel.transform.GetChild( 2 ).gameObject;
        panelYellow = panel.transform.GetChild( 3 ).gameObject;

        azureImage = panelAzure.transform.GetChild( 0 ).gameObject.GetComponent<Image>();
        bossFaceAzure = panelAzure.transform.GetChild( 1 ).gameObject.GetComponent<Image>();
        pinkImage = panelPink.transform.GetChild( 0 ).gameObject.GetComponent<Image>();
        bossFacePink = panelPink.transform.GetChild( 1 ).gameObject.GetComponent<Image>();
        greenImage = panelGreen.transform.GetChild( 0 ).gameObject.GetComponent<Image>();
        bossFaceGreen = panelGreen.transform.GetChild( 1 ).gameObject.GetComponent<Image>();
        yellowImage = panelYellow.transform.GetChild( 0 ).gameObject.GetComponent<Image>();
        bossFaceYellow = panelYellow.transform.GetChild( 1 ).gameObject.GetComponent<Image>();

        bossFaceAzure.enabled = false;
        bossFacePink.enabled = false;
        bossFaceGreen.enabled = false;
        bossFaceYellow.enabled = false;

        meleeSprite = Resources.Load<Sprite>( "Sprites/melee_black" );
        rangeSprite = Resources.Load<Sprite>( "Sprites/ranged_black" );
        tankSprite = Resources.Load<Sprite>( "Sprites/tank_black" );
        supportSprite = Resources.Load<Sprite>( "Sprites/dance_black" );
        fullHPSprite = Resources.Load<Sprite>( "Sprites/faceIcon" );
        halfHPSprite = Resources.Load<Sprite>( "Sprites/halfFaceIcon" );
        halfCrownSprite = Resources.Load<Sprite>( "Sprites/halfFaceIconCrown" );
        fullCrownSprite = Resources.Load<Sprite>( "Sprites/faceIconCrown" );

        player = GameObject.FindGameObjectWithTag( "Player" );

        if ( player != null )
        {
            tacticsManager = player.GetComponent<TacticsManager>();
            playerCombatEventsManager = player.GetComponent<CombatEventsManager>();

            if ( playerCombatEventsManager != null )
            {
                playerCombatEventsManager.onDeath += OnPlayerDeath;
            }
            else
            {
                Debug.LogError( "HUD cannot find player's CombatEventsManager" );
            }
        }
        else
        {
            Debug.LogError( "HUD cannot find player" );
        }

        GameObject[] groups = GameObject.FindGameObjectsWithTag( "Group" );

        foreach ( GameObject go in groups )
        {
            switch ( go.name )
            {
                case "GroupAzure":
                    groupAzure = go.GetComponent<GroupBehaviour>();
                    break;
                case "GroupPink":
                    groupPink = go.GetComponent<GroupBehaviour>();
                    break;
                case "GroupGreen":
                    groupGreen = go.GetComponent<GroupBehaviour>();
                    break;
                case "GroupYellow":
                    groupYellow = go.GetComponent<GroupBehaviour>();
                    break;
            }
        }

        groupBehaviours[ 0 ] = groupAzure;
        groupBehaviours[ 1 ] = groupPink;
        groupBehaviours[ 2 ] = groupGreen;
        groupBehaviours[ 3 ] = groupYellow;

        dict.Add( groupAzure, azureImage );
        dict.Add( groupPink, pinkImage );
        dict.Add( groupGreen, greenImage );
        dict.Add( groupYellow, yellowImage );

        alliesManager = GameObject.FindGameObjectWithTag( "Managers" ).GetComponentInChildren<AlliesManager>();

        //foreach (GameObject go in alliesManager.AlliesList )
        //{

        //}

        healthPool = transform.GetChild( 1 ).gameObject;

        // Player's Health
        healthPoolArray[ 0 ] = healthPool.transform.GetChild( 0 ).gameObject.GetComponent<Image>();

        for ( int i = 1; i < alliesManager.AlliesList.Count; i++ )
        {
            healthPoolArray[ i ] = healthPool.transform.GetChild( i ).gameObject.GetComponent<Image>();
            impsCount++;
        }

        for ( int i = alliesManager.AlliesList.Count + 1; i < healthPool.transform.childCount; i++ )
        {
            healthPool.transform.GetChild( i ).gameObject.SetActive( false );
            healthIconsCount--;
        }
    }

    private void OnDisable()
    {
        if ( playerCombatEventsManager != null )
        {
            playerCombatEventsManager.onDeath -= OnPlayerDeath;
        }
    }

    void Update()
    {
        ResizeHealthPool();

        CheckImpsHealth();

        foreach ( GroupBehaviour gb in groupBehaviours )
        {
            switch ( gb.currentState )
            {
                case GroupBehaviour.State.MeleeAttack:
                    dict[ gb ].overrideSprite = meleeSprite;
                    break;
                case GroupBehaviour.State.RangeAttack:
                    dict[ gb ].overrideSprite = rangeSprite;
                    break;
                case GroupBehaviour.State.Tank:
                    dict[ gb ].overrideSprite = tankSprite;
                    break;
                case GroupBehaviour.State.Support:
                    dict[ gb ].overrideSprite = supportSprite;
                    break;
            }
        }

        switch ( tacticsManager.CurrentShowedGroup )
        {
            case TacticsManager.Group.GroupAzure:
                panelAzure.transform.localScale = enlargedScale;
                panelAzure.transform.SetAsLastSibling();
                panelAzure.transform.localPosition = panelPosition;
                panelPink.transform.localScale = defaultScale;
                panelPink.transform.localPosition = xCorrection;
                panelGreen.transform.localScale = defaultScale;
                panelYellow.transform.localScale = defaultScale;
                panelYellow.transform.localPosition = xCorrection * 3;
                break;
            case TacticsManager.Group.GroupPink:
                panelAzure.transform.localScale = defaultScale;
                panelAzure.transform.localPosition = Vector3.zero;
                panelPink.transform.localScale = enlargedScale;
                panelPink.transform.SetAsLastSibling();
                panelPink.transform.localPosition = panelPosition + xCorrection;
                panelGreen.transform.localScale = defaultScale;
                panelGreen.transform.localPosition = xCorrection * 2;
                panelYellow.transform.localScale = defaultScale;
                break;
            case TacticsManager.Group.GroupGreen:
                panelAzure.transform.localScale = defaultScale;
                panelPink.transform.localScale = defaultScale;
                panelPink.transform.localPosition = xCorrection;
                panelGreen.transform.localScale = enlargedScale;
                panelGreen.transform.SetAsLastSibling();
                panelGreen.transform.localPosition = panelPosition + xCorrection * 2;
                panelYellow.transform.localScale = defaultScale;
                panelYellow.transform.localPosition = xCorrection * 3;
                break;
            case TacticsManager.Group.GroupYellow:
                panelAzure.transform.localScale = defaultScale;
                panelAzure.transform.localPosition = Vector3.zero;
                panelPink.transform.localScale = defaultScale;
                panelGreen.transform.localScale = defaultScale;
                panelGreen.transform.localPosition = xCorrection * 2;
                panelYellow.transform.localScale = enlargedScale;
                panelYellow.transform.SetAsLastSibling();
                panelYellow.transform.localPosition = panelPosition + xCorrection * 3;

                break;
        }


    }

    private void OnPlayerDeath()
    {
        if ( playerCombatEventsManager != null )
        {
            playerCombatEventsManager.onDeath -= OnPlayerDeath;
        }

        player = GameObject.FindGameObjectWithTag( "Player" );

        if ( player != null )
        {
            tacticsManager = player.GetComponent<TacticsManager>();
            playerCombatEventsManager = player.GetComponent<CombatEventsManager>();

            if ( playerCombatEventsManager != null )
            {
                playerCombatEventsManager.onDeath += OnPlayerDeath;
            }
            else
            {
                Debug.LogError( "HUD cannot find player's CombatEventsManager" );
            }
        }
        else
        {
            Debug.LogError( "HUD cannot find player" );
        }
    }
}
