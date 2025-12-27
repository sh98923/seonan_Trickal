using UnityEngine;

public class ProjectileData
{
    public Sprite Sprite;
    public Vector3 StartPos;
    public Vector3 Direction;
    public HitType HitType;
    public AtkEffectType EffectType;
    public float Damage;
    public float DotDamage;
    public float EffectValue;
    public float Duration;
    public float Speed;
    public string Key;
    public string Name;
    public bool IsFlipX;
    public bool IsRotation;
}

public class Projectile : MonoBehaviour
{
    protected const float _lifeTime = 2.0f;
    protected const float _rotationSpeed = 750.0f;

    protected const int _sortingOffset = 75;
    protected const int _sortingScale = 100;

    protected SpriteRenderer _sprite;
    protected ProjectileData _data;

    protected Vector2 _direction;

    protected float _lifeTimer;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _lifeTimer = 0.0f;
    }

    protected virtual void Update()
    {
        transform.Translate(_direction * _data.Speed * Time.deltaTime);

        if (_data.IsRotation)
        {
            transform.Rotate(Vector3.back, _rotationSpeed * Time.deltaTime);
        }

        _lifeTimer += Time.deltaTime;
        if (_lifeTimer >= _lifeTime)
        {
            gameObject.SetActive(false);
        }

        _sprite.sortingOrder = Mathf.RoundToInt(-transform.position.y * _sortingScale + _sortingOffset);
    }

    public virtual void Fire(ProjectileData data)
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

    protected void ApplyDamage(Character target)
    {
        target.TakeDamage(_data.Damage);
    }

    protected void ApplyDot(Character target)
    {
        target.TakeDotDamage(_data.HitType, _data.DotDamage, _data.Duration, _data.EffectValue);
    }

    protected void ApplySlow(Character target)
    {
        target.ApplyAttackSlow(_data.HitType, _data.Duration, _data.EffectValue);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == _data.Name)
        {
            Character target = collision.GetComponent<Character>();

            if (target == null) return;

            ApplyDamage(target);

            switch (_data.EffectType)
            {
                case AtkEffectType.Dot:
                    ApplyDot(target);
                    break;
                case AtkEffectType.Status:
                    ApplySlow(target);
                    break;
            }

            gameObject.SetActive(false);
        }
    }
}
