using UnityEngine;

public class MeleeAttack : CharacterAttack
{
    public override void BaseAttack(Collider2D target, float damage)
    {
        if (target == null)
            return;

        Character character = target.GetComponent<Character>();
        if (character != null)
        {
            character.TakeDamage(damage);
        }
    }

    public override void SkillAttack(Collider2D target, float damage)
    {
        if (target == null)
            return;

        Character character = target.GetComponent<Character>();
        if (character != null)
        {
            character.TakeDamage(damage);
        }
    }

    public override void UltAttack(Collider2D target, float damage)
    {
    }
}