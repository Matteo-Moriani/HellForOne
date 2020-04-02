using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CombatSystemManager : MonoBehaviour
{
	#region Fields

	// TODO - Remove materials after testing
	[SerializeField]
	public Material idleMaterial;
	[SerializeField]
	public Material normalAttackMaterial;
	
	//[SerializeField] [Tooltip("Position offset for idleCombatManager GameObject")]
	//private Vector3 combatSystemGameObjectsOffset = Vector3.zero;

	[SerializeField] private float defaultCollidersScale = 1.0f;
	
	private static string combatSystemLayer = "CombatSystem";
	
	#endregion

	#region Methods

	public GameObject CreateCombatSystem_GO(Transform parent, string nameAndTag)
	{
		Vector3 gameObjectPosition = parent.position + Vector3.up * defaultCollidersScale / 2;

		GameObject currentGameObject = new GameObject();

		currentGameObject.transform.position = gameObjectPosition;
		currentGameObject.transform.parent = parent;

		// Reset Rotation
		currentGameObject.transform.localEulerAngles = Vector3.zero;

		if (UnityEditorInternal.InternalEditorUtility.tags.Contains(nameAndTag))
		{
			currentGameObject.name = nameAndTag;
			currentGameObject.tag = nameAndTag;
		}
		else
		{
			Debug.LogError(parent.gameObject.name + " " + parent.root.name + " is trying to set [" + nameAndTag +
			               "] as tag but tag manager does not contain it");
		}

		return currentGameObject;
	}

	public GameObject CreateCombatSystemCollider_GO(Transform parent, string nameAndTag)
	{
		GameObject currentGameObject = new GameObject();
		currentGameObject.transform.position = parent.position;
		currentGameObject.transform.parent = parent;

		// Reset Rotation
		currentGameObject.transform.localEulerAngles = Vector3.zero;

		// Set default scale
		currentGameObject.transform.localScale = new Vector3(defaultCollidersScale, defaultCollidersScale, defaultCollidersScale);

		// Set Layer
		currentGameObject.layer = LayerMask.NameToLayer(combatSystemLayer);

		// Rendering for testing
		MeshRenderer rend = currentGameObject.AddComponent<MeshRenderer>();

		// TODO - Remove mesh filter after testing.
		// Mesh filter
		MeshFilter filter = currentGameObject.AddComponent<MeshFilter>();
		GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		filter.mesh = sphere.GetComponent<MeshFilter>().mesh;
		Destroy(sphere);
		GameObject.Destroy(sphere);
		

		// Rigidbody
		//Rigidbody rb = currentGameObject.AddComponent<Rigidbody>();
		//rb.isKinematic = true;
		//rb.useGravity = false;

		// Collider
		SphereCollider sphereCollider = currentGameObject.AddComponent<SphereCollider>();
		sphereCollider.isTrigger = true;

		if (UnityEditorInternal.InternalEditorUtility.tags.Contains(nameAndTag))
		{
			currentGameObject.name = nameAndTag;
			currentGameObject.tag = nameAndTag;
		}
		else
		{
			Debug.LogError(parent.gameObject.name + " " + parent.root.name + " is trying to set [" + nameAndTag +
			               "] as tag but tag manager does not contain it");
		}

		return currentGameObject;
	}

	#endregion
}
