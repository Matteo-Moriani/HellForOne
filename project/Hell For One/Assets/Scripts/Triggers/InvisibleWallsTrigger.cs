using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWallsTrigger : MonoBehaviour
{

    private GameObject flameCircle;
    private GameObject firewallMidBoss, arenaWallMidBoss;
    private GameObject firewallBoss, arenaWallBoss;

    private void Start()
    {
        flameCircle = GameObject.FindGameObjectWithTag( "FlameCircle" );
        firewallMidBoss = GameObject.Find( "firewallMidBoss" );
        arenaWallMidBoss = GameObject.Find( "ArenaWallMidBoss" );
        firewallBoss = GameObject.Find( "firewallBoss" );
        arenaWallBoss = GameObject.Find( "ArenaWallBoss" );
    }

    private void DisableWallsBoss()
    {
        firewallBoss.SetActive( false );
        arenaWallBoss.SetActive( false );
    }

    private void EnableWallBoss()
    {
        firewallBoss.SetActive( true );
        arenaWallBoss.SetActive( true );
    }

    private void DisableArenaWallMidBoss()
    {
        arenaWallMidBoss.SetActive( false );
    }

    private void DisableFirewall2()
    {
        firewallMidBoss.SetActive( false );
    }

    private void EnableFlameCircle()
    {
        flameCircle.SetActive(true);
    }

    private void DisableFlameCircle()
    {
        flameCircle.SetActive( false );
    }

    private void OnEnable()
    {
        BattleEventsManager.onBossBattleExit += OpenInvisibleWalls;
        BattleEventsManager.onBossBattleExit += DisableBossFlames;
        BattleEventsManager.onBossBattleExit += DisableWallsBoss;
        BattleEventsManager.onBossBattleExit += DisableFlameCircle;
        BattleEventsManager.onBattleExit += OpenInvisibleWalls;
        BattleEventsManager.onBattleExit += DisableFirewall2;
        BattleEventsManager.onBattleExit += DisableArenaWallMidBoss;
        BattleEventsManager.onBattleExit += DisableNormalFlames;
        BattleEventsManager.onBattleExit += LetDemonsPassFlames;
        BattleEventsManager.onBossBattleEnter += CloseInvisibleWalls;
        BattleEventsManager.onBossBattleEnter += EnableFlameCircle;
        BattleEventsManager.onBossBattleEnter += EnableBossFlames;
        BattleEventsManager.onBossBattleEnter += EnableWallBoss;
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
