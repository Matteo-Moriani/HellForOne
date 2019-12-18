using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitFeedback : MonoBehaviour
{
    [SerializeField]
    private Material[] finalBossMaterials;

    private Stats.Type type;

    private Renderer renderer;

    private Color startingColor;

    private CombatEventsManager combatEventsManager;

    private Coroutine blinkCR;

    private void Awake()
    {
        combatEventsManager = this.transform.root.gameObject.GetComponent<CombatEventsManager>();
    }

    private void OnEnable()
    {
        combatEventsManager.onBeenHit += OnBeenHIt;
        combatEventsManager.onDeath += OnDeath;
    }

    private void OnDisable()
    {
        combatEventsManager.onBeenHit -= OnBeenHIt;
        combatEventsManager.onDeath -= OnDeath;
    }

    private void Start()
    {
        renderer = this.GetComponent<Renderer>();

        startingColor = renderer.material.GetColor("_EmissiveColor");
        type = this.transform.root.GetComponent<Stats>().type;
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
        if(this.transform.root.gameObject.name == "Boss") {
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
            renderer.material.SetColor("_EmissiveColor", Color.red);

            yield return new WaitForSeconds(0.1f);

            // Maybe we can use Color.black instead of this
            renderer.material.SetColor("_EmissiveColor", startingColor);

            blinkCR = null;
        }
    }

    private void OnDeath() {
        // Boss has strange material setup, so we need a different approach.
        if (this.transform.root.gameObject.name == "Boss") {
            foreach(Material material in finalBossMaterials) { 
                material.SetColor("_EmissiveColor",Color.black);    
            }
        }
        // For everyone else this should be ok
        else
        {
            // Maybe we can use Color.black instead of this
            renderer.material.SetColor("_EmissiveColor", startingColor);
        }    
    }
}
