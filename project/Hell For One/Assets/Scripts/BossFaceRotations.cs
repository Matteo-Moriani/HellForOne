using AI.Boss;
using UnityEngine;

public class BossFaceRotations : MonoBehaviour
{
    private Vector3 cameraPosition, rotateDirection;
    private bool bossFaceActive = false;
    private Quaternion lookRotation;

    public void BossFaceON()
    {
        GetComponent<Canvas>().enabled = true;
        bossFaceActive = true;
    }

    public void BossFaceOFF()
    {
        GetComponent<Canvas>().enabled = false;
        bossFaceActive = false;
    }

    private void OnEnable()
    {
        BossAi.OnBossTargetChanged += OnBossTargetChanged;
    }

    private void OnBossTargetChanged( Transform transform )
    {
        
    }

    private void OnDisable()
    {
        BossAi.OnBossTargetChanged -= OnBossTargetChanged;
    }

    void Update()
    {
        if ( bossFaceActive )
        {
            cameraPosition = Camera.main.transform.position;
            rotateDirection = (cameraPosition - transform.position).normalized;
            lookRotation = Quaternion.LookRotation( rotateDirection );
            transform.rotation = Quaternion.Slerp( transform.rotation, lookRotation, 1 );
        }
        else
        {
            GetComponent<Canvas>().enabled = false;
        }
    }
}
