using System.Collections;
using UnityEngine;

public class Player : Character
{
    public enum TargetCount
    {
        None,      // 타겟 없음
        Single,    // 단일 타겟
        Multiple   // 다중 타겟
    }

    private PlayerHp _playerHealth;
    public PlayerHp Health
    {
        get { return _playerHealth; }
    }

    // 이건 다시 private으로 ㄱ
    protected PlayerMp _playerMp;
    protected Collider2D _attackTarget;
    protected SkillEffectController _skillEffectController;
    private PlayerMovement _playerMovement;

    protected float _atkBuff = 1.0f;
    private float _damageReduction = 1.0f;

    public float AtkBuff
    {
        get { return _atkBuff; }
        set { _atkBuff = value; }
    }

    public float DamageReduction
    {
        get { return _damageReduction; }
        set { _damageReduction = value; }
    }

    private bool _isArrived = false;
    private bool _isInBattle = false;

    protected void Awake()
    {
        base.Awake();

        _playerHealth = GetComponentInChildren<PlayerHp>();
        _playerMp = GetComponentInChildren<PlayerMp>();

        _skillEffectController = GetComponent<SkillEffectController>();

        _scale.x *= -1;
        transform.localScale = _scale;
    }

    protected void Start()
    {
        _playerMovement = _movement as PlayerMovement;
        _animator.SetTrigger("Intro");
    }

    protected void OnEnable()
    {
        base.OnEnable();

        BattleStateManager.Instance.OnReroll += EnableCollider;
        BattleStateManager.Instance.OnEnteringReroll += DisableCollider;
    }

    protected void OnDisable()
    {
        BattleStateManager.Instance.OnReroll -= EnableCollider;
        BattleStateManager.Instance.OnEnteringReroll -= DisableCollider;
    }

    protected void OnDestroy()
    {
        if (BattleStateManager.Instance != null)
        {
            BattleStateManager.Instance.OnReroll -= EnableCollider;
            BattleStateManager.Instance.OnEnteringReroll -= DisableCollider;
        }
    }

    protected void Update()
    { 
        base.Update();

        if (Input.GetKeyDown(KeyCode.S))
        {
            _actionType = ActionSlot.Ult;
            _animator.SetTrigger("Ult");
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            _actionType = ActionSlot.Attack;
            _animator.SetTrigger("Attack");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _actionType = ActionSlot.Skill;
            _animator.SetTrigger("Skill");
        }
    }

    protected override void MoveStateAction()
    {
        base.MoveStateAction();

        switch (BattleStateManager.Instance.CurrentState)
        {
            case BattleState.Battle:
            case BattleState.EnteringBattle:
                CheckForTargetAndEnterBattle();
                break;
            case BattleState.EnteringReroll:
                EnteringRerollMovement();
                break;
        }
    }

    private void CheckForTargetAndEnterBattle()
    {
        if (!_isInBattle)
        {
            TryFindTargetOrKeepMoving();
        }
        else
        {
            BattleMovement();
        }
    }

    private void TryFindTargetOrKeepMoving()
    {
        _movement.SetMovementActive(true);
        //_targetCollider = FindTarget(_data.Target, _findTargetRange);

        // 타겟이 있다면
        if (_movement.HasTarget)
        {
            Character target = _movement.Target;
            _skillEffectController.Initialize(target);

            _isInBattle = true;
            print(_data.Target + " 발견");

            return; // 발견한 프레임에서 BattleMovement 실행 X
        }

        // 조우 전까지 계속 이동 (후열 캐릭터 움직임 유지)
        //transform.Translate(_moveDir * _moveSpeed * Time.deltaTime);
    }

    private void BattleMovement()
    {
        // 타겟이 사거리 안에 들어오면 공격 상태로
        float dist = Vector2.Distance(transform.position, _movement.Target.transform.position);

        if (dist <= _data.AtkRange)
        {   
            // 전체 전역 스테이트 전환은 한번만
            if (BattleStateManager.Instance.CurrentState != BattleState.Battle)
            {
                BattleStateManager.Instance.SetState(BattleState.Battle);
            }

            _animator.SetIdle(true);
            _movement.SetMovementActive(false);
            _curState = CharacterState.Attack;
        }
    }

    private void EnteringRerollMovement()
    {
        _playerMovement.EnteringRerollMovement();

        if (!_isArrived && _playerMovement.IsArrived())
        {
            _isArrived = true;
            _isInBattle = false;
            InGamePlayerSpawn.Instance.CheckNextWaveReady();
        }
    }

    private void EnableCollider()
    {
        GetComponent<Collider2D>().enabled = true;
    }

    private void DisableCollider()
    {
        GetComponent<Collider2D>().enabled = false;
    }

    /*int sortingDiff = Mathf.Abs(_sortingGroup.sortingOrder - targetCharacter.SortingIndex);

    if (Vector2.Distance(_targetCollider.transform.position, transform.position) <= _data.AtkRange
        && sortingDiff <= 20) // SortingOrder 차이 20 이내
    {
        _animator.SetBool("IdleState", true);
        _curState = State.Attack;
    }*/

    protected override void StartAttack()
    {
        if(_movement.Target == null)
        {
            return;
        }

        _isAttacking = true;

        if (_playerMp.GetCurMp() >= _data.Mp)
        {
            _actionType = ActionSlot.Skill;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            _actionType = ActionSlot.Ult;
        }
        else
        {
            _actionType = ActionSlot.Attack;
        }

        _attackTarget = _targets[(int)_actionType].GetTarget(this).GetComponent<Collider2D>();
        // 트리거 실행 전에 항상 이펙트 정렬 세팅
        _skillEffectController.SetSortingPosition(_data.IsEffectInFront[(int)_actionType]);
        _animator.SetTrigger(_actionType.ToString());
    }

    public void OnBuff()
    {
        if (_actionType == ActionSlot.Skill)
        {
            _playerMp.UseMp();
        }

        _clipName = _data.ClipName[(int)_actionType];
        _duration = _data.Duration[(int)_actionType];

        _action[(int)ActionCategory.Buff].SetBuffInfo(_actionType, _clipName, _data.EffectValue[(int)_actionType], _duration);
        _action[(int)ActionCategory.Buff].Excute();
    }

    public override void OnAttack()
    {
        if (BattleStateManager.Instance.CurrentState == BattleState.EnteringReroll)
            return;

        // clip, duration 세팅 (기존 동작 유지)
        if (tag == "Player")
        {
            _clipName = _data.ClipName[(int)_actionType];
            _duration = _data.Duration[(int)_actionType];
        }

        // 공통 전처리(콤보 카운트, 쿨다운 등)를 자식에 위임
        PreHit(_actionType);
        
        // 타겟 가져오기
        Collider2D[] targets = GetTargets(_actionType);

        // 데미지 계산
        float finalDamage = CalculateDamage(_actionType);

        TargetCount targetCount;

        switch (targets.Length)
        {
            case 0:
                targetCount = TargetCount.None;
                break;
            case 1:
                targetCount = TargetCount.Single;
                break;
            default: // 2 이상
                targetCount = TargetCount.Multiple;
                break;
        }

        switch (targetCount)
        {
            case TargetCount.None:
                // 빈 타겟이면 기본 동작: 단일 타겟이 있으면 그걸 사용
                if (_attackTarget != null && _curState != CharacterState.Dead)
                {
                    _action[(int)ActionCategory.Attack].SetAttackInfo(_attackTarget, _actionType, _clipName, _duration, finalDamage);
                }
                else
                {
                    // 타겟이 아예 없으면 그래도 호출해 둠(기존 흐름 유지)
                    _action[(int)ActionCategory.Attack].SetAttackInfo((Collider2D)null, _actionType, _clipName, _duration, finalDamage);
                }
                break;
            case TargetCount.Single:
                _action[(int)ActionCategory.Attack].SetAttackInfo(targets[0], _actionType, _clipName, _duration, finalDamage);
                break;
            case TargetCount.Multiple:
                // 다중 타겟용 오버로드가 있을 경우 배열 전달 (기존 Destroyer 코드와 호환)
                _action[(int)ActionCategory.Attack].SetAttackInfo(targets, _actionType, _clipName, _duration, finalDamage);
                break;
        }

        base.OnAttack();
    }

    // ---------------------------
    // --- 확장 포인트(자식 오버라이드) ---
    // ---------------------------
    protected virtual float CalculateDamage(ActionSlot slot)
    {
        // 기본(기존 Player.OnAttack 동작 재현)
        float finalDamage = 0.0f;

        switch (slot)
        {
            case ActionSlot.Attack:
                finalDamage = _data.Atk * _atkBuff;
                break;
            case ActionSlot.Skill:
                _playerMp.UseMp();
                finalDamage = _data.Atk * _data.SkillRate * _atkBuff;
                break;
            case ActionSlot.Ult:
                finalDamage = _data.Atk * _data.Ultimate * _atkBuff;
                break;
        }

        return finalDamage;
    }

    protected virtual Collider2D[] GetTargets(ActionSlot slot)
    {
        // 기본은 단일 타겟(타겟이 없으면 빈 배열 반환)
        if (_attackTarget != null)
            return new Collider2D[] { _attackTarget };

        return new Collider2D[0];
    }

    protected virtual void PreHit(ActionSlot slot)
    {
        // 기본 전처리 없음. 캐릭터별로 오버라이드해서 콤보/위상/카운트 처리
    }

    // 실제 이펙트가 있는 위치를 반환
    protected Vector2 GetUltHitBoxCenter()
    {
        Vector2 worldPos = transform.position;
        Vector2 localPos = _skillEffectController.GetPosition();

        // 방향 고려
        localPos.x *= transform.localScale.x;

        return worldPos + localPos;
    }

    protected bool IsMonster(Collider2D target)
    {
        return target.CompareTag("Monster");
    }

    protected bool IsWithinYOffset(float targetY, float yOffsetLimit)
    {
        return Mathf.Abs(targetY - transform.position.y) <= yOffsetLimit;
    }

    public CharacterState CurState()
    {
        return _curState; 
    }

    public override void SetCharacterData(CharacterData data)
    { 
        _data = data;
        _damageReceiver.MaxHp = data.Hp;
        _playerMp.SetMpData(_data.Mp, _data.MpTickRate);
    }

    public void ResetArrivalState()
    {
        _isArrived = false;
    }

    public void PlayEnterAnim()
    {
        _animator.SetTrigger("Intro");

        if (InGameManager.Instance.IsGameStart)
        {
            StartCoroutine(CheckIntroEnd());
        }
    }

    private IEnumerator CheckIntroEnd()
    {
        yield return null;

        bool entered = false;

        while (true)
        {
            // Intro를 진입했다가 빠져나온 순간
            if (_animator.HasEnteredThenExited("Intro", ref entered))
            {
                break;
            }

            yield return null;
        }

        _curState = CharacterState.Move;
    }

    public void RevivePlayer(float camPosX)
    {
        // 1. 도착 상태 초기화
        ResetArrivalState();

       /* // 2. 원래 위치 + 카메라 X 좌표 조정
        Vector3 spawnPos = transform.position;//_originalWayPoint;
        spawnPos.x += camPosX; // 카메라 X 위치 만큼 더함
        transform.position = spawnPos;*/

        // 필요하다면 시각적 표시 초기화 등 추가 가능
        //_graphics?.ResetVisuals();
    }

    public void ApplyDamageReduction(float damageReduction)
    {
        _damageReceiver.UpdateDamageReduction(damageReduction);
    }
}