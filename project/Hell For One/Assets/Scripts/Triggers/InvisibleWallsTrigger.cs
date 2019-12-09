using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWallsTrigger : MonoBehaviour
{
    private void OnEnable()
    {
        BattleEventsManager.onBossBattleExit += OpenInvisibleWalls;
        BattleEventsManager.onBossBattleExit += DisableBossFlames;
        BattleEventsManager.onBattleExit += OpenInvisibleWalls;
        BattleEventsManager.onBattleExit += DisableNormalFlames;
        BattleEventsManager.onBossBattleEnter += CloseInvisibleWalls;
        BattleEventsManager.onBossBattleEnter += EnableBossFlames;
        BattleEventsManager.onBattleEnter += CloseInvisibleWalls;
        BattleEventsManager.onBattleEnter += EnableNormalFlames;
    }

    public void OpenInvisibleWalls()
    {
        Physics.IgnoreLayerCollision( LayerMask.NameToLayer( "Player" ), LayerMask.NameToLayer( "InvisibleWalls" ), true );
        Physics.IgnoreLayerCollision( LayerMask.NameToLayer( "InvisibleWalls" ), LayerMask.NameToLayer( "Player" ), true );
    }

    public void CloseInvisibleWalls()
    {
        Physics.IgnoreLayerCollision( LayerMask.NameToLayer( "Player" ), LayerMask.NameToLayer( "InvisibleWalls" ), false );
        Physics.IgnoreLayerCollision( LayerMask.NameToLayer( "InvisibleWalls" ), LayerMask.NameToLayer( "Player" ), false );
    }

    public void EnableBossFlames()
    {
        foreach(GameObject go in GameObject.FindGameObjectsWithTag( "BossBattleFlames" ) )
        {
            go.GetComponent<ParticleSystem>().Play();
        }
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
