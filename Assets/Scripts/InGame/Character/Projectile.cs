using UnityEngine;

public class ProjectileData
{
    public Sprite Sprite;
    public Vector3 StartPos;
    public Vector3 Direction;
    public AtkEffectType EffectType;
    public float Damage;
    public float DotDamage;
    public float EffectValue;
    public float Duration;
    public float Speed;
    public string Key;
    public string Name;
    public bool IsFlipX;
}

public class Projectile : MonoBehaviour
{
    private readonly float _lifeTime = 3.0f;

    private SpriteRenderer _sprite;
    private ProjectileData _data;

    private Vector2 _direction;

    private readonly int _sortingOffset = 75;
    private readonly int _sortingScale = 100;

    private float _lifeTimer;

    protected bool _hasHit = false;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _hasHit = false;
        _lifeTimer = 0.0f;
    }

    private void Update()
    {
        transform.Translate(_direction * _data.Speed * Time.deltaTime);

        _lifeTimer += Time.deltaTime;
        if (_lifeTimer >= _lifeTime)
        {
            gameObject.SetActive(false);
        }

        _sprite.sortingOrder = Mathf.RoundToInt(-transform.position.y * _sortingScale + _sortingOffset);
    }

    public void Fire(ProjectileData data)
    {
        transform.position = data.StartPos;

        _data = data;

        _direction = data.Direction.normalized;

        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;

        if (transform.localScale.x > 0)
        {
            angle += 180.0f;

            _direction = Vector2.left;
        }
        else
        {
            _direction = Vector2.right;
        }

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
    }

    private void ApplyDamage(Character target)
    {
        target.TakeDamage(_data.Damage);
    }

    private void ApplyDot(Character target)
    {
        target.TakeDotDamage(_data.DotDamage, _data.Duration, _data.EffectValue);
    }

    private void ApplySlow(Character target)
    {
        target.ApplyAttackSlow(_data.Duration, _data.EffectValue);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (_hasHit) return;

        if (collision.name == _data.Name)
        {
            _hasHit = true;

            Character target = collision.GetComponent<Character>();

            if (target == null) return;

            ApplyDamage(target);

            switch (_data.EffectType)
            {
                case AtkEffectType.Dot:
                    ApplyDot(target);
                    break;
                case AtkEffectType.Slow:
                    ApplySlow(target);
                    break;
            }

            gameObject.SetActive(false);
        }
    }
}
