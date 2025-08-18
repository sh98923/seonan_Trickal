using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Character : MonoBehaviour
{
    protected enum ActionType
    {
        Attack, Buff
    }

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
    protected CharacterAction[] _action;
    protected Collider2D _targetCollider;
    protected SortingGroup _sortingGroup;
    public int SortingIndex
    {
        get { return _sortingGroup.sortingOrder; }
    }

    protected CharacterData _data;
    public CharacterData Data
    {
        get { return _data; }
    }

    protected Vector3 _scale;
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
    //protected float _atkCoolTime = 0.0f;
    protected float _moveSpeed = 2.0f;

    protected bool _isAttacking = false;

    private Transform _atkPoint;
    public Transform AtkPoint
    {
        get { return _atkPoint; }
    }
    private Transform _centerPoint;
    public Transform CenterPoint
    {
        get { return _centerPoint; }
    }

    private Collider2D _myCollider;
    private SpriteRenderer[] _spriteRenderers;

    private Coroutine _slowCoroutine = null;

    private float _animLengthRate = 1.0f;
    private float _fadeDuration = 3.0f;
    private float _attackCoolTimer = 0.0f;

    private readonly int _sortingScale = 100;

    private bool _isDead = false;

    protected void Awake()
    {
        _scale = transform.localScale;
        _atkPoint = transform.Find("AtkPos");
        _centerPoint = transform.Find("CenterPos");
        _action = GetComponents<CharacterAction>();
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
            if (_spriteRenderers[i].name == "Shadow")
                continue;

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
        }

        FlipToTarget();

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

        _sortingGroup.sortingOrder = Mathf.RoundToInt(-transform.position.y * _sortingScale);
    }

    private void FlipToTarget()
    {
        if (_targetCollider != null)
        {
            if (_targetCollider.transform.position.x < transform.position.x)
            {
                _scale.x = Mathf.Abs(_scale.x); // 무조건 양수
            }
            else
            {
                _scale.x = Mathf.Abs(_scale.x) * -1; // 무조건 음수
            }

            transform.localScale = _scale;
        }
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

        //StartCoroutine(SetCoolTime());
    }

   /* private float GetCurrentAnimationLength()
    {
        AnimatorClipInfo[] clipInfos = _animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfos.Length > 0.0f)
        {
            // 일반적으로 하나만 존재하지만 여러 개일 수도 있음 -> 첫 번째 클립의 길이를 가져옴
            return clipInfos[0].clip.length * _animLengthRate;
        }

        return 0.0f;
    }

    protected IEnumerator SetCoolTime()
    {
        yield return null; // 한 프레임 대기 (애니메이션 상태가 바뀔 시간을 줌)

        float animLength = GetCurrentAnimationLength();

        _atkCoolTime = animLength;
    }*/

    public void TakeDamage(float damage)
    {
        _curHp -= damage;

        ShowDamageText(damage);

        if (_curHp <= 0)
        {
            _curState = State.Dead;
        }
    }

    public void TakeDotDamage(float damage, float duration, float tickInterval)
    {
        StartCoroutine(Dot(damage, duration, tickInterval));
    }

    private IEnumerator Dot(float damagePerTick, float duration, float tickInterval)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            yield return new WaitForSeconds(tickInterval);

            if (_curState == State.Dead)
                yield break;

            TakeDamage(damagePerTick);
            elapsed += tickInterval;
        }
    }

    public void ApplyAttackSlow(float duration, float speed)
    {
        if (_slowCoroutine != null)
            StopCoroutine(_slowCoroutine);

        _slowCoroutine = StartCoroutine(SlowAttackCoroutine(duration, speed));
    }

    private IEnumerator SlowAttackCoroutine(float duration, float speed)
    {
        _animLengthRate = 1 / speed;
        _animator.speed = speed;

        yield return new WaitForSeconds(duration);

        _animLengthRate = 1.0f;
        _animator.speed = 1.0f;
        _slowCoroutine = null; // 코루틴 종료 표시
    }   

    protected void ShowDamageText(float damage)
    {
        GameObject damageText = PoolingManager.Instance.Pop("DamageText");

        Vector3 worldPos = transform.position + Vector3.up * 1.8f;
        damageText.GetComponent<DamageText>().Initialize(damage, worldPos);
    }

    private void AttackCoolTime()
    {
        if (!_isAttacking)
            return;

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        bool isNotTransitioning = !_animator.IsInTransition(0);
        bool hasAnimationEnded = stateInfo.normalizedTime >= 1.0f;

        // 공격 애니메이션이 끝났거나 다른 상태로 넘어갔을 때만 종료
        if (isNotTransitioning && hasAnimationEnded)
        {
            _isAttacking = false;
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

    public void SetCharacterData(CharacterData data)
    {
        _data = data;
        _criRate = data.CriRate;
        _atk = data.Atk;
        _attackType = data.AtkType;
        _attackRange = data.AtkRange;
    }

    public void SetCharacterActionInit()
    {
        for (int i = 0; i < _action.Length; i++)
        {
            _action[i].SetInit();
        }
    }    

    public void OnAttackHit()
    {
        _action[(int)ActionType.Attack].SetAttackInfo(_targetCollider, AttackType.Base, _atk, _data.IsRangeAttack[(int)AttackType.Base]);
        _action[(int)ActionType.Attack].Excute();
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