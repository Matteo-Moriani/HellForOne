using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWallsTrigger : MonoBehaviour
{

    //private GameObject flameCircle;
    private GameObject firewallMidBossEnter, firewallMidBossExit, arenaWallMidBossEnter, arenaWallMidBossExit;
    private GameObject firewallBoss, arenaWallBoss;

    private void Start()
    {
        //flameCircle = GameObject.FindGameObjectWithTag( "FlameCircle" );
        firewallMidBossEnter = GameObject.Find( "firewallMidBossEnter" );
        firewallMidBossExit = GameObject.Find( "firewallMidBossExit" );
        arenaWallMidBossEnter = GameObject.Find( "ArenaWallMidBossEnter" );
        arenaWallMidBossExit = GameObject.Find( "ArenaWallMidBossExit" );
        firewallBoss = GameObject.Find( "firewallBoss" );
        arenaWallBoss = GameObject.Find( "ArenaWallBoss" );
    }

    private void DisableWallsBoss()
    {
        firewallBoss.SetActive( false );

        foreach ( ParticleSystem ps in firewallMidBossEnter.GetComponentsInChildren<ParticleSystem>() )
        {
            ps.Stop();
        }

        arenaWallBoss.SetActive( false );
    }

    private void EnableWallBoss()
    {
        firewallBoss.SetActive( true );

        foreach ( ParticleSystem ps in firewallBoss.GetComponentsInChildren<ParticleSystem>() )
        {
            ps.Play();
        }

        arenaWallBoss.SetActive( true );
    }

    private void EnableArenaWallMidBoss()
    {
        arenaWallMidBossEnter.SetActive( true );
        arenaWallMidBossExit.SetActive( true );
    }

    private void DisableArenaWallMidBoss()
    {
        arenaWallMidBossEnter.SetActive( false );
        arenaWallMidBossExit.SetActive( false );
    }

    private void EnableFirewallMidBoss()
    {
        firewallMidBossEnter.SetActive( true );
        firewallMidBossEnter.tag = "InvisibleWalls";

        foreach ( ParticleSystem ps in firewallMidBossEnter.GetComponentsInChildren<ParticleSystem>() )
        {
            ps.Play();
        }

        firewallMidBossExit.SetActive( true );
        firewallMidBossExit.tag = "InvisibleWalls";

        foreach ( ParticleSystem ps in firewallMidBossExit.GetComponentsInChildren<ParticleSystem>() )
        {
            ps.Play();
        }
    }

    private void DisableFirewallMidBoss()
    {
        foreach ( ParticleSystem ps in firewallMidBossEnter.GetComponentsInChildren<ParticleSystem>() )
        {
            ps.Stop();
        }

        firewallMidBossEnter.SetActive( false );

        foreach ( ParticleSystem ps in firewallMidBossExit.GetComponentsInChildren<ParticleSystem>() )
        {
            ps.Stop();
        }

        firewallMidBossExit.SetActive( false );
    }

    private void EnableFlameCircle()
    {
        //flameCircle.SetActive(true);
    }

    private void DisableFlameCircle()
    {
        //flameCircle.SetActive( false );
    }

    private void OnEnable()
    {
        BattleEventsManager.onBossBattleExit += OpenInvisibleWalls;
        BattleEventsManager.onBossBattleExit += DisableBossFlames;
        BattleEventsManager.onBossBattleExit += DisableWallsBoss;
        BattleEventsManager.onBossBattleExit += DisableFlameCircle;
        BattleEventsManager.onBossBattleExit += DisableFirewallMidBoss;
        BattleEventsManager.onBattleExit += OpenInvisibleWalls;
        BattleEventsManager.onBattleExit += DisableFirewallMidBoss;
        BattleEventsManager.onBattleExit += DisableArenaWallMidBoss;
        BattleEventsManager.onBattleExit += DisableNormalFlames;
        BattleEventsManager.onBattleExit += LetDemonsPassFlames;
        BattleEventsManager.onBossBattleEnter += CloseInvisibleWalls;
        BattleEventsManager.onBossBattleEnter += EnableArenaWallMidBoss;
        BattleEventsManager.onBossBattleEnter += EnableFlameCircle;
        BattleEventsManager.onBossBattleEnter += EnableBossFlames;
        BattleEventsManager.onBossBattleEnter += EnableWallBoss;
        BattleEventsManager.onBossBattleEnter += EnableFirewallMidBoss;
        BattleEventsManager.onBattleEnter += CloseInvisibleWalls;
        BattleEventsManager.onBattleEnter += DisableFlameCircle;
        BattleEventsManager.onBattleEnter += EnableNormalFlames;
        DisableNormalFlames();
        DisableBossFlames();
    }

    public void OpenInvisibleWalls()
    {
        Physics.IgnoreLayerCollision( LayerMask.NameToLayer( "Player" ), LayerMask.NameToLayer( "InvisibleWalls" ), true );
        Physics.IgnoreLayerCollision( LayerMask.NameToLayer( "InvisibleWalls" ), LayerMask.NameToLayer( "Player" ), true );
        Physics.IgnoreLayerCollision( LayerMask.NameToLayer( "Demons" ), LayerMask.NameToLayer( "InvisibleWalls" ), true );
        Physics.IgnoreLayerCollision( LayerMask.NameToLayer( "InvisibleWalls" ), LayerMask.NameToLayer( "Demons" ), true );
    }

    public void CloseInvisibleWalls()
    {
        Physics.IgnoreLayerCollision( LayerMask.NameToLayer( "Player" ), LayerMask.NameToLayer( "InvisibleWalls" ), false );
        Physics.IgnoreLayerCollision( LayerMask.NameToLayer( "InvisibleWalls" ), LayerMask.NameToLayer( "Player" ), false );
        Physics.IgnoreLayerCollision( LayerMask.NameToLayer( "Demons" ), LayerMask.NameToLayer( "InvisibleWalls" ), false );
        Physics.IgnoreLayerCollision( LayerMask.NameToLayer( "InvisibleWalls" ), LayerMask.NameToLayer( "Demons" ), false );
    }

    public void EnableBossFlames()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag( "BossBattleFlames" ) )
        {
            go.GetComponent<ParticleSystem>().Play();
        }
    }

    public void LetDemonsPassFlames()
    {
        Physics.IgnoreLayerCollision( LayerMask.NameToLayer( "Demons" ), LayerMask.NameToLayer( "InvisibleWalls" ), true );
        Physics.IgnoreLayerCollision( LayerMask.NameToLayer( "InvisibleWalls" ), LayerMask.NameToLayer( "Demons" ), true );
    }

    public void DisableBossFlames()
    {
        foreach ( GameObject go in GameObject.FindGameObjectsWithTag( "BossBattleFlames" ) )
        {
            go.GetComponent<ParticleSystem>().Stop();
        }
    }

    public void EnableNormalFlames()
    {
        foreach ( GameObject go in GameObject.FindGameObjectsWithTag( "NormalBattleFlames" ) )
        {
            go.GetComponent<ParticleSystem>().Play();
        }
    }

    public void DisableNormalFlames()
    {
        foreach ( GameObject go in GameObject.FindGameObjectsWithTag( "NormalBattleFlames" ) )
        {
            go.GetComponent<ParticleSystem>().Stop();
        }
    }
}
