using CallToArmsSystem;
using UnityEngine;

public class CallToArmsParticlesBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject _effect;
    //private AudioSource _audioSource;

    private void Start()
    {
        //singleton needs start instead of onEnable
        CallToArmsManager.Instance.OnStartCallToArmsImpsSpawn += OnStartCallToArmsImpsSpawn;
    }

    private void OnDisable()
    {
        CallToArmsManager.Instance.OnStartCallToArmsImpsSpawn -= OnStartCallToArmsImpsSpawn;
    }

    private void OnStartCallToArmsImpsSpawn(Vector3 position)
    {
        Instantiate(_effect, position, Quaternion.identity);
    }
}