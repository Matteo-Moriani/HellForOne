using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitFeedback : MonoBehaviour
{
    /*
    [SerializeField]
    private Material[] finalBossMaterials;

    private Stats.Type type;

    private Renderer bossRenderer;

    private Color startingColor;

    private CombatEventsManager combatEventsManager;

    private Coroutine blinkCR;

    private void Awake()
    {
        combatEventsManager = transform.root.gameObject.GetComponent<CombatEventsManager>();
    }

    private void OnEnable()
    {
        combatEventsManager.onBeenHit += OnBeenHit;
        combatEventsManager.onDeath += OnDeath;
    }

    private void OnDisable()
    {
        combatEventsManager.onBeenHit -= OnBeenHit;
        combatEventsManager.onDeath -= OnDeath;
    }

    private void Start()
    {
        bossRenderer = GetComponent<Renderer>();

        if(bossRenderer) {
            startingColor = bossRenderer.material.GetColor("_EmissiveColor");
            type = transform.root.GetComponent<Stats>().ThisUnitType;
        }
    }

    private void OnBeenHit(Stats attackerStats)
    {
        // We blink only if receiving damage from the player
        if (attackerStats.ThisUnitType == Stats.Type.Player && blinkCR == null)
        {
            blinkCR = StartCoroutine(BlinkCoroutine(attackerStats));
        }
    }

    private IEnumerator BlinkCoroutine(Stats attackerStats)
    { 
        if(transform.root.gameObject.name == "Boss") {
            foreach (Material material in finalBossMaterials)
            {
                material.SetColor("_EmissiveColor", Color.red);
            }

            yield return new WaitForSeconds(0.1f);

            foreach (Material material in finalBossMaterials)
            {
                // Black means no emission
                material.SetColor("_EmissiveColor", Color.black);
            }

            blinkCR = null;
        }
        else {
            bossRenderer.material.SetColor("_EmissiveColor", Color.red);

            yield return new WaitForSeconds(0.1f);

            // Maybe we can use Color.black instead of this
            bossRenderer.material.SetColor("_EmissiveColor", Color.black);

            blinkCR = null;
        }
    }

    private void OnDeath() {
        // Boss has strange material setup, so we need a different approach.
        if (transform.root.gameObject.name == "Boss") {
            foreach(Material material in finalBossMaterials) { 
                material.SetColor("_EmissiveColor",Color.black);    
            }
        }
        // For everyone else this should be ok
        else
        {
            // Maybe we can use Color.black instead of this
            bossRenderer.material.SetColor("_EmissiveColor", Color.black);
        }    
    }
    */
}
