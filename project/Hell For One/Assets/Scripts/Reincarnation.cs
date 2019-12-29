using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;
using UnityEngine.SceneManagement;

public class Reincarnation : MonoBehaviour
{
    #region fields

    private static GameObject player;

    private bool playerIsReincarnated = false;

    private Coroutine reicarnationCR = null;

    private GameObjectSearcher gameObjectSearcher;
    private CinemachineFreeLook cinemachineFreeLook;
    private GameObject virtualCamera;

    #endregion

    #region methods

    void Start()
    {
        player = gameObject;
        gameObjectSearcher = GameObject.FindGameObjectWithTag( "ChildrenSearcher" ).GetComponent<GameObjectSearcher>();
        cinemachineFreeLook = GameObject.FindGameObjectWithTag( "ThirdPersonCamera" ).GetComponent<CinemachineFreeLook>();
        virtualCamera = GameObject.FindGameObjectWithTag( "VirtualCameraLock" );
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
            TacticsManager tacticsManager = player.GetComponent<TacticsManager>();
            if (tacticsManager != null)
            {
                tacticsManager.enabled = false;
            }

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

            // Start reicarnation coroutine
            if (reicarnationCR == null)
            {
                reicarnationCR = StartCoroutine(ReincarnationCoroutine());
            }
        }
        else { 
            Debug.LogError("Reicarnation cannot find Player");

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
                player = AlliesManager.Instance.AlliesList[Random.Range(0, AlliesManager.Instance.AlliesList.Count - 1)];

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

            DemonBehaviour demonBehaviour = player.GetComponent<DemonBehaviour>();
            if(demonBehaviour != null) {
                GroupBehaviour groupBehaviour = demonBehaviour.groupBelongingTo.GetComponent<GroupBehaviour>();

                // Remmoving the player from group
                if (groupBehaviour != null)
                {
                    int playerIndex = System.Array.IndexOf(groupBehaviour.demons, player);
                    groupBehaviour.demons[playerIndex] = null;
                    groupBehaviour.SetDemonsNumber(groupBehaviour.GetDemonsNumber() - 1);
                }

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

            //NavMeshAgent navMeshAgent = player.GetComponent<NavMeshAgent>();
            //if(navMeshAgent != null) { 
            //    navMeshAgent.enabled = false;    
            //}

            Stats stats = player.GetComponent<Stats>();
            if(stats != null) { 
                stats.type = Stats.Type.Player;    
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
            
            if(combatEventsManager != null) { 
                combatEventsManager.RaiseOnReincarnation();    
            }
        }
    }

    #endregion
}
