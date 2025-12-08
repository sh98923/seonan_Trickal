using UnityEngine;

public class PiercingProjectile : Projectile
{
    protected override void Update()
    {
        transform.Translate(_direction * _data.Speed * Time.deltaTime, Space.World);

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

    public override void Fire(ProjectileData data)
    {
        base.Fire(data);

        _direction = data.Direction.normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Monster")
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
        }
    }
}
