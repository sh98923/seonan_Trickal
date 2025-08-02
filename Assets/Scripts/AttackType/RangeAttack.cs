using System.Collections.Generic;
using UnityEngine;

public class RangeAttack : CharacterAttack
{
    private Character _character;
    private GameObject _prefab;
    private List<GameObject> _projectiles = new List<GameObject>();

    private readonly int _pool = 5;

    private void Start()
    {
        _character = GetComponent<Character>();
        _prefab = Resources.Load<GameObject>(_character.CharacterWeaponPath);

        if (_prefab != null)
        {
            Add(_pool, _prefab);
        }
    }

    private void Add(int pool, GameObject prefab)
    {
        for (int i = 0; i < pool; i++)
        {
            GameObject projectile = Instantiate(prefab, transform);
            projectile.name = prefab.name;
            projectile.SetActive(false);
            _projectiles.Add(projectile);
        }
    }

    private GameObject Pop()
    {
        foreach (GameObject projectile in _projectiles)
        {
            if (!projectile.activeSelf)
            {
                projectile.SetActive(true);
                return projectile;
            }
        }

        return null;
    }

    public override void BaseAttack(Collider2D target, float damage)
    {
        Vector3 curPos = transform.position;
        Vector2 dir = target.transform.position - transform.position;
        Projectile projectile = Pop().GetComponent<Projectile>();
        projectile.Fire(dir, curPos, damage);
    }

    public override void SkillAttack(Collider2D target, float damage)
    {
    }

    public override void UltAttack(Collider2D target, float damage)
    {
    }
}