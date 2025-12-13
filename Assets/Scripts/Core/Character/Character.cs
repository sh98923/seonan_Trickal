using System;
using UnityEngine;
using UnityEngine.Rendering;

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
    protected event Action _onDie;
    public event Action OnDie
    {
        add { _onDie += value; }
        remove { _onDie -= value; }
    }

    // 컴포넌트
    protected CharacterMovement _movement;
    protected CharacterAnimator _animator;
    protected CharacterAction[] _action;
    protected SortingGroup _sortingGroup;
    protected DamageReceiver _damageReceiver;
    protected TargetSelector[] _targets = new TargetSelector[3];

    private CharacterGraphics _characterVisual;
    private CircleCollider2D _myCollider;
    private Transform _atkPoint;
    private Transform _centerPoint;
    private Transform _rootPoint;
    public Vector3 RootPos
    {
        get { return _rootPoint.transform.position; }
    }

    // 속성
    public CharacterData Data => _data;
    public GameObject Object => gameObject;
    public Character Self => gameObject.GetComponent<Character>();
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

    // 스탯
    protected CharacterData _data;
    protected Vector3 _scale;
    //protected Vector2 _moveDir;
    protected ActionSlot _actionType = ActionSlot.Attack;
    protected CharacterState _curState = CharacterState.Idle;
    public CharacterState CurState
    {
        get { return _curState; }
    }
    protected readonly float _findTargetRange = 5.0f;
    protected float _maxHp = 0.0f;
   // protected float _moveSpeed = 4.0f;

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
        _rootPoint = transform.Find("Root");
        _action = GetComponents<CharacterAction>();
        _myCollider = GetComponent<CircleCollider2D>();
        _myCollider.radius = 0.35f;
        _sortingGroup = GetComponent<SortingGroup>();

        _movement = GetComponent<CharacterMovement>();
        _animator = GetComponent<CharacterAnimator>();
        _characterVisual = GetComponent<CharacterGraphics>();

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
    }

    protected void Update()
    {
        if(_movement.HasTarget || !_isDead)
        {
            if (_movement.Target != null)
            {
                _characterVisual.FlipTo(transform, _movement.Target.transform);
            }
        }

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
        _animator.SetIdle(true);
        if (BattleStateManager.Instance.IsEnteringBattle)
            _curState = CharacterState.Move;
    }

    protected virtual void MoveStateAction()
    {
        _animator.SetIdle(false);
    }

    protected virtual void AttackStateAction()
    {
        // 공격 중이면 애니메이션 끝났는지 체크
        if (CheckAttackAnimationState())
        {
            return;
        }

        // 공격 시작
        StartAttack();
    }

    protected bool CheckAttackAnimationState()
    {
        if (!_isAttacking)
            return false;

        // 애니메이션 끝났는데 타겟이 없다면 이동 상태로
        if (_animator.IsAnimationFinished())
        {
            _isAttacking = false;

            if(_movement.Target == null)
            {
                _curState = CharacterState.Move;
                return true;
            }

            float dist = Vector2.Distance(transform.position, _movement.Target.transform.position);

            if (dist > _data.AtkRange)
            { 
                _curState = CharacterState.Move; 
            }
        }

        return true; // 공격 중이었음
    }

    protected virtual void StartAttack()
    {
        if (!_movement.TargetColliderEnable)
        {
            _curState = CharacterState.Move;
            return;
        }

        _isAttacking = true;
        _animator.SetTrigger("Attack");
    }

    protected virtual void DeadStateAction()
    {
        if (_isDead) return;

        _isDead = true; 
        _curState = CharacterState.Dead;

        _onDie?.Invoke();
        _animator.SetTrigger("Dead");
        _myCollider.enabled = false;
        //_sortingGroup.sortingOrder = -10000;

        Despawn();
    }

    /*protected Collider2D FindTarget(string targetTag, float range)
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
    }*/

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
    public void ApplyAttackSlow(HitType type, float duration, float speed)
    {
        _characterVisual.PlayStatusColor(type, duration, speed);
    }

    public void SetAnimatorSpeed(float speed)
    {
        _animLengthRate = 1 / speed;
        _animator.SetSpeed(speed);
    }

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
    private void Despawn()
    {
        _characterVisual.StartFadeOutAndDisable();
    }

    public TargetSelector GetSelector(ActionSlot slot)
    {
        return _targets[(int)slot];
    }
}