using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;
using UnityEngine.SceneManagement;

// TODO - implement reincarnation using events
public class Reincarnation : MonoBehaviour
{
    #region fields

    private static GameObject player;

    private bool playerIsReincarnated = false;

    private Coroutine reicarnationCR = null;

    //private GameObjectSearcher gameObjectSearcher;
    //private CinemachineFreeLook cinemachineFreeLook;
    //private GameObject virtualCamera;

    private Action<GameObject> OnReincarnation;

    #endregion

    #region methods

    void Start()
    {
        if(this.gameObject.tag == "Player") {
            player = gameObject;
        }
        
        //gameObjectSearcher = GameObject.FindGameObjectWithTag( "ChildrenSearcher" ).GetComponent<GameObjectSearcher>();
        //cinemachineFreeLook = GameObject.FindGameObjectWithTag( "ThirdPersonCamera" ).GetComponent<CinemachineFreeLook>();
        //virtualCamera = GameObject.FindGameObjectWithTag( "VirtualCameraLock" );
    }

    public void Reincarnate()
    {if (player != null) {
            // Disable controller
            Controller controller = player.GetComponent<Controller>();
            if (controller != null)
            {
                controller.enabled = false;
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
            player.GetComponent<Stats>().health = 4f;

            DemonBehaviour demonBehaviour = player.GetComponent<DemonBehaviour>();
            if(demonBehaviour != null) {
                /*
                GroupManager groupManager = demonBehaviour.groupBelongingTo.GetComponent<GroupManager>();

                // Removing the player from group
                if(groupManager != null) { 
                    groupManager.RemoveImp(player);    
                }
                */

                // Update group aggro
                GroupAggro groupAggro  = demonBehaviour.groupBelongingTo.GetComponent<GroupAggro>();
                if(groupAggro != null) { 
                    groupAggro.UpdateGroupAggro();            
                }

                // Update supporting units
                GroupSupport groupSupport = demonBehaviour.groupBelongingTo.GetComponent<GroupSupport>();
                if(groupSupport != null) { 
                    groupSupport.UpdateSupportingUnits();    
                }

                demonBehaviour.enabled = false;
            }

            NewCameraManager newCameraManager = Camera.main.GetComponent<NewCameraManager>();
            if ( newCameraManager )
            {
                newCameraManager.Player = player;
                newCameraManager.PlayerReincarnated();
            }

            Reincarnation reincarnation = player.GetComponent<Reincarnation>();
            if(reincarnation != null) { 
                reincarnation.enabled = true;    
            }

            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            if(playerInput != null) { 
                playerInput.enabled = true;    
            }
            
            Lancer lancer = player.GetComponent<Lancer>();
            if(lancer != null) { 
                lancer.enabled = true;    
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

            Controller controller = player.GetComponent<Controller>();
            if(controller != null) { 
                controller.enabled = true;    
            }

            DemonMovement demonMovement = player.GetComponent<DemonMovement>();
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

            // Reset Combat
            Combat playerCombat = player.GetComponent<Combat>();
            if (!player.GetComponent<Stats>().CombatIdle)
            {
                playerCombat.ResetCombat();
            }

            // Update allies list
            AlliesManager.Instance.ManagePlayerReincarnation(player);

            CombatEventsManager combatEventsManager = player.GetComponent<CombatEventsManager>();
            
            //RaiseOnReincarnation(player);

            Reincarnation newPlayerReincarnation = player.GetComponent<Reincarnation>();

            newPlayerReincarnation.RaiseOnReincarnation(player);

            // Activate groups in range detection
            // TODO - shouold this be activated after picking up the crown?
            Transform groupsInRangeDetector = player.transform.Find("GroupsInRangeDetector");

            if(groupsInRangeDetector != null) { 
                groupsInRangeDetector.gameObject.SetActive(true);    
            }
        }
    }

    /// <summary>
    /// Register a method to OnReincarnation event
    /// </summary>
    /// <param name="method">The method to register</param>
    public void RegisterOnReincarnation(Action<GameObject> method) { 
        OnReincarnation += method;     
    }

    /// <summary>
    /// Unregister a method to OnReincarnation event
    /// </summary>
    /// <param name="method">The method to unregister</param>
    public void UnregisterOnReincarnation(Action<GameObject> method) { 
        OnReincarnation -= method;    
    }

    private void RaiseOnReincarnation(GameObject player) { 
        if(OnReincarnation != null) { 
            OnReincarnation(player);    
        }
    }

    #endregion
}
