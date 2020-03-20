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
	
	[SerializeField] [Tooltip("Position offset for idleCombatManager GameObject")]
	private Vector3 combatSystemGameObjectsOffset = Vector3.zero;

	[SerializeField] 
	private float defaultScaleX = 0.5f;
	[SerializeField] 
	private float defaultScaleY = 1f;
	[SerializeField] 
	private float defaultScaleZ = 0.5f;
	
	private static string combatSystemLayer = "CombatSystem";
	
	#endregion

	#region Properties

	public float DefaultScaleX
	{
		get => defaultScaleX;
		private set => defaultScaleX = value;
	}

	public float DefaultScaleY
	{
		get => defaultScaleY;
		private set => defaultScaleY = value;
	}

	public float DefaultScaleZ
	{
		get => defaultScaleZ;
		private set => defaultScaleZ = value;
	}

	#endregion

	#region Methods

	public GameObject CreateCombatSystem_GO(Transform parent, string nameAndTag)
	{
		Vector3 gameObjectPosition = parent.position + combatSystemGameObjectsOffset;

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
		currentGameObject.transform.localScale = new Vector3(defaultScaleX, defaultScaleY, defaultScaleZ);

		// Set Layer
		currentGameObject.layer = LayerMask.NameToLayer(combatSystemLayer);

		// Rendering for testing
		MeshRenderer rend = currentGameObject.AddComponent<MeshRenderer>();

		// Mesh filter
		MeshFilter filter = currentGameObject.AddComponent<MeshFilter>();
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		filter.mesh = cube.GetComponent<MeshFilter>().mesh;
		Destroy(cube);
		GameObject.Destroy(cube);
		

		// Rigidbody
		Rigidbody rb = currentGameObject.AddComponent<Rigidbody>();
		rb.isKinematic = true;
		rb.useGravity = false;

		// Collider
		BoxCollider boxCollider = currentGameObject.AddComponent<BoxCollider>();
		boxCollider.isTrigger = true;

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
