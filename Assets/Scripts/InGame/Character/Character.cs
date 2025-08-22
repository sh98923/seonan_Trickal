using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Character : MonoBehaviour
{
    // Enum
    protected enum ActionCategory 
    {
        Attack, Buff 
    }
    protected enum State 
    {
        Idle, Move, Attack, Dead
    }

    // 이벤트
    protected event Action<Character> _onDie;
    public event Action<Character> OnDie
    {
        add { _onDie += value; }
        remove { _onDie -= value; }
    }

    // 컴포넌트
    protected Animator _animator;
    protected CharacterAction[] _action;
    protected Collider2D _targetCollider;
    protected SortingGroup _sortingGroup;
    private Collider2D _myCollider;
    private SpriteRenderer _shadowSprite;
    private SpriteRenderer[] _spriteRenderers;
    private Transform _atkPoint;
    private Transform _centerPoint;

    // 속성
    public int SortingIndex => _sortingGroup.sortingOrder;
    public CharacterData Data => _data;
    public Transform AtkPoint => _atkPoint;
    public Transform CenterPoint => _centerPoint;

    // 임시/액션 관련 변수
    protected string _clipName = "";
    protected float _duration = 0.0f;

    // 색상
    private Color _shadowOriginalColor = Color.white;

    // 스탯
    protected CharacterData _data;
    protected Vector3 _scale;
    protected Vector2 _moveDir;
    protected ActionSlot _actionType = ActionSlot.Base;
    protected State _curState = State.Idle;
    protected readonly float _findTargetRange = 5.0f;
    protected float _curHp = 0.0f;
    protected float _maxHp = 0.0f;
    protected float _moveSpeed = 2.0f;

    protected bool _isAttacking = false;
    private bool _isDead = false;

    private readonly float _colliderOffset = 0.5f;
    private float _animLengthRate = 1.0f;
    private float _fadeDuration = 3.0f;
    private float _attackCoolTimer = 0.0f;
    private readonly int _sortingScale = 100;
    private Coroutine _slowCoroutine = null;


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

        foreach (SpriteRenderer sprite in _spriteRenderers)
        {
            if (sprite.name == "Shadow")
            {
                _shadowSprite = sprite;
                _shadowOriginalColor = sprite.color;
                break;
            }
        }
    }

    private void OnEnable()
    {
        _attackCoolTimer = 0.0f;
        _curState = State.Idle;
        _isAttacking = false;
        _isDead = false;
        _myCollider.enabled = true;

        foreach (SpriteRenderer sprite in _spriteRenderers)
        {
            Color color = sprite.color;

            if (sprite.name == "Shadow")
            {
                sprite.color = _shadowOriginalColor;
                continue;
            }

            color.a = 1.0f;
            sprite.color = color;
        }
    }

    protected void Update()
    {
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

    private void OnDrawGizmosSelected()
    {
        if (_data == null) return;

        Vector3 pos = transform.position;
        pos.y += _colliderOffset;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, _data.AtkRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos, _findTargetRange);
    }

    // FSM 상태 관련 함수
    protected virtual void IdleStateAction()
    {
        _animator.SetBool("IdleState", true);
        if (BattleStateManager.Instance.IsBattle)
            _curState = State.Move;
    }

    protected virtual void MoveStateAction()
    {
        _animator.SetBool("IdleState", false);
    }

    protected virtual void AttackStateAction()
    {
        if (_isAttacking) return;
        _isAttacking = true;

        if (_targetCollider == null || !_targetCollider.enabled)
        {
            _targetCollider = null;
            _curState = State.Move;
            return;
        }

        _animator.SetTrigger("Attack");
    }

    protected virtual void DeadStateAction()
    {
        if (_isDead) return;

        _isDead = true;
        _targetCollider = null;
        _onDie?.Invoke(this);
        _animator.SetTrigger("Dead");
        _myCollider.enabled = false;

        StartCoroutine(FadeOutAndInactive());
    }

    // 타겟 위치에 맞게 Flip
    private void FlipToTarget()
    {
        if (_targetCollider == null) return;

        _scale.x = (_targetCollider.transform.position.x < transform.position.x)
            ? Mathf.Abs(_scale.x)
            : -Mathf.Abs(_scale.x);

        transform.localScale = _scale;
    }

    protected Collider2D FindTarget(string targetTag, float range)
    {
        Vector3 pos = transform.position;
        pos.y += _colliderOffset;

        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, range);
        float closestDistance = float.MaxValue;
        Collider2D closestTarget = null;

        foreach (var hit in hits)
        {
            if (!hit.CompareTag(targetTag)) continue;

            float dist = Vector2.Distance(pos, hit.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestTarget = hit;
            }
        }

        return closestTarget;
    }

    // 공격 처리
    public void TakeDamage(float damage)
    {
        _curHp -= damage;
        ShowDamageText(damage);

        if (_curHp <= 0) _curState = State.Dead;
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
            if (_curState == State.Dead) yield break;

            TakeDamage(damagePerTick);
            elapsed += tickInterval;
        }
    }

    private void ShowDamageText(float damage)
    {
        GameObject damageText = PoolingManager.Instance.Pop("DamageText");
        Vector3 worldPos = transform.position + Vector3.up * 1.8f;
        damageText.GetComponent<DamageText>().Initialize(damage, worldPos);
    }

    // 버프, 디버프 관련
    public void ApplyAttackSlow(float duration, float speed)
    {
        if (_slowCoroutine != null) StopCoroutine(_slowCoroutine);
        _slowCoroutine = StartCoroutine(SlowAttackCoroutine(duration, speed));
    }

    private IEnumerator SlowAttackCoroutine(float duration, float speed)
    {
        _animLengthRate = 1 / speed;
        _animator.speed = speed;

        yield return new WaitForSeconds(duration);

        _animLengthRate = 1.0f;
        _animator.speed = 1.0f;
        _slowCoroutine = null;
    }

    // 공격 이벤트
    public virtual void OnAttack()
    {
        _action[(int)ActionCategory.Attack].SetAttackInfo(_targetCollider, _actionType, _data.Atk);
        _action[(int)ActionCategory.Attack].Excute();
    }

    private void AttackCoolTime()
    {
        if (!_isAttacking) return;

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        bool isNotTransitioning = !_animator.IsInTransition(0);
        bool hasAnimationEnded = stateInfo.normalizedTime >= 1.0f;

        if (isNotTransitioning && hasAnimationEnded)
            _isAttacking = false;
    }

    // 캐릭터 세팅
    public void SetCharacterData(CharacterData data) => _data = data;

    public void SetCharacterActionInit()
    {
        for(int i = 0; i < _action.Length; i++)
        {
            _action[i].SetInit();
        }
    }

    // 죽었을 때 페이드 아웃
    private IEnumerator FadeOutAndInactive()
    {
        yield return new WaitForSeconds(1.5f);

        float timer = 0.0f;

        Color shadowOriginalColor = _shadowSprite.color;

        while (timer < _fadeDuration)
        {
            timer += Time.deltaTime;

            foreach (SpriteRenderer sprite in _spriteRenderers)
            {
                UpdateSpriteAlpha(sprite, shadowOriginalColor, timer);
            }

            yield return null;
        }

        gameObject.SetActive(false);
    }

    private void UpdateSpriteAlpha(SpriteRenderer sprite, Color shadowColor, float timer)
    {
        Color color = sprite.color;

        float progress = timer / _fadeDuration;

        if (sprite == _shadowSprite)
        {
            color.a = Mathf.Lerp(shadowColor.a, 0.0f, progress);
        }
        else
        {
            color.a = Mathf.Lerp(1.0f, 0.0f, progress);
        }

        sprite.color = color;
    }
}