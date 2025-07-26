using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private int poolSize = 10;

    private Queue<Projectile> pool = new Queue<Projectile>();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            Projectile proj = Instantiate(projectilePrefab, transform);
            proj.gameObject.SetActive(false);
            pool.Enqueue(proj);
        }
    }

    public Projectile Get(Vector2 position, Vector2 direction, float damage)
    {
        if (pool.Count == 0)
        {
            Projectile proj = Instantiate(projectilePrefab, transform);
            proj.gameObject.SetActive(false);
            pool.Enqueue(proj);
        }

        Projectile projectile = pool.Dequeue();
        projectile.transform.position = position;
        projectile.Init(direction, damage);
        return projectile;
    }

    public void Return(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
        pool.Enqueue(projectile);
    }
}
