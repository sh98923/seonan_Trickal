using UnityEngine;

public abstract class CharacterAction : MonoBehaviour
{
    public abstract void BaseAttack(Collider2D target, float damage);
    public abstract void Skill(Collider2D target, float damage);
    public abstract void Ultimate(Collider2D target, float damage);
}