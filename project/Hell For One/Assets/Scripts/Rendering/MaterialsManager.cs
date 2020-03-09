using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsManager : MonoBehaviour
{
    private SkinnedMeshRenderer[] renderers;

    private Material defaultMaterial;

    private CombatEventsManager combatEventsManager;

    /// <summary>
    /// Renderers of this GameObject
    /// </summary>
    public SkinnedMeshRenderer[] Renderers { get => renderers; private set => renderers = value; }

    private void Awake()
    {
        combatEventsManager = this.gameObject.GetComponent<CombatEventsManager>();
        Renderers = this.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        defaultMaterial = renderers[0].material;
    }

    private void OnEnable()
    {
        combatEventsManager.onReincarnation += SetDefaultMaterial;
    }

    private void OnDisable()
    {
        combatEventsManager.onReincarnation += SetDefaultMaterial;
    }

    /// <summary>
    /// Change the material for every SkinnedMeshRenderer of this GameObject
    /// </summary>
    /// <param name="newMaterial">New material to use</param>
    public void ChangeMaterials(Material newMaterial)
    {
        foreach (SkinnedMeshRenderer renderer in Renderers)
        {
            renderer.material = newMaterial;
        }
    }

    /// <summary>
    /// Change the material for every SkinnedMeshRenderer of this GameObject to default
    /// </summary>
    public void SetDefaultMaterial()
    {
        foreach (SkinnedMeshRenderer renderer in Renderers)
        {
            renderer.material = defaultMaterial;
        }
    }
}