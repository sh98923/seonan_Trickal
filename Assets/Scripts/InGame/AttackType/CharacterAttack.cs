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

    private void MeleeAttack()
    {
        Character character = _target.GetComponent<Character>();
        if (character != null)
        {
            character.TakeDamage(_damage);
        }
    }

    private void RangeAttack()
    {
        Character targetChar = _target.GetComponent<Character>();
        string effectName = _character.Data.AttackEffect[(int)_type];
        float attackSpeed = _character.Data.AtkSpeed[(int)_type];
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
                Sprite = _sprites[(int)_type],
                StartPos = _character.AtkPoint.position,
                Direction = dir,
                EffectType = effectType,
                Speed = attackSpeed,
                Damage = _damage,
                Key = _character.Data.ProjectileKey,
                Name = targetChar.name,
                IsFlipX = _isFlipX
            }; 
        
        WeaponManager.Instance.Fire(data);
    }

    private void Attack()
    {
        if (_target == null) return;

        if (_isRange)
        {
            RangeAttack();
        }
        else
        {
            MeleeAttack();
        }
    }

    public override void Excute()
    {
        Attack();
        if(_type == AttackType.Skill)
            print(_damage);
    }
}