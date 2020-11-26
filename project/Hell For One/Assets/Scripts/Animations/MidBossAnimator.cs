public class MidBossAnimator : EnemyAnimator
{
    
    private protected override void OnStartAttack(NormalCombat sender, GenericAttack attack)
    {
        if (attack.CanHitMultipleTargets)
        {
            PlayGroupAttackAnimation();
        }
    }
}
