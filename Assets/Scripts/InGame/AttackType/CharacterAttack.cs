using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : CharacterAction
{
    private Collider2D _target;
    private Character _character;
    private List<GameObject> _prefabs = new List<GameObject>();
    private Dictionary<AttackType, List<GameObject>> _projectiles = new Dictionary<AttackType, List<GameObject>>();

    private AttackType _type;

    private float _damage;

    private bool _isRange;

    public override void SetInit()
    {
        _character = GetComponent<Character>();

        for (int i = 0; i < _character.Data.IsRangeAttack.Length; i++)
        {
            // 1. 프리팹 로드
            string projectilePath = _character.Data.ProjectilePath[i];
            GameObject prefab = Resources.Load<GameObject>(projectilePath);
            _prefabs.Add(prefab);

            // 2. 풀 사이즈와 스프라이트 경로 가져오기
            int poolSize = _character.Data.PoolSize[i];
            string spritePath = _character.Data.ProjectileSpritePath[i];

            // 3. 해당 공격 타입 풀 생성
            Add(i, poolSize, prefab, spritePath);
        }
    }

    private void Add(int index, int pool, GameObject prefab, string sprite)
    {
        List<GameObject> projectiles = new List<GameObject>();

        for (int i = 0; i < pool; i++)
        {
            GameObject projectile = Instantiate(prefab, transform);
            projectile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(sprite);
            projectile.name = prefab.name;
            projectile.SetActive(false);
            projectiles.Add(projectile);
        }

        _projectiles.Add((AttackType)index, projectiles);
    }

    private GameObject Pop(AttackType type)
    {
        List<GameObject> projectiles = _projectiles[type];

        foreach (GameObject projectile in projectiles)
        {
            if (!projectile.activeSelf)
            {
                projectile.SetActive(true);
                return projectile;
            }
        }

        return null;
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
        Vector3 curPos = _character._attackPoint.position;
        Vector2 dir = target.transform.position - transform.position;
        BaseProjectile projectile = Pop(type).GetComponent<BaseProjectile>();

        if (_character.transform.localScale.x < 0)
        {
            dir.x = -dir.x;
        }

        projectile.Fire(dir, curPos, damage, _character.Data.Target);
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