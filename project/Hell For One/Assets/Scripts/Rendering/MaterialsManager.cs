using ReincarnationSystem;
using UnityEngine;

namespace Rendering
{
    public class MaterialsManager : MonoBehaviour, IReincarnationObserver
    {
        private SkinnedMeshRenderer[] _renderers;

        private Material _defaultMaterial;

        private void Awake()
        {
            _renderers = this.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            _defaultMaterial = _renderers[0].material;
        }

        /// <summary>
        /// Change the material for every SkinnedMeshRenderer of this GameObject
        /// </summary>
        /// <param name="newMaterial">New material to use</param>
        public void ChangeMaterials(Material newMaterial)
        {
            foreach (SkinnedMeshRenderer renderer in _renderers)
            {
                renderer.material = newMaterial;
            }
        }

        /// <summary>
        /// Change the material for every SkinnedMeshRenderer of this GameObject to default
        /// </summary>
        public void SetDefaultMaterial()
        {
            foreach (SkinnedMeshRenderer renderer in _renderers)
            {
                renderer.material = _defaultMaterial;
            }
        }

        public void Reincarnate() => SetDefaultMaterial();
    }
}