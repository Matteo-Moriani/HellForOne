using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineGroup : MonoBehaviour
{
    // comment for push

    [SerializeField]
    [Tooltip("The material to use to outline this group")]
    private Material outlineMaterial;

    [SerializeField]
    [Tooltip("The color to use to outline this group")]
    private Color color = Color.white;

    private Material defaultImpMaterial;
    private GroupBehaviour groupBehaviour;
    private bool isOutlined = false;

    private void Awake()
    {
        groupBehaviour = this.gameObject.GetComponent<GroupBehaviour>();
    }

    private void OnEnable()
    {
        GroupsInRangeDetector.RegisterOnMostRappresentedGroupChanged(OnMostRappresentedGroupChanged);
    }

    private void OnDisable()
    {
        GroupsInRangeDetector.UnregisterOnMostRappresentedGroupChanged(OnMostRappresentedGroupChanged);
    }

    private void OnMostRappresentedGroupChanged() { 
        if(groupBehaviour != null) {
            // New most rappresented group
            if(groupBehaviour.ThisGroupName == GroupsInRangeDetector.MostRappresentedGroupInRange) {
                isOutlined = true;

                if(outlineMaterial != null) {
                    // Set outline material for this group
                    outlineMaterial.SetColor("_OutlineColor",color);

                    // Assign new material
                    foreach (GameObject imp in groupBehaviour.demons)
                    {
                        if(imp != null) {
                            MaterialsManager materialsManager = imp.GetComponent<MaterialsManager>();

                            if (materialsManager != null)
                            {
                                // TODO - Optimize this
                                if (defaultImpMaterial == null)
                                {
                                    defaultImpMaterial = materialsManager.Renderers[0].material;
                                }

                                materialsManager.ChangeMaterials(outlineMaterial);
                            }
                            else
                            {
                                Debug.LogError(this.gameObject.name + " " + this.name + " cannot find " + imp.name + " MaterialsManager");
                            }
                        }
                    }
                }
                else {
                    Debug.LogError(this.gameObject.name + " " + this.name + " outlineMaterial not assigned");
                }    
            }
            // Last outlined group
            else {
                if (isOutlined) { 
                    isOutlined = false;

                    // Assign default material
                    foreach (GameObject imp in groupBehaviour.demons)
                    {
                        if(imp != null) {
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
        else { 
            Debug.LogError(this.gameObject.name + " " + this.name + " cannot find GroupBehaviour");    
        }        
    }
}
