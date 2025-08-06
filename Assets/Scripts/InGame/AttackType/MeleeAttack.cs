using UnityEngine;

public class MeleeAttack : CharacterAction
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

    public override void Skill(Collider2D target, float damage)
    {
        if (target == null)
            return;

        Character character = target.GetComponent<Character>();
        if (character != null)
        {
            character.TakeDamage(damage);
        }
    }

    public override void Ultimate(Collider2D target, float damage)
    {
    }
}