using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWallsTrigger : MonoBehaviour
{
    private void OnEnable()
    {
        BattleEventsManager.onBossBattleExit += OpenInvisibleWalls;
        BattleEventsManager.onBattleExit += OpenInvisibleWalls;
        BattleEventsManager.onBossBattleEnter += CloseInvisibleWalls;
        BattleEventsManager.onBattleEnter += CloseInvisibleWalls;
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
}
