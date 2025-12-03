using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public enum CharacterState
{
    Idle, Move, Attack, Dead
}

public class Character : MonoBehaviour, ITrackable
{
    // Enum
    protected enum ActionCategory
    {
        Attack, Buff
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
    protected DamageReceiver _damageReceiver;

    private CharacterGraphics _characterVisual;
    private CircleCollider2D _myCollider;
    private SpriteRenderer _shadowSprite;
    private SpriteRenderer[] _spriteRenderers;
    private Transform _atkPoint;
    private Transform _centerPoint;

    // 속성
    public CharacterData Data => _data;
    public GameObject Object => gameObject;
    public Transform AtkPoint => _atkPoint;
    public Transform CenterPoint => _centerPoint;

    public int SortingIndex
    {
        get { return _sortingGroup.sortingOrder; }
        set {  _sortingGroup.sortingOrder = value; }
    }
    public bool IsColliderEnable => _myCollider.enabled;

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
    protected CharacterState _curState = CharacterState.Idle;
    protected readonly float _findTargetRange = 5.0f;
    protected float _maxHp = 0.0f;
    protected float _moveSpeed = 4.0f;

    protected bool _isAttacking = false;
    private bool _isDead = false;

    private readonly float _colliderOffset = 0.5f;
    private float _animLengthRate = 1.0f;
    private readonly int _sortingScale = 100;

    protected void Awake()
    {
        _scale = transform.localScale;
        _atkPoint = transform.Find("AtkPos");
        _centerPoint = transform.Find("CenterPos");
        _action = GetComponents<CharacterAction>();
        _animator = GetComponent<Animator>();
        _myCollider = GetComponent<CircleCollider2D>();
        _myCollider.radius = 0.35f;
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

        _characterVisual = GetComponent<CharacterGraphics>();
        _characterVisual.Initialize(this, _spriteRenderers, _shadowSprite);

        _damageReceiver = GetComponent<DamageReceiver>();
    }

    protected void OnEnable()
    {
        if (_data == null) return;

        _damageReceiver.Initialize(this, _data.Hp);
        //print(_data.EngName + " : " + _data.Hp);

        _curState = CharacterState.Idle;
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
            case CharacterState.Idle:
                IdleStateAction();
                break;
            case CharacterState.Move:
                MoveStateAction();
                break;
            case CharacterState.Attack:
                AttackStateAction();
                break;
            case CharacterState.Dead:
                DeadStateAction();
                break;
        }

        _sortingGroup.sortingOrder = Mathf.RoundToInt(-transform.position.y * _sortingScale);
    }

    protected virtual void OnDrawGizmosSelected()
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
        if (BattleStateManager.Instance.IsEnteringBattle)
            _curState = CharacterState.Move;
    }

    protected virtual void MoveStateAction()
    {
        _animator.SetBool("IdleState", false);
    }

    protected virtual void AttackStateAction()
    {
        // 타겟 상태 체크
        CheckTargetStatus();

        // 공격 중이면 애니메이션 끝났는지 체크
        if (CheckAttackAnimation())
        {
            return;
        }

        // 공격 시작
        StartAttack();
    }

    protected void CheckTargetStatus()
    {
        // 타겟 없거나 죽었으면 _targetCollider는 null로 초기화
        if (_targetCollider != null && !_targetCollider.enabled)
        { 
            _targetCollider = null;
        }
    }

    protected bool CheckAttackAnimation()
    {
        if (!_isAttacking)
            return false;

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        bool hasAnimationEnded = stateInfo.normalizedTime >= 1.0f;

        // 애니메이션 끝났는데 타겟이 없다면 이동 상태로
        if (hasAnimationEnded)
        {
            _isAttacking = false;

            if (_targetCollider == null)
            { 
                _curState = CharacterState.Move; 
            }
        }

        return true; // 공격 중이었음
    }

    protected virtual void StartAttack()
    {
        if (!_targetCollider.enabled)
            return;

        _isAttacking = true;
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
        //_sortingGroup.sortingOrder = -10000;

        FadeOutAndInactive();
    }

    // 타겟 위치에 맞게 Flip
    private void FlipToTarget()
    {
        if (_targetCollider == null || _isDead)
        {
            return;
        }

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

        foreach (Collider2D hit in hits)
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
        _damageReceiver.TakeDamage(damage);
    }

    public void CharacterDeath()
    {
        _curState = CharacterState.Dead;
    }

    public void TakeDotDamage(HitType type, float damage, float duration, float tickInterval)
    {
        _damageReceiver.TakeDotDamage(type, damage, duration, tickInterval);
    }

    // 버프, 디버프 관련
    public void ApplyAttackSlow(float duration, float speed)
    {
        _damageReceiver.ApplyAttackSlow(duration, speed);
    }

    public void SetAnimatorSpeed(float speed)
    {
        _animLengthRate = 1 / speed;
        _animator.speed = speed;
    }

    /*private IEnumerator SlowAttackCoroutine(float duration, float speed)
    {
        _animLengthRate = 1 / speed;
        _animator.speed = speed;

        yield return new WaitForSeconds(duration);

        _animLengthRate = 1.0f;
        _animator.speed = 1.0f;
        _slowCoroutine = null;
    }*/
    // 공격 이벤트
    public virtual void OnAttack()
    {
        _action[(int)ActionCategory.Attack].Excute();
    }

    // 캐릭터 세팅
    public virtual void SetCharacterData(CharacterData data)
    {
        _data = data;
    }

    public void SetCharacterActionInit()
    {
        for (int i = 0; i < _action.Length; i++)
        {
            _action[i].SetInit();
        }
    }

    // 죽었을 때 페이드 아웃
    private void FadeOutAndInactive()
    {
        _characterVisual.StartFadeOutAndDisable();
    }
}