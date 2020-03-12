using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsManager : MonoBehaviour
{
    private SkinnedMeshRenderer[] renderers;

    private Material defaultMaterial;

    private Reincarnation reincarnation;
 
    /// <summary>
    /// Renderers of this GameObject
    /// </summary>
    public SkinnedMeshRenderer[] Renderers { get => renderers; private set => renderers = value; }

    private void Awake()
    {
        reincarnation = this.transform.root.gameObject.GetComponent<Reincarnation>();
        Renderers = this.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        defaultMaterial = renderers[0].material;
    }

    private void OnEnable()
    {
        reincarnation.RegisterOnReincarnation(OnReincarnation);
    }

    private void OnDisable()
    {
        reincarnation.UnregisterOnReincarnation(OnReincarnation);
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

    private void OnReincarnation(GameObject player) { 
        SetDefaultMaterial();  
    }
}