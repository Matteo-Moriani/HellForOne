using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsManager : MonoBehaviour
{
    private SkinnedMeshRenderer[] renderers;

    /// <summary>
    /// Renderers of this GameObject
    /// </summary>
    public SkinnedMeshRenderer[] Renderers { get => renderers; private set => renderers = value; }

    private void Awake()
    {
        Renderers = this.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    /// <summary>
    /// Change the material for every SkinnedMeshRenderer of this GameObject
    /// </summary>
    /// <param name="newMaterial">New material to use</param>
    public void ChangeMaterials(Material newMaterial) { 
        foreach(SkinnedMeshRenderer renderer in Renderers) { 
            renderer.material = newMaterial;    
        }
    }
}
