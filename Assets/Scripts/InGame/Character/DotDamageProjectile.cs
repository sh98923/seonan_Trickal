using UnityEngine;

public class DotDamageProjectile : BaseProjectile
{
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (_hasHit) return;

        if (collision.CompareTag(_tag))
        {
            _hasHit = true;

            Character target = collision.GetComponent<Character>();

            if (target != null)
            { 
                target.TakeDotDamage(_damage, 10.0f, 3.2f);
            }

            gameObject.SetActive(false);
        }
    }
}