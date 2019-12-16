using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitFeedback : MonoBehaviour
{
    private Renderer renderer;

    private CombatEventsManager combatEventsManager;

    private Coroutine blinkCR;

    private void Awake()
    {
        combatEventsManager = this.transform.root.gameObject.GetComponent<CombatEventsManager>();
    }

    private void OnEnable()
    {
        combatEventsManager.onBeenHit += OnBeenHIt;
    }

    private void OnDisable()
    {
        combatEventsManager.onBeenHit -= OnBeenHIt;
    }

    private void Start()
    {
        renderer = this.GetComponent<Renderer>();
    }

    private void Update()
    {
       //float i = renderer.material.GetFloat("_Surfacetype");
       //Debug.Log(i);
       Debug.Log(renderer.material.IsKeywordEnabled("_SURFACE_TYPE_TRANSPARENT"));
    }

    private void OnBeenHIt(Stats attackerStats)
    {
        // We blink only if receiving damage from the player
        if (attackerStats.type == Stats.Type.Player && blinkCR == null)
        {
            blinkCR = StartCoroutine(BlinkCoroutine(attackerStats));
        }
    }

    private IEnumerator BlinkCoroutine(Stats attackerStats)
    {
        Color startingColor = renderer.material.GetColor("_EmissiveColor");
      
        renderer.material.SetColor("_EmissiveColor", Color.red);
        
        yield return new WaitForSeconds(0.1f);

        renderer.material.SetColor("_EmissiveColor", startingColor);

        blinkCR = null;
    }
}
