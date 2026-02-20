using System;
using UnityEngine;

public enum ActionSlot
{
    Attack, Skill, Ult
}

public abstract class CharacterAction : MonoBehaviour
{
    protected Character _character;
    protected CharacterData _data;

    protected ActionSlot _type;

    protected string _clipName = "";

    protected float _timer = 0.0f;

    protected bool _isValid;

    protected void PlayEffect(Transform transform)
    {
        if (_clipName == "None") return;

        // 이펙트 재생
        EffectManager.Instance.PlayEffect(transform, _clipName, _timer);
    }

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

    public virtual void SetAttackInfo(Character target, ActionSlot type, float damage)
    {
    }

    public virtual void SetAttackInfo(Collider2D target, ActionSlot type, string clipName, float time, float damage)
    {
    }

    public virtual void SetAttackInfo(Collider2D[] targets, ActionSlot type, string clipName, float time, float damage)
    {
    }
}