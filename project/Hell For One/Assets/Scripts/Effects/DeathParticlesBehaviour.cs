using Animations;
using System.Collections;
using UnityEngine;

public class DeathParticlesBehaviour : MonoBehaviour
{
    [SerializeField] private float waitBeforeDisappear;
    
    private ParticleSystem _particleSystem;
    private AudioSource _audioSource;
    private Quaternion _fixedRotation;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _audioSource = GetComponent<AudioSource>();

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
        //Debug.Log(transform.root.name + " OnDeathAnimationEnd");
        
        _particleSystem.Play();
        
        //Debug.Log(_particleSystem.isEmitting);
        //Debug.Log(_particleSystem.isPaused);
        //Debug.Log(_particleSystem.isPlaying);
        //Debug.Log(_particleSystem.isStopped);
        //Debug.Log(_particleSystem.IsAlive());
        
        _audioSource.Play();
        
        //Debug.Log(_particleSystem.isEmitting);
        //Debug.Log(_particleSystem.isPaused);
        //Debug.Log(_particleSystem.isPlaying);
        //Debug.Log(_particleSystem.isStopped);
        //Debug.Log(_particleSystem.IsAlive());

        StartCoroutine(DisappearLater());
    }

    private IEnumerator DisappearLater()
    {
        yield return new WaitForSeconds(waitBeforeDisappear);

        foreach(SkinnedMeshRenderer m in transform.root.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            m.enabled = false;
        }
        foreach(MeshRenderer meshRenderer in transform.root.GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.enabled = false;
        }
        
        //Debug.Log("Disappear");
        //Debug.Log(_particleSystem.isEmitting);
        //Debug.Log(_particleSystem.isPaused);
        //Debug.Log(_particleSystem.isPlaying);
        //Debug.Log(_particleSystem.isStopped);
        //Debug.Log(_particleSystem.IsAlive());
    }
}