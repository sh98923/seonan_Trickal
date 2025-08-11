using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    private Character _character;
    private GameObject _prefab;
    private List<GameObject> _projectiles = new List<GameObject>();

    public void SetAttack()
    {
        _character = GetComponent<Character>();
        _prefab = Resources.Load<GameObject>(_character.Data.ProjectilePath);

        if (_prefab != null)
        {
            for(int i = 0; i < _character.Data.IsRangeAttack.Length; i++)
            {
                int poolSize = _character.Data.PoolSize[i];
                string spritePath = _character.Data.ProjectileSpritePath[i];

                Add(poolSize, _prefab, spritePath);
            }
        }
    }

    private void Add(int pool, GameObject prefab, string sprite)
    {
        for (int i = 0; i < pool; i++)
        {
            GameObject projectile = Instantiate(prefab, transform);
            if(Resources.Load<Sprite>(sprite) != null)
            {
                print(sprite);
            }
            projectile.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(sprite);
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

    public void Attack(Collider2D target, float damage, bool isRange)
    {
        if (target == null) return;

        if (isRange)
        {
            Vector3 curPos = _character._attackPoint.position;
            Vector2 dir = target.transform.position - transform.position;
            Projectile projectile = Pop().GetComponent<Projectile>(); 
            
            if (_character.transform.localScale.x < 0)
            {
                dir.x = -dir.x;
            }

            projectile.Fire(dir, curPos, damage, _character.Data.Target);
        }
        else
        {
            Character character = target.GetComponent<Character>();
            if (character != null)
            {
                character.TakeDamage(damage);
            }
        }
    }
}