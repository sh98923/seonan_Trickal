using System;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    Base, Skill, Ult
}

public class CharacterAttack : CharacterAction
{
    private Collider2D _target;
    private Character _character;
    private List<Sprite> _sprites = new List<Sprite>();

    private AttackType _type;

    private float _damage;

    private bool _isRange;
    private bool _isFlipX;

    public override void SetInit()
    {
        _character = GetComponent<Character>();

        for (int i = 0; i < _character.Data.IsRangeAttack.Length; i++)
        {
            string spritePath = _character.Data.ProjectileSpritePath[i];
            Sprite sprite = Resources.Load<Sprite>(spritePath);
            _sprites.Add(sprite);
        }
    }

    public override void SetAttackInfo(Collider2D target, AttackType type, float damage, bool isRange)
    {
        _target = target;
        _type = type;
        _damage = damage;
        _isRange = isRange;
    }

    private void MeleeAttack(Collider2D target, float damage)
    {
        Character character = target.GetComponent<Character>();
        if (character != null)
        {
            character.TakeDamage(damage);
        }
    }

    private void RangeAttack(Collider2D target, AttackType type, float damage)
    {
        Character targetChar = target.GetComponent<Character>();
        string effectName = _character.Data.AttackEffect[(int)type];
        float attackSpeed = _character.Data.AtkSpeed[(int)type];
        bool success = Enum.TryParse(effectName, out AttackEffectType effectType);

        if (!success) return;

        Vector2 dir = targetChar.CenterPoint.position - _character.CenterPoint.position;

        if (_character.transform.localScale.x < 0)
        {
            _isFlipX = true;
        }

        ProjectileData data =
            new ProjectileData
            {
                Sprite = _sprites[(int)type],
                StartPos = _character.AtkPoint.position,
                Direction = dir,
                EffectType = effectType,
                Speed = attackSpeed,
                Damage = damage,
                Key = _character.Data.ProjectileKey,
                Name = targetChar.name,
                IsFlipX = _isFlipX
            }; 
        
        WeaponManager.Instance.Fire(data);
    }

    public void Attack(Collider2D target, AttackType type, float damage, bool isRange)
    {
        if (target == null) return;

        if (isRange)
        {
            RangeAttack(target, type, damage);
        }
        else
        {
            MeleeAttack(target, damage);
        }
    }

    public override void Excute()
    {
        Attack(_target, _type, _damage, _isRange);
        if(_type == AttackType.Skill)
            print(_damage);
    }
}