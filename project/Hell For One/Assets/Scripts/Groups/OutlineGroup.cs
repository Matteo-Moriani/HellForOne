using System.Collections;
using System.Collections.Generic;
using Groups;
using UnityEngine;

public class OutlineGroup : MonoBehaviour
{
    #region Fields

    [SerializeField]
    [Tooltip("The material to use to outline this group")]
    private Material outlineMaterial;

    [SerializeField]
    [Tooltip("The color to use to outline this group")]
    private Color color = Color.white;

    private GroupManager groupManager;
    private bool isOutlined = false;

    #endregion

    #region Unity methods

    private void Awake()
    {
        groupManager = this.gameObject.GetComponent<GroupManager>();
    }

    private void OnEnable()
    {
        GroupsInRangeDetector.RegisterOnMostRappresentedGroupChanged(OnMostRappresentedGroupChanged);

        groupManager.OnImpJoined += OnImpJoined;
    }

    private void OnDisable()
    {
        GroupsInRangeDetector.UnregisterOnMostRappresentedGroupChanged(OnMostRappresentedGroupChanged);
        
        groupManager.OnImpJoined -= OnImpJoined;

        outlineMaterial.SetColor("_OutlineColor", Color.white);
    }

    #endregion

    #region Event handlers

    private void OnMostRappresentedGroupChanged()
    {
        if (groupManager != null)
        {
            // New most rappresented group
            if (groupManager.ThisGroupName == GroupsInRangeDetector.MostRappresentedGroupInRange)
            {
                isOutlined = true;

                if (outlineMaterial != null)
                {
                    // Set outline material for this group
                    outlineMaterial.SetColor("_OutlineColor", color);

                    // Assign new material
                    foreach (Transform imp in groupManager.Imps)
                    {
                        if (imp != null)
                        {
                            MaterialsManager materialsManager = imp.GetComponent<MaterialsManager>();

                            if (materialsManager != null)
                            {
                                materialsManager.ChangeMaterials(outlineMaterial);
                            }
                            else
                            {
                                Debug.LogError(this.gameObject.name + " " + this.name + " cannot find " + imp.name + " MaterialsManager");
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError(this.gameObject.name + " " + this.name + " outlineMaterial not assigned");
                }
            }
            // Last outlined group
            else
            {
                if (isOutlined)
                {
                    isOutlined = false;

                    // Assign default material
                    foreach (Transform imp in groupManager.Imps)
                    {
                        if (imp != null)
                        {
                            MaterialsManager materialsManager = imp.GetComponent<MaterialsManager>();

                            if (materialsManager != null)
                            {
                                materialsManager.SetDefaultMaterial();
                            }
                            else
                            {
                                Debug.LogError(this.gameObject.name + " " + this.name + " cannot find " + imp.name + " MaterialsManager");
                            }
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogError(this.gameObject.name + " " + this.name + " cannot find GroupBehaviour");
        }
    }

    private void OnImpJoined(GroupManager sender, GameObject demon) { 
        if(groupManager.ThisGroupName == GroupsInRangeDetector.MostRappresentedGroupInRange) { 
            MaterialsManager materialsManager = demon.GetComponent<MaterialsManager>();
            
            if(materialsManager != null) {
                materialsManager.ChangeMaterials(outlineMaterial);    
            }
        }    
    }

    #endregion
}
