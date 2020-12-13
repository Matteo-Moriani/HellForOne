using Animations;
using System.Collections;
using UnityEngine;

public class DeathParticlesBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject _deathEffect;
    private ParticleSystem[] _particleSystems;
    private SkinnedMeshRenderer[] _skinnedMeshRenderers;
    private MeshRenderer[] _meshRenderers;
    private Quaternion _fixedRotation;

    private void Awake()
    {
        _particleSystems = _deathEffect.GetComponentsInChildren<ParticleSystem>();

        _skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();

        _fixedRotation = new Quaternion(transform.rotation.x, 0f, transform.rotation.z, transform.rotation.w);
    }

    private void OnEnable()
    {
        GetComponent<AnimationEventsHooks>().OnDeathAnimationEnd += OnDeathAnimationEnd;
    }

    private void OnDisable()
    {
        GetComponent<AnimationEventsHooks>().OnDeathAnimationEnd -= OnDeathAnimationEnd;
    }

    private void OnDeathAnimationEnd()
    {
        foreach(ParticleSystem p in _particleSystems)
        {
            p.Play();
        }

        StartCoroutine(DisappearLater());
    }

    private IEnumerator DisappearLater()
    {
        yield return null;

        foreach(SkinnedMeshRenderer m in _skinnedMeshRenderers)
        {
            m.enabled = false;
        }
        foreach(MeshRenderer meshRenderer in _meshRenderers)
        {
            meshRenderer.enabled = false;
        }
    }
}