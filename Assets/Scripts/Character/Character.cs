using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Character : MonoBehaviour
{
    protected enum State
    {
        Idle, Move, Attack, Dead
    }

    private SortingGroup _sortingGroup;
    private Animator _animator;
    private SpriteRenderer[] _spriteRenderers;
    protected Collider2D _target;
    [SerializeField] protected Transform _attackPoint;

    protected Vector2 _moveDir;
    protected State _curState = State.Idle;

    protected string _type = "";
    protected readonly float _colliderOffset = 0.5f;
    protected readonly float _findTargetRange = 5.0f;
    protected float _hp = 0.0f;
    protected float _atk = 0.0f;
    protected float _criRate = 0.0f;
    protected float _attackRange = 0.0f;
    protected float _atkCoolTime = 0.0f;
    protected float _moveSpeed = 2.0f;
    private float _attackCoolTimer = 0.0f;
    private float _animLength = 0.0f;

    private bool _isAttacking = false;
    private bool _isDead = false;
    private float _fadeDuration = 3.0f;

    protected void Awake() 
    { 
        _animator = GetComponent<Animator>();
        _sortingGroup = GetComponent<SortingGroup>();
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _hp -= 30f;
            Debug.Log($"{gameObject.name} HP: {_hp}");
        }

        switch (_curState)
        {
            case State.Idle:
                IdleStateAction();
                break;
            case State.Move:
                MoveStateAction();
                break;
            case State.Attack:
                AttackStateAction();
                AttackCoolTime();
                break;
            case State.Dead:
                DeadStateAction();
                break;
        }

        _sortingGroup.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }

    protected virtual void IdleStateAction()
    {
        _animator.SetBool("IsInRanged", true);
    }

    protected virtual void MoveStateAction()
    {
        _animator.SetBool("IsInRanged", false);
    }

    protected virtual void AttackStateAction()
    {
        if (_isAttacking)
            return;

        _isAttacking = true;
        _animator.SetTrigger("Attack");

        _animLength = _animator.GetCurrentAnimatorStateInfo(0).length;
    }

    public void TakeDamage(float damage)
    {
        _hp -= damage;
        Debug.Log($"현재 체력: {_hp}");

        if (_hp <= 0 && !_isDead)
        {
            _curState = State.Dead;
        }
    }

    private void AttackCoolTime()
    {
        if (_isAttacking)
        {
            _attackCoolTimer += Time.deltaTime;

            if (_attackCoolTimer >= _animLength)
            {
                _attackCoolTimer -= _animLength;
                _isAttacking = false;
            }
        }
    }

    protected Collider2D FindTarget(string target, float range)
    {
        Vector3 pos = transform.position;
        pos.y += _colliderOffset;

        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, range);

        float closestDistance = float.MaxValue;
        Collider2D closestTarget = null;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag(target))
            {
                float dist = Vector2.Distance(pos, hit.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestTarget = hit;
                }
            }
        }

        return closestTarget;
    }

    protected virtual void DeadStateAction()
    {
        if (_isDead) 
            return;

        _isDead = true;
        _animator.SetTrigger("Dead");
        GetComponent<Collider2D>().enabled = false;

        StartCoroutine(FadeOutAndInactive());
    }

    private IEnumerator FadeOutAndInactive()
    {
        yield return new WaitForSeconds(3.0f);

        float timer = 0;

        while (timer < _fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1.0f, 0, timer / _fadeDuration);

            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                var color = _spriteRenderers[i].color;
                _spriteRenderers[i].color = new Color(color.r, color.g, color.b, alpha);
            }

            yield return null;
        }

        gameObject.SetActive(false);
    }

    public virtual void SetCharacterStat(MonsterData data, int wave)
    {
        _type = data.Type;
        _criRate = data.CriRate;
        _attackRange = data.Range;
        _atkCoolTime = data.AtkCoolTime;
    }

    public void SetPlayerStat(PlayerStatData data)
    {
        _hp = data.Hp;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        
        Vector3 pos = transform.position;
        pos.y += _colliderOffset;

        Gizmos.DrawWireSphere(pos, _attackRange);


        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos, _findTargetRange);
    }
}