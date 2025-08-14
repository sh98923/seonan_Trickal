using UnityEngine;

public class ProjectileData
{
    public Sprite Sprite;
    public Vector3 StartPos;
    public Vector3 Direction;
    public AttackEffectType EffectType;
    public float Damage;
    public float Speed;
    public string Key;
    public string Name;
    public bool IsFlipX;
}

public enum AttackEffectType 
{ 
    Damage, Dot, Slow
}

public class Projectile : MonoBehaviour
{
    [Header("공통")]
    private AttackEffectType _effectType;
    private float _baseDamage;

    [Header("도트 전용 옵션")]
    private float _dotDamage;
    private float _dotDuration;
    private float _dotInterval;

    private float _speed;
    private readonly float _lifeTime = 3.0f;
    private readonly float _offset = 0.5f;

    private SpriteRenderer _sprite;

    private Vector2 _direction;

    protected string _name;

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
        transform.Translate(_direction * _speed * Time.deltaTime);

        _lifeTimer += Time.deltaTime;
        if (_lifeTimer >= _lifeTime)
        {
            gameObject.SetActive(false);
        }

        _sprite.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100 + 75);
    }

    public void Fire(ProjectileData data)
    {
        transform.position = data.StartPos;

        _name = data.Name;
        _effectType = data.EffectType;
        _speed = data.Speed;
        _baseDamage = data.Damage;
        _dotDamage = _baseDamage * 0.15f;
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
        target.TakeDamage(_baseDamage);
    }

    private void ApplyDot(Character target)
    {
        target.TakeDotDamage(_dotDamage, 10, 2.2f);
    }

    private void ApplySlow(Character target)
    {
        target.ApplyAttackSlow(15, 0.65f);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (_hasHit) return;

        if (collision.name == _name)
        {
            _hasHit = true;

            Character target = collision.GetComponent<Character>();

            if (target == null) return;

            ApplyDamage(target);

            switch (_effectType)
            {
                case AttackEffectType.Dot:
                    ApplyDot(target);
                    break;
                case AttackEffectType.Slow:
                    ApplySlow(target);
                    break;
            }

            gameObject.SetActive(false);
        }
    }
}
