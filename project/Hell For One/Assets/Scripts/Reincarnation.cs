using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;
using FactoryBasedCombatSystem;
using UnityEngine.SceneManagement;

// TODO - implement reincarnation using events
// TODO - when events implementation is not possible, add/remove needed component in reincarnation 
public class Reincarnation : MonoBehaviour
{
    #region fields

    private static GameObject player;

    private bool playerIsReincarnated = false;

    private Coroutine reicarnationCR = null;

    #endregion

    #region Delegates and events

    public delegate void OnReincarnation(GameObject newPlayer);
    public event OnReincarnation onReincarnation;

    public delegate void OnLateReincarnation(GameObject newPlayer);
    public event OnLateReincarnation onLateReincarnation;

    public delegate void OnPlayerReincarnated(GameObject newPlayer);
    public static event OnPlayerReincarnated onPlayerReincarnated;
    
    #region Methods

    private void RaiseOnReincarnation(GameObject newPlayer)
    {
        onReincarnation?.Invoke(newPlayer);
    }

    private void RaiseOnLateReincarnation(GameObject newPlayer)
    {
        onLateReincarnation?.Invoke(newPlayer);
    }

    private void RaiseOnPlayerReincarnated(GameObject newPlayer)
    {
        onPlayerReincarnated?.Invoke(newPlayer);
    }

    #endregion
        
    #endregion

    #region methods

    void Start()
    {
        if(this.gameObject.tag == "Player") {
            player = gameObject;
        }
    }

    public void Reincarnate()
    {if (player != null) {
            // Disable controller
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }

            // Disable Tactics Manager
            //TacticsManager tacticsManager = player.GetComponent<TacticsManager>();
            //if (tacticsManager != null)
            //{
            //    tacticsManager.enabled = false;
            //}

            // Disable Dash
            Dash dash = player.GetComponent<Dash>();
            if (dash != null)
            {
                dash.enabled = false;
            }

            // Disable Combat
            Combat combat = player.GetComponent<Combat>();
            if (combat != null)
            {
                combat.enabled = false;
            }

            // Flag this player as dead player
            player.tag = "DeadPlayer";

            // Manage the camera
            CameraManager cameraManager = Camera.main.GetComponent<CameraManager>();
            if (cameraManager != null)
            {
                cameraManager.player = null;
            }

            PlayerScriptedMovements playerScriptedMovements = player.GetComponent<PlayerScriptedMovements>();
            if(playerScriptedMovements != null) {
                playerScriptedMovements.enabled = false;
            }

            // Dectivate groups in range detection
            Transform groupsInRangeDetector = player.transform.Find("GroupsInRangeDetector");

            if (groupsInRangeDetector != null)
            {
                groupsInRangeDetector.gameObject.SetActive(false);
            }

            // Start reicarnation coroutine
            if (reicarnationCR == null)
            {
                reicarnationCR = StartCoroutine(ReincarnationCoroutine());
            }
        }
        else { 
            //Debug.LogError("Reicarnation cannot find Player");

            // Start reicarnation coroutine
            if (reicarnationCR == null)
            {
                reicarnationCR = StartCoroutine(ReincarnationCoroutine());
            }
        }
    }

    private IEnumerator ReincarnationCoroutine()
    {
        float reicarnationTimer = 0f;

        while (!playerIsReincarnated && reicarnationTimer < 2.0f)
        {
            if (AlliesManager.Instance.AlliesList.Count > 0)
            {
                player = AlliesManager.Instance.AlliesList[UnityEngine.Random.Range(0, AlliesManager.Instance.AlliesList.Count - 1)];

                if (player != null)
                {
                    playerIsReincarnated = true;

                    ReincarnatePlayer(player);
                }
            }

            reicarnationTimer += Time.deltaTime;

            yield return null;
        }            
    }

    private void ReincarnatePlayer(GameObject player)
    {
        if (player != null)
        {
            player.tag = "Player";
            player.layer = LayerMask.NameToLayer( "Player" );
            
            NewCameraManager newCameraManager = Camera.main.GetComponent<NewCameraManager>();
            if ( newCameraManager )
            {
                newCameraManager.Player = player;
                newCameraManager.PlayerReincarnated();
            }

            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            if(playerInput != null) { 
                playerInput.enabled = true;    
            }
            
            ProjectileCaster projectileCaster = player.GetComponent<ProjectileCaster>();
            if(projectileCaster != null) { 
                projectileCaster.enabled = true;    
            }

            NavMeshAgent navMeshAgent = player.GetComponent<NavMeshAgent>();
            if(navMeshAgent != null) {
                navMeshAgent.enabled = false;
            }

            Rigidbody rigidbody = player.GetComponent<Rigidbody>();
            if(rigidbody != null) {
                rigidbody.useGravity = true;
            }

            Dash dash = player.GetComponent<Dash>();
            if(dash != null) { 
                dash.enabled = true;    
            }

            PlayerController playerController = player.GetComponent<PlayerController>();
            if(playerController != null) { 
                playerController.enabled = true;    
            }

            AllyImpMovement demonMovement = player.GetComponent<AllyImpMovement>();
            if(demonMovement != null) { 
                demonMovement.enabled = false;    
            }

            NavMeshObstacle navMeshObstacle = player.GetComponent<NavMeshObstacle>();
            if(navMeshObstacle != null) { 
                navMeshObstacle.enabled = true;    
            }

            PlayerScriptedMovements playerScriptedMovements = player.GetComponent<PlayerScriptedMovements>();
            if(playerScriptedMovements != null) {
                playerScriptedMovements.enabled = true;
            }

            ChildrenObjectsManager childrenObjectsManager = player.GetComponent<ChildrenObjectsManager>();
            if(childrenObjectsManager != null) { 
                childrenObjectsManager.DeactivateCircle();    
            }
           
            // Set rigidbody
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if(rb != null) { 
                rb.useGravity = true;
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }
            
            // Update allies list
            AlliesManager.Instance.ManagePlayerReincarnation(player);

            CombatEventsManager combatEventsManager = player.GetComponent<CombatEventsManager>();
            
            //RaiseOnReincarnation(player);

            Reincarnation newPlayerReincarnation = player.GetComponent<Reincarnation>();

            // First handlers are called (stats or indipendent stuff)
            newPlayerReincarnation.RaiseOnReincarnation(player);
            
            // We add components specific to the Player
            player.AddComponent<ImpMana>();
            
            // TODO - this is used when we need to access Stats.type, so it's better to create an event in stats to call when Stats.type changes
            // Dipendent handlers
            newPlayerReincarnation.RaiseOnLateReincarnation(player);
            
            // Notify behaviours that player is changed
            RaiseOnPlayerReincarnated(player);

            // Activate groups in range detection
            // TODO - shouold this be activated after picking up the crown?
            Transform groupsInRangeDetector = player.transform.Find("GroupsInRangeDetector");

            if(groupsInRangeDetector != null) { 
                groupsInRangeDetector.gameObject.SetActive(true);    
            }
        }
    }

    #endregion
}
