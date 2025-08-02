using UnityEngine;

public abstract class CharacterAttack : MonoBehaviour
{
    public abstract void BaseAttack(Collider2D target, float damage);
    public abstract void SkillAttack(Collider2D target, float damage);
    public abstract void UltAttack(Collider2D target, float damage);
}