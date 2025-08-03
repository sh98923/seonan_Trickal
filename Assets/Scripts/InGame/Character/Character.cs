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

    [SerializeField] public Transform _attackPoint;
    protected Animator _animator;
    protected CharacterAttack _attack;
    protected Collider2D _targetCollider;

    protected CharacterFullData _characterData;
    public string CharacterProjectilePath
    {
        get 
        {
            if (_characterData == null)
            {
                return string.Empty;
            }

            return _characterData.ProjectilePath;
        }
    }

    protected Vector2 _moveDir;
    protected State _curState = State.Idle;
    
    protected string _attackType = "";
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
        _attack = GetComponent<CharacterAttack>();
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
        _isDead = false;
        _myCollider.enabled = true;

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
            TakeDamage(30);
            //Debug.Log($"{gameObject.name} HP: {_curHp}");
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

        if (BattleStateManager.Instance.IsBattle)
        {
            _curState = State.Move;
        }
    }

    protected virtual void MoveStateAction()
    {
        _animator.SetBool("IdleState", false);
    }

    protected virtual void AttackStateAction()
    {
        if (_isAttacking)
            return;

        _isAttacking = true;

        if (!_targetCollider.enabled)
        {
            _targetCollider = null;
            _curState = State.Move;
        }

        if (_targetCollider != null)
        { 
            _animator.SetTrigger("Attack");
        }
    }

    public void TakeDamage(float damage)
    {
        _curHp -= damage;
        Debug.Log($"{gameObject.name} 현재 체력: {_curHp}");

        ShowDamageText(damage);

        if (_curHp <= 0)
        {
            _curState = State.Dead;
        }
    }

    protected void ShowDamageText(float damage)
    {
        GameObject damageText = PoolingManager.Instance.Pop("DamageText");

        Vector3 worldPos = transform.position + Vector3.up * 1.8f;
        damageText.GetComponent<DamageText>().Initialize(damage, worldPos);
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

    public void SetCharacterData(CharacterFullData fullData)
    {
        _characterData = fullData;
        _attackType = fullData.CharacterData.AttackType;
        _criRate = fullData.CriRate;
        _attackRange = fullData.AtkRange;
        _atkCoolTime = fullData.AtkCoolTime;
    }
    public void OnAttackHit()
    {
        _attack.BaseAttack(_targetCollider, _atk);
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