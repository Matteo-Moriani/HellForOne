using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobBlink : MonoBehaviour
{
    SkinnedMeshRenderer[] renderers;
    //CombatEventsManager combatEventsManager;

    Coroutine blinkCR;

    [SerializeField]
    private Material blinkMaterial;
    [SerializeField]
    private Material baseMaterial;

    [SerializeField]
    private float blinkDuration = 0.1f;

    // private void Awake()
    // {
    //     combatEventsManager = this.GetComponent<CombatEventsManager>();
    // }
    //
    // private void OnEnable()
    // {
    //     combatEventsManager.onBeenHit += OnBeenHit;
    // }
    //
    // private void OnDisable()
    // {
    //     combatEventsManager.onBeenHit -= OnBeenHit;
    // }

    private void Start()
    {
        renderers = this.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    private void OnBeenHit(Stats attackerStats)
    {
        if(attackerStats.ThisUnitType == Stats.Type.Player) {
            if (blinkCR == null)
            {
                blinkCR = StartCoroutine(BlinkCoroutine());
            }
        }
    }

    private IEnumerator BlinkCoroutine()
    {
        foreach (SkinnedMeshRenderer rend in renderers)
        {
            rend.material = blinkMaterial;
        }

        yield return new WaitForSeconds(blinkDuration);

        foreach (SkinnedMeshRenderer rend in renderers)
        {
            rend.material = baseMaterial;
        }

        blinkCR = null;
    }
}
