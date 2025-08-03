using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 10.0f;
    [SerializeField] private float _lifeTime = 2.0f;
    private readonly float _offset = 0.5f;

    private Vector2 _direction;

    private float _damage;
    private float _lifeTimer;

    private void OnEnable()
    {
        _lifeTimer = 0.0f;
    }

    private void Update()
    {
        transform.Translate(_direction * _speed * Time.deltaTime);

        _lifeTimer += Time.deltaTime;
        if (_lifeTimer >= _lifeTime)
        {
            gameObject.SetActive(false);
        }
    }

    public void Fire(Vector2 direction, Vector3 startPos, float damage)
    {
        Vector3 pos = startPos;
        //pos.y += _offset;
        startPos = pos;

        transform.position = startPos;
        _damage = damage;
        _direction = direction.normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"Projectile hit: {collision.name} with tag {collision.tag}");
            Character player = collision.GetComponent<Character>();

            if (player != null)
                player.TakeDamage(_damage);

            gameObject.SetActive(false);
        }
    }
}
