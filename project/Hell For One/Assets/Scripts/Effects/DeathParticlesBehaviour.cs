using Animations;
using System.Collections;
using UnityEngine;

public class DeathParticlesBehaviour : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private AudioSource _audioSource;
    private SkinnedMeshRenderer[] _skinnedMeshRenderers;
    private MeshRenderer[] _meshRenderers;
    private Quaternion _fixedRotation;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _audioSource = GetComponent<AudioSource>();

        _skinnedMeshRenderers = transform.root.GetComponentsInChildren<SkinnedMeshRenderer>();
        _meshRenderers = transform.root.GetComponentsInChildren<MeshRenderer>();

        _fixedRotation = new Quaternion(transform.rotation.x, 0f, transform.rotation.z, transform.rotation.w);
    }

    private void OnEnable()
    {
        transform.root.GetComponent<AnimationEventsHooks>().OnDeathAnimationEnd += OnDeathAnimationEnd;
    }

    private void OnDisable()
    {
        transform.root.GetComponent<AnimationEventsHooks>().OnDeathAnimationEnd -= OnDeathAnimationEnd;
    }

    private void OnDeathAnimationEnd()
    {
        _particleSystem.Play();
        _audioSource.Play();

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