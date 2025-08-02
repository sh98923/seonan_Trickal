using UnityEngine;

public class MeleeMonsterAttack : CharacterAttack
{
    public override void BaseAttack(Collider2D target, float damage)
    {
        if (target == null)
            return;

        Player player = target.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(damage);
        }
    }

    public override void SkillAttack(Collider2D target, float damage)
    {
    }

    public override void UltAttack(Collider2D target, float damage)
    {
    }
}