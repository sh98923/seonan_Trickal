using System.Collections.Generic;
using UnityEngine;

public enum AtkEffectType
{
    Damage, Dot, Status
}

public class CharacterAttack : CharacterAction
{
    private Character _target = null;
    private List<Character> _targets = new List<Character>();
    private List<Sprite> _sprites = new List<Sprite>();

    private HitType _hitType;
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

    public override void SetAttackInfo(Character target, ActionSlot type, float damage)
    {
        _type = type;
        _damage = damage;

        if (target != null)
        {
            _target = target;
        }
        else
        {
            _target = null;
        }

        _isRange = _data.IsRangeAtk[(int)type];
    }

    public override void SetAttackInfo(Collider2D target, ActionSlot type, string clipName, float time, float damage)
    {
        _type = type; 
        _timer = time;
        _damage = damage;
        _clipName = clipName; 
        _targets = new List<Character>();

        if (target != null)
        {
            _target = target.GetComponent<Character>();
        }
        else
        {
            _target = null;
        }

        _isRange = _data.IsRangeAtk[(int)type];

        _dotDamage = _damage * _data.DotDamageRate;
        _effectValue = _data.EffectValue[(int)type];
        _duration = _data.Duration[(int)type];
    }

    public override void SetAttackInfo(Collider2D[] targets, ActionSlot type, string clipName, float time, float damage)
    {
        _type = type;
        _timer = time;
        _damage = damage;
        _clipName = clipName;
        _targets = new List<Character>();

        // Collider2D 배열 -> Character 리스트로 변환
        foreach (Collider2D target in targets)
        {
            Character character = target.GetComponent<Character>();
            if (character != null)
            { 
                _targets.Add(character); 
            }
        }

        _isRange = _data.IsRangeAtk[(int)type];
        _dotDamage = _damage * _data.DotDamageRate;
        _effectValue = _data.EffectValue[(int)type];
        _duration = _data.Duration[(int)type];
    }

    private void MeleeAttack(Character target, string atkEffectName, string hitEffectName)
    {
        IDamageableHealth targetHealth = target.Health;
        if (targetHealth == null)
        {
            return;
        }

        target.TakeDamage(targetHealth, _damage); ;
        
        if(target.tag == "Monster")
        {
            //print($"{target.name} 데미지 입음");
        }

        _hitType = GetEffectType<HitType>(hitEffectName, out _isValid);
        _effectType = GetEffectType<AtkEffectType>(atkEffectName, out _isValid);

        if (!_isValid) return;

        switch(_effectType)
        {
            case AtkEffectType.Dot:
                target.TakeDotDamage(targetHealth, _hitType, _dotDamage, _duration, _effectValue);
                break;
            case AtkEffectType.Status:
                target.ApplyAttackSlow(_hitType, _duration, _effectValue);
                break;
        }
    }

    private void RangeAttack(Character target, string atkEffectName, string hitEffectName)
    {
        _hitType = GetEffectType<HitType>(hitEffectName, out _isValid);
        _effectType = GetEffectType<AtkEffectType>(atkEffectName, out _isValid);

        if (!_isValid) return;

        float atkSpeed = _data.AtkSpeed[(int)_type];
        Vector2 dir = target.CenterPoint.position - _character.CenterPoint.position;

        if (_character.transform.localScale.x < 0)
        {
            _isFlipX = true;
        }

        ProjectileData data = new ProjectileData
        {
            // 기본 공격 정보
            Name = target.name,
            Key = _data.ProjectileKey[(int)_type],
            StartPos = _character.AtkPoint.position,
            Direction = dir,
            Speed = atkSpeed,
            Duration = _duration,
            Damage = _damage,
            DotDamage = _dotDamage,

            // 상태/효과 정보
            HitType = _hitType,
            EffectType = _effectType,
            EffectValue = _effectValue,

            // 시각/스프라이트
            Sprite = _sprites[(int)_type],
            IsFlipX = _isFlipX,
            IsRotation = _data.IsRotationProjectile
        };

        WeaponManager.Instance.Fire(data);
    }

    private void Attack()
    {
        List<Character> attackTargets = new List<Character>();

        if (_targets.Count > 0)
        {
            // 다중 타겟
            attackTargets.AddRange(_targets); 
        }
        else if (_target != null)
        { 
            // 단일 타겟
            attackTargets.Add(_target);
        }

        foreach (Character target in attackTargets)
        {
            string hitEffectName = _character.Data.FlashHit[(int)_type];
            string atkEffectName = _character.Data.ActionImpact[(int)_type];

            PlayEffect(target.transform);

            if (_isRange)
            {
                RangeAttack(target, atkEffectName, hitEffectName);
            }
            else
            {
                MeleeAttack(target, atkEffectName, hitEffectName);
            }
        }
    }

    public override void Excute()
    {
        Attack();
    }
}