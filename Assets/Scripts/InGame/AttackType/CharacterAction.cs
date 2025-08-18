using UnityEngine;

public abstract class CharacterAction : MonoBehaviour
{
    public abstract void Excute();

    public virtual void SetInit()
    {
    }

    public virtual void SetAttackInfo(Collider2D target, AttackType type, float damage, bool isRange)
    {
    }
}