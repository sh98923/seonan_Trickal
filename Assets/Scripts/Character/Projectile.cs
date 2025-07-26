using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 10.0f;
    [SerializeField] private float _lifeTime = 2.0f;

    private float _damage;
    private Vector2 _direction;
    private ProjectilePool _pool;

    private float _timer;

    private void Update()
    {
        transform.Translate(_direction * _speed * Time.deltaTime);

        _timer += Time.deltaTime;
        if (_timer >= _lifeTime)
        {
            ReturnToPool();
        }
    }

    public void SetPool(ProjectilePool pool)
    {
        _pool = pool;
    }

    public void Init(Vector2 direction, float damage)
    {
        _direction = direction.normalized;
        _damage = damage;

        _timer = 0f;
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        _timer = 0f;   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"Projectile hit: {collision.name} with tag {collision.tag}");
            Character player = collision.GetComponent<Character>();

            if (player != null)
                player.TakeDamage(_damage);

            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        gameObject.SetActive(false);
        _pool?.Return(this);
    }
}
