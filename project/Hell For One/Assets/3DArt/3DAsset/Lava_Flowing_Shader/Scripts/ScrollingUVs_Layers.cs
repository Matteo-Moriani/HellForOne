using UnityEngine;
using System.Collections;

public class ScrollingUVs_Layers : MonoBehaviour 
{
	[SerializeField]
	[Tooltip("The material to use to animate lava")]
	private Material lavaAnimationMaterial;

	[SerializeField]
	[Tooltip("Speed of the animation, restart game to see changes")]
	private float xAanimationRate = 0.25f;

	private string textureName = "_MainTex";
	
	private Vector2 defaultTextureOffset;

	private Renderer[] renderers;

	private Vector2 uvOffset = Vector2.zero;

	private void Awake()
	{
		renderers = this.gameObject.GetComponentsInChildren<Renderer>();
		if(lavaAnimationMaterial != null) {
			defaultTextureOffset = lavaAnimationMaterial.GetTextureOffset(textureName);
		}
		else {
			ShowMaterialNotAssignedError();
		}
	}

	private void Start()
	{
		if(lavaAnimationMaterial != null) {
			foreach (Renderer rend in renderers)
			{
				if (rend != null)
				{
					rend.material = lavaAnimationMaterial;
				}
			}
		}
		else {
			ShowMaterialNotAssignedError();
		}

		renderers = null;
	}

	void LateUpdate() 
	{
		uvOffset += (new Vector2(xAanimationRate,0.0f) * Time.deltaTime);

		if (lavaAnimationMaterial != null) { 
			lavaAnimationMaterial.SetTextureOffset(textureName,uvOffset);
		}
		else { 
			ShowMaterialNotAssignedError();	
		}
	}

	private void ShowMaterialNotAssignedError() {
		Debug.LogError(this.name + " " + this.gameObject.name + " lavaAnimationMaterial not assigned");
	}
}