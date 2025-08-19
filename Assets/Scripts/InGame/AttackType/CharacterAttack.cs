using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum AtkEffectType
{
    Damage, Dot, Slow
}

public class CharacterAttack : CharacterAction
{
    private Character _target;
    private List<Sprite> _sprites = new List<Sprite>();

    private AtkEffectType _effectType;

    private float _damage;
    private float _dotDamage;
    private float _duration;
    private float _effectValue;

    private bool _isRange;
    private bool _isFlipX;

    public override void SetInit()
    {
        base.SetInit();

        for (int i = 0; i < _data.IsRangeAtk.Length; i++)
        {
            string spritePath = _data.ProjectileSpritePath[i];
            Sprite sprite = Resources.Load<Sprite>(spritePath);
            _sprites.Add(sprite);
        }
    }

    public override void SetAttackInfo(Collider2D target, ActionSlot type, float damage)
    {
        _type = type;
        _damage = damage;
        _target = target.GetComponent<Character>();
        _isRange = _data.IsRangeAtk[(int)type];

        if (_target.tag == "Monster")
        {
            _dotDamage = _damage * _data.DotDamageRate;
            _effectValue = _data.EffectValue;
            _duration = _data.Duration[(int)type];
        }
    }

    private void MeleeAttack(string effectName)
    {
        if (_target != null)
        {
            _target.TakeDamage(_damage);

            _effectType = GetEffectType<AtkEffectType>(effectName, out _isValid);

            if (!_isValid) return;

            switch(_effectType)
            {
                case AtkEffectType.Dot:
                    _target.TakeDotDamage(_dotDamage, _duration, _effectValue);
                    break;
                case AtkEffectType.Slow:
                    _target.ApplyAttackSlow(_duration, _effectValue);
                    break;
            }
        }
    }

    private void RangeAttack(string effectName)
    {
        _effectType = GetEffectType<AtkEffectType>(effectName, out _isValid);

        if (!_isValid) return;

        float atkSpeed = _character.Data.AtkSpeed[(int)_type];
        Vector2 dir = _target.CenterPoint.position - _character.CenterPoint.position;

        if (_character.transform.localScale.x < 0)
        {
            _isFlipX = true;
        }

        ProjectileData data = new ProjectileData
        {
            // 기본 공격 정보
            Name = _target.name,
            Key = _character.Data.ProjectileKey,
            StartPos = _character.AtkPoint.position,
            Direction = dir,
            Speed = atkSpeed,
            Duration = _duration,
            Damage = _damage,
            DotDamage = _dotDamage,

            // 상태/효과 정보
            EffectType = _effectType,
            EffectValue = _effectValue,

            // 시각/스프라이트
            Sprite = _sprites[(int)_type],
            IsFlipX = _isFlipX
        };

        WeaponManager.Instance.Fire(data);
    }

    private void Attack()
    {
        if (_target == null) return;

        string effectName = _character.Data.ActionImpact[(int)_type];

        if (_isRange)
        {
            RangeAttack(effectName);
        }
        else
        {
            MeleeAttack(effectName);
        }
    }

    public override void Excute()
    {
        Attack();
    }
}