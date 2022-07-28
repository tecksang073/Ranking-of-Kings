public class AttackAnimationData : BaseAttackAnimationData
{
    public bool isRangeAttack = false;
    public float meleeAttackDistance = 0f;
}

public static class AttackAnimationDataExtension
{
    public static bool GetIsRangeAttack(this AttackAnimationData attackAnimation)
    {
        return attackAnimation == null ? false : attackAnimation.isRangeAttack;
    }

    public static float GetMeleeAttackDistance(this AttackAnimationData attackAnimation)
    {
        return attackAnimation == null ? 0f : attackAnimation.meleeAttackDistance;
    }
}
