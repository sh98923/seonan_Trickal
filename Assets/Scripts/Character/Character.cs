using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Character : MonoBehaviour
{
    protected enum State
    {
        Idle, Move, Attack, Dead
    }

    protected event Action<Character> _onDie;
    public event Action<Character> OnDie
    {
        add { _onDie += value; }
        remove { _onDie -= value; }
    }
    protected Animator _animator;
    protected Collider2D _targetCollider;
    [SerializeField] protected Transform _attackPoint;
    
    protected Vector2 _moveDir;
    protected State _curState = State.Idle;

    protected string _type = "";
    protected readonly float _colliderOffset = 0.5f;
    protected readonly float _findTargetRange = 5.0f;
    protected float _curHp = 0.0f;
    protected float _maxHp = 0.0f;
    protected float _atk = 0.0f;
    protected float _criRate = 0.0f;
    protected float _attackRange = 0.0f;
    protected float _atkCoolTime = 0.0f;
    protected float _moveSpeed = 2.0f;

    protected bool _isAttacking = false;

    private SortingGroup _sortingGroup;
    private SpriteRenderer[] _spriteRenderers;
    private Collider2D _myCollider;

    private float _fadeDuration = 3.0f;
    private float _attackCoolTimer = 0.0f;

    private bool _isDead = false;

    protected void Awake() 
    { 
        _animator = GetComponent<Animator>();
        _myCollider = GetComponent<Collider2D>();
        _sortingGroup = GetComponent<SortingGroup>();
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
    }

    private void OnEnable()
    {
        _attackCoolTimer = 0.0f;
        
        _curState = State.Idle; 
        
        _isAttacking = false;
        _myCollider.enabled = true;
        _isDead = false;

        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            Color color = _spriteRenderers[i].color;
            color.a = 1.0f;
            _spriteRenderers[i].color = color;
        }
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _curHp -= 30f;
            Debug.Log($"{gameObject.name} HP: {_curHp}");
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
        _animator.SetBool("IdleState", true);
    }

    protected virtual void MoveStateAction()
    {
        _animator.SetBool("IdleState", false);
    }

    protected virtual void AttackStateAction()
    {
        if (_isAttacking)
            return;

        if (!_targetCollider.enabled)
        {
            _targetCollider = null;
            _curState = State.Move;
        }

        _isAttacking = true;
        _animator.SetTrigger("Attack");
    }

    public void TakeDamage(float damage)
    {
        _curHp -= damage;
        Debug.Log($"{gameObject.name} 현재 체력: {_curHp}");

        if (_curHp <= 0)
        {
            _curState = State.Dead;
        }
    }

    private void AttackCoolTime()
    {
        if (_isAttacking)
        {
            _attackCoolTimer += Time.deltaTime;

            if (_attackCoolTimer >= _atkCoolTime)
            {
                _attackCoolTimer -= _atkCoolTime;
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
        _targetCollider = null;
        _onDie?.Invoke(this);
        _animator.SetTrigger("Dead");
        GetComponent<Collider2D>().enabled = false;

        StartCoroutine(FadeOutAndInactive());
    }

    private IEnumerator FadeOutAndInactive()
    {
        yield return new WaitForSeconds(1.5f);

        float timer = 0;

        while (timer < _fadeDuration)
        {
            timer += Time.deltaTime;
            float curAlpha = Mathf.Lerp(1.0f, 0, timer / _fadeDuration);

            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                Color color = _spriteRenderers[i].color;
                color.a = curAlpha;
                _spriteRenderers[i].color = color;
            }

            yield return null;
        }

        gameObject.SetActive(false);
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