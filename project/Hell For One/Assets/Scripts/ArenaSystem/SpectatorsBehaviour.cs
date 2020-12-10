using ArenaSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorsBehaviour : MonoBehaviour, IArenaObserver
{
    private SkinnedMeshRenderer[] _meshRenderers;
    
    private void Awake()
    {
        _meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    private void Start()
    {
        foreach(SkinnedMeshRenderer s in _meshRenderers)
        {
            s.enabled = false;
        }
    }

    public void EnterBattle(ArenaManager subject)
    {

    }

    public void ExitBattle(ArenaManager subject)
    {
        foreach(SkinnedMeshRenderer s in _meshRenderers)
        {
            s.enabled = false;
        }
    }

    public void PrepareBattle(ArenaManager subject)
    {
        foreach(SkinnedMeshRenderer s in _meshRenderers)
        {
            s.enabled = true;
        }
        subject.NotifyBattlePrepared(this);
    }
}
