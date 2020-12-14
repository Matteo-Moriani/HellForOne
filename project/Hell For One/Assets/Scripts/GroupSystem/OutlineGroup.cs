using System.Linq;
using Rendering;
using UnityEngine;

namespace GroupSystem
{
    public class OutlineGroup : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        [Tooltip("The material to use to outline this group")]
        private Material outlineMaterial;

        [SerializeField]
        [Tooltip("The color to use to outline this group")]
        private Color color = Color.white;

        private GroupManager _groupManager;
        private bool _isOutlined = false;

        #endregion

        #region Unity methods

        private void Awake()
        {
            _groupManager = this.gameObject.GetComponent<GroupManager>();
        }

        private void OnEnable()
        {
            GroupsInRangeDetector.OnMostRepresentedGroupChanged += OnMostRepresentedGroupChanged;

            _groupManager.OnImpJoined += OnImpJoined;
        }

        private void OnDisable()
        {
            GroupsInRangeDetector.OnMostRepresentedGroupChanged -= OnMostRepresentedGroupChanged;
        
            _groupManager.OnImpJoined -= OnImpJoined;

            outlineMaterial.SetColor("_OutlineColor", Color.white);
        }

        #endregion

        #region Event handlers

        private void OnMostRepresentedGroupChanged(GroupManager.Group newGroup)
        {
            if (_groupManager.ThisGroupName == newGroup)
            {
                _isOutlined = true;
                
                // Set outline material for this group
                outlineMaterial.SetColor("_OutlineColor", color);

                // Assign new material
                foreach (var materialsManager in _groupManager.Imps.Keys.Select(imp => imp.GetComponent<MaterialsManager>()))
                    materialsManager.ChangeMaterials(outlineMaterial);
            }
            // Last outlined group
            else
            {
                if (!_isOutlined) return;
                
                _isOutlined = false;
                
                foreach (MaterialsManager materialsManager in _groupManager.Imps.Keys.Select(imp => imp.GetComponent<MaterialsManager>()))
                    materialsManager.SetDefaultMaterial();
            }
        }

        private void OnImpJoined(GroupManager sender, GameObject demon)
        {
            if (_groupManager.ThisGroupName != GroupsInRangeDetector.MostRepresentedGroupInRange) return;
            
            MaterialsManager materialsManager = demon.GetComponent<MaterialsManager>();
            
            if(materialsManager != null) {
                materialsManager.ChangeMaterials(outlineMaterial);    
            }
        }

        #endregion
    }
}
