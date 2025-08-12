using UnityEngine;

public class BaseProjectile : MonoBehaviour
{
    [SerializeField] private float _speed = 10.0f;
    [SerializeField] private float _lifeTime = 2.0f;
    private readonly float _offset = 0.5f;

    private Vector2 _direction;

    protected string _tag;

    protected float _damage;
    private float _lifeTimer;

    protected bool _hasHit = false;

    private void OnEnable()
    {
        _hasHit = false;
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

    public void Fire(Vector2 direction, Vector3 startPos, float damage, string tag)
    {
        Vector3 pos = startPos;
        //pos.y += _offset;
        startPos = pos;
        transform.position = startPos;

        _tag = tag;
        _damage = damage;
        _direction = direction.normalized;
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (_hasHit) return;

        if (collision.CompareTag(_tag))
        {
            _hasHit = true;

            Character target = collision.GetComponent<Character>();

            if (target != null)
                target.TakeDamage(_damage);

            gameObject.SetActive(false);
        }
    }
}
