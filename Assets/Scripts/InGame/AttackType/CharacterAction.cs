using System;
using UnityEngine;

public enum ActionSlot
{
    Base, Skill, Ult
}

public abstract class CharacterAction : MonoBehaviour
{
    protected Character _character;
    protected CharacterData _data;

    protected ActionSlot _type;

    protected bool _isValid;

    protected T GetEffectType<T>(string effectName, out bool isValid) where T : struct, System.Enum
    {
        isValid = Enum.TryParse(effectName, out T effectType);

        return effectType;
    }

    public abstract void Excute();

    public virtual void SetInit()
    {
        _character = GetComponent<Character>();
        _data = _character.Data;
    }

    public virtual void SetBuffInfo(ActionSlot type, string clipName, float effectValue, float time)
    {
    }

    public virtual void SetAttackInfo(Collider2D target, ActionSlot type, float damage)
    {
    }
}