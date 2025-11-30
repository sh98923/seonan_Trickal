using System.Collections;
using UnityEngine;

public class Player : Character
{
    private PlayerHealth _playerHealth;
    public PlayerHealth Health
    {
        get { return _playerHealth; }
    }

    private PlayerMp _playerMp;

    private Collider2D _attackTarget;
    private SkillEffectController _skillEffectController;

    private Vector3 _originalWayPoint = new Vector3();
    private Vector3 _nextWayPoint = new Vector3();

    private float _nextWaveDist = 0.0f; // 배틀 → 리롤 이동 시 플레이어 이동 거리 변수
    private float _atkBuff = 1.0f;
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

        _playerHealth = GetComponent<PlayerHealth>();
        _playerMp = GetComponent<PlayerMp>();

        _skillEffectController = GetComponent<SkillEffectController>();
        _skillEffectController.Stop();

        _moveDir = Vector2.right;
        _scale.x *= -1;
        transform.localScale = _scale;
    }

    protected void Start()
    {
        _originalWayPoint = transform.position;
        _animator.SetTrigger("Intro");
    }

    protected void OnEnable()
    {
        base.OnEnable();

        BattleStateManager.Instance.OnReroll += EnableCollider;
        BattleStateManager.Instance.OnEnteringReroll += DisableCollider;
        BattleStateManager.Instance.OnEnteringReroll += MoveToNextWaypoint;
    }

    protected void OnDisable()
    {
        BattleStateManager.Instance.OnReroll -= EnableCollider;
        BattleStateManager.Instance.OnEnteringReroll -= DisableCollider;
        BattleStateManager.Instance.OnEnteringReroll -= MoveToNextWaypoint;
    }

    protected void OnDestroy()
    {
        if (BattleStateManager.Instance != null)
        {
            BattleStateManager.Instance.OnReroll -= EnableCollider;
            BattleStateManager.Instance.OnEnteringReroll -= DisableCollider;
            BattleStateManager.Instance.OnEnteringReroll -= MoveToNextWaypoint;
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
            _actionType = ActionSlot.Base;
            _animator.SetTrigger("Attack");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _actionType = ActionSlot.Skill;
            _animator.SetTrigger("Skill");
        }
    }

    private void SkillEffectOn()
    {
        if(!_targetCollider.enabled)
        {
            return;
        }

        Character target = _targetCollider.GetComponent<Character>();

        _skillEffectController.Initialize(target);
        _skillEffectController.Play(_data.IsEffectInFront[(int)_actionType]);
    }

    private void SkillEffectOff()
    {
        _skillEffectController.Stop();
    }

    private void MoveToNextWaypoint()
    {
        // waypoint 이동
        _nextWayPoint = _originalWayPoint;
        _nextWayPoint.x += _nextWaveDist;
        print("다음 목적지 : " + _nextWayPoint);
        /*_scale.x = -Mathf.Abs(_scale.x);
        transform.localScale = _scale;

        print(gameObject.name + " : " + _scale.x);*/
    }

    /*protected override void IdleStateAction()
    {
        base.IdleStateAction();

        if (BattleStateManager.Instance.IsBattle)
        {
            StartCoroutine(RegenerateMp());
        }
    }*/

    protected override void MoveStateAction()
    {
        base.MoveStateAction();

        switch (BattleStateManager.Instance.CurrentState)
        {
           /* case BattleState.Reroll:
                RerollMovement();
                break;*/
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
        _targetCollider = FindTarget(_data.Target, _findTargetRange);

        // 타겟을 발견한 순간
        if (_targetCollider != null)
        {
            _isInBattle = true;

            return; // 발견한 프레임에서 BattleMovement 실행 X
        }

        // 조우 전까지 계속 이동 (후열 캐릭터 움직임 유지)
        transform.Translate(_moveDir * _moveSpeed * Time.deltaTime);
    }

    private void BattleMovement()
    {
        // 전체 전역 스테이트 전환은 한번만
        if (BattleStateManager.Instance.CurrentState != BattleState.Battle)
        { 
            BattleStateManager.Instance.SetState(BattleState.Battle);
        }

        // 이미 전투 상태인데 타겟이 죽거나 null이면 재탐색
        if (_targetCollider == null || !_targetCollider.enabled)
        {
            _targetCollider = FindTarget(_data.Target, _findTargetRange);

            if (_targetCollider == null)
            {
                // 타겟 잃으면 다시 전진하며 재탐색
                transform.Translate(_moveDir * _moveSpeed * Time.deltaTime);
                return;
            }
        }

        Character targetCharacter = _targetCollider.GetComponent<Character>();
        if (targetCharacter == null)
        {
            _targetCollider = null;
            return;
        }

        float dist = Vector2.Distance(_targetCollider.transform.position, transform.position);

        if (dist <= _data.AtkRange)
        {
            _animator.SetBool("IdleState", true);
            _curState = CharacterState.Attack;
        }
        else
        {
            Vector2 dir = (_targetCollider.transform.position - transform.position).normalized;
            transform.Translate(dir * _moveSpeed * Time.deltaTime);
        }

        /*if (_targetCollider == null)
        {
            _targetCollider = FindTarget(_data.Target, _findTargetRange);
            transform.Translate(_moveDir * _moveSpeed * Time.deltaTime);
            print(gameObject.name + " : " + BattleStateManager.Instance.CurrentState);
        }

        Character targetCharacter = _targetCollider.GetComponent<Character>();
        if (targetCharacter == null)
        {
            _targetCollider = null;
            return;
        }

        if (Vector2.Distance(_targetCollider.transform.position, transform.position) - 0.5f <= _data.AtkRange)
        {
            _animator.SetBool("IdleState", true);
            _curState = CharacterState.Attack;
        }
        else
        {
            Vector2 dir = (_targetCollider.transform.position - transform.position).normalized;
            transform.Translate(dir * _moveSpeed * Time.deltaTime);
        }*/
    }

    private void RerollMovement()
    {
        //print($"{gameObject.name} 준비완료");
        /*if (Vector3.Distance(transform.position, _wayPoint) <  0.001f)
        {
            _curState = State.Idle;
        }
        else
        {
            float a = Vector3.Distance(transform.position, _wayPoint);

            print($"{gameObject.name} 거리 : {a}");
        }*/
    }

    private void EnteringRerollMovement()
    {
        Vector3 dir = _nextWayPoint - transform.position;
        float distanceCurFrame = _moveSpeed * Time.deltaTime;

        if (dir.magnitude <= distanceCurFrame)
        {
            // 목표 위치까지 남은 거리가 이번 프레임 이동 거리보다 작으면 정확히 도착
            transform.position = _nextWayPoint;
            if (!_isArrived)
            {
                _isArrived = true;
                _isInBattle = false;
                InGamePlayerSpawn parent = transform.parent.GetComponent<InGamePlayerSpawn>();
                parent.CheckNextWaveReady();
            }
        }
        else
        {
            // 아직 목표까지 멀면 계속 이동
            transform.Translate(dir.normalized * distanceCurFrame);
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

    /*protected override void MoveStateAction()
    {
        
        if (_targetCollider == null)
        {
            _targetCollider = FindTarget(_data.Target, _findTargetRange);
            transform.Translate(_moveDir * _moveSpeed * Time.deltaTime);
        }
        else
        {
            Character targetCharacter = _targetCollider.GetComponent<Character>();
            if (targetCharacter == null)
            {
                _targetCollider = null;
                return;
            }

            int sortingDiff = Mathf.Abs(_sortingGroup.sortingOrder - targetCharacter.SortingIndex);

            if (Vector2.Distance(_targetCollider.transform.position, transform.position) <= _data.AtkRange)
                //&& sortingDiff <= 20) // SortingOrder 차이 20 이내
            {
                _targetCollider = FindTarget(_data.Target, _findTargetRange);
                transform.Translate(_moveDir * _moveSpeed * Time.deltaTime);
                _animator.SetBool("IdleState", true);
                _curState = State.Attack;
            }
            else
            {
                Character targetCharacter = _targetCollider.GetComponent<Character>();
                if (targetCharacter == null)
                {
                    _targetCollider = null;
                    return;
                }

                int sortingDiff = Mathf.Abs(_sortingGroup.sortingOrder - targetCharacter.SortingIndex);

                if (Vector2.Distance(_targetCollider.transform.position, transform.position) <= _data.AtkRange
                    && sortingDiff <= 20) // SortingOrder 차이 20 이내
                {
                    _animator.SetBool("IdleState", true);
                    _curState = State.Attack;
                }
                else
                {
                    Vector2 dir = (_targetCollider.transform.position - transform.position).normalized;
                    transform.Translate(dir * _moveSpeed * Time.deltaTime);
                }
                Vector2 dir = (_targetCollider.transform.position - transform.position).normalized;
                transform.Translate(dir * _moveSpeed * Time.deltaTime);
            }
        }
    }
        
    }*/

    protected override void AttackStateAction()
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
        /*if (!_targetCollider.enabled)
        {
            _targetCollider = null;
            _curState = CharacterState.Move;
        }*/

        /*// 몬스터 전멸
        if (BattleStateManager.Instance.CurrentState == BattleState.Victory)
        {
            _curState = CharacterState.Move;
            return;
        }*/

        /*if (_isAttacking)
            return;

        _isAttacking = true;

        if (_playerMp.GetCurMp() >= _data.Mp)
        {
            _actionType = ActionSlot.Skill;
            _animator.SetTrigger("Skill");
        }
        // 궁극기는 업그레이드 3레벨, battle 상태일 때 UI 터치 시 발동 (쿨타임 있음)
        else if (Input.GetKeyDown(KeyCode.S)) 
        {
            _actionType = ActionSlot.Ult;
            _animator.SetTrigger("Ult");
        }
        else
        {
            _actionType = ActionSlot.Base;
            _animator.SetTrigger("Attack");
        }*/
    }

    protected override void StartAttack()
    {
        if (!_targetCollider.enabled)
            return;

        _isAttacking = true;

        _attackTarget = _targetCollider;

        if (_playerMp.GetCurMp() >= _data.Mp)
        {
            _actionType = ActionSlot.Skill;
            _animator.SetTrigger("Skill");
        }
        // 궁극기는 업그레이드 3레벨, battle 상태일 때 UI 터치 시 발동 (쿨타임 있음)
        else if (Input.GetKeyDown(KeyCode.S))
        {
            _actionType = ActionSlot.Ult;
            _animator.SetTrigger("Ult");
        }
        else
        {
            _actionType = ActionSlot.Base;
            _animator.SetTrigger("Attack");
        }
    }

    public void OnBuff()
    {
        if (_actionType == ActionSlot.Skill)
        {
            _playerMp.UseMp();
        }

        _clipName = _data.ClipName[(int)_actionType];
        _duration = _data.Duration[(int)_actionType];

        _action[(int)ActionCategory.Buff].SetBuffInfo(_actionType, _clipName, _data.EffectValue, _duration);
        _action[(int)ActionCategory.Buff].Excute();
    }

    public override void OnAttack()
    {
        if(BattleStateManager.Instance.CurrentState == BattleState.EnteringReroll)
        {
            return;
        }

        float finalDamage = 0.0f;

        switch (_actionType)
        {
            case ActionSlot.Base:
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

        if (tag == "Player")
        {
            _clipName = _data.ClipName[(int)_actionType];
            _duration = _data.Duration[(int)_actionType];
        }

        _action[(int)ActionCategory.Attack].SetAttackInfo(_attackTarget, _actionType, _clipName, _duration, finalDamage);

        base.OnAttack();
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

    public void SetNextWaveX(float dist)
    {
        _nextWaveDist = dist;
    }

    public void ResetArrivalState()
    {
        _isArrived = false;
    }

    public void ReviveAnim()
    {
        _animator.SetTrigger("Intro");
        StartCoroutine(CheckIntroEnd());
    }

    private IEnumerator CheckIntroEnd()
    {
        yield return null;

        bool entered = false;

        while (true)
        {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

            // Intro로 진입한 적 있는지 체크
            if (stateInfo.IsName("Intro"))
            {
                entered = true;
            }
            else
            {
                // Intro 끝나면 move 상태로 변경
                if (entered)
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

        // 2. 원래 위치 + 카메라 X 좌표 조정
        Vector3 spawnPos = _originalWayPoint;
        spawnPos.x += camPosX; // 카메라 X 위치 만큼 더함
        transform.position = spawnPos;

        // 필요하다면 시각적 표시 초기화 등 추가 가능
        //_graphics?.ResetVisuals();
    }

    public void ApplyDamageReduction(float damageReduction)
    {
        _damageReceiver.UpdateDamageReduction(damageReduction);
    }
}