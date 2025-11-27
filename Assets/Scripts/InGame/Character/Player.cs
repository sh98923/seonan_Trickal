using UnityEngine;
using System.Collections;
using System;

public class Player : Character
{
    private PlayerHealth _playerHealth;
    public PlayerHealth Health
    {
        get { return _playerHealth; }
    }

    private SkillEffectController _skillEffectController;

    private Vector3 _originalWayPoint = new Vector3();
    private Vector3 _nextWayPoint = new Vector3();

    private float _nextWaveDist = 0.0f; // 배틀 → 리롤 이동 시 플레이어 이동 거리 변수
    private float _atkBuff = 1.0f;
    private bool _isArrived = false;
    public float AtkBuff
    {
        get { return _atkBuff; }
        set { _atkBuff = value; }
    }

    private float _curMp = 0.0f;

    private void Awake()
    {
        base.Awake();

        _playerHealth = GetComponent<PlayerHealth>();

        _skillEffectController = GetComponent<SkillEffectController>();
        _skillEffectController.Initialize(this);

        _skillEffectController.Stop();

        _moveDir = Vector2.right;

        _scale.x *= -1;
        transform.localScale = _scale;
    }

    private void Start()
    {
        _originalWayPoint = transform.position;
        _animator.SetTrigger("Intro");
    }

    private void OnEnable()
    {
        base.OnEnable();

        BattleStateManager.Instance.OnReroll += EnableCollider;
        BattleStateManager.Instance.OnWaveAdvance += DisableCollider;
        BattleStateManager.Instance.OnWaveAdvance += MoveToNextWaypoint;
    }

    private void OnDisable()
    {
        BattleStateManager.Instance.OnReroll -= EnableCollider;
        BattleStateManager.Instance.OnWaveAdvance -= DisableCollider;
        BattleStateManager.Instance.OnWaveAdvance -= MoveToNextWaypoint;
    }

    private void OnDestroy()
    {
        if (BattleStateManager.Instance != null)
        {
            BattleStateManager.Instance.OnReroll -= EnableCollider;
            BattleStateManager.Instance.OnWaveAdvance -= DisableCollider;
            BattleStateManager.Instance.OnWaveAdvance -= MoveToNextWaypoint;
        }
    }

    private void Update()
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

        /*_scale.x = -Mathf.Abs(_scale.x);
        transform.localScale = _scale;

        print(gameObject.name + " : " + _scale.x);*/
    }

    protected override void IdleStateAction()
    {
        base.IdleStateAction();

        if (BattleStateManager.Instance.IsBattle)
        {
            StartCoroutine(RegenerateMp());
        }
    }

    protected override void MoveStateAction()
    {
        base.MoveStateAction();

        switch (BattleStateManager.Instance.CurrentState)
        {
            case BattleState.Reroll:
                HandleRerollMovement();
                break;
            case BattleState.Battle:
                HandleBattleMovement();
                break;
            case BattleState.WaveAdvance:
                HandleWaveAdvanceMovement();
                break;
        }

        //print(BattleStateManager.Instance.CurrentState);
    }

    private void HandleRerollMovement()
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
    
    private void HandleBattleMovement()
    {
        /*if (_targetCollider == null)
        {
            _targetCollider = FindTarget(_data.Target, _findTargetRange);
            transform.Translate(_moveDir * _moveSpeed * Time.deltaTime);
            print(_moveDir + " : " + _moveSpeed);
        }
        else
        {
            Character targetCharacter = _targetCollider.GetComponent<Character>();
            if (targetCharacter == null)
            {
                //BattleStateManager.Instance.SetState(BattleState.Reroll);
                _curState = State.Idle;
                _targetCollider = null;
                return;
            }

            int sortingDiff = Mathf.Abs(_sortingGroup.sortingOrder - targetCharacter.SortingIndex);

            if (Vector2.Distance(_targetCollider.transform.position, transform.position) <= _data.AtkRange)
            //&& sortingDiff <= 20) // SortingOrder 차이 20 이내
            {
                _animator.SetBool("IdleState", true);
                _curState = State.Attack;
            }
            else
            {
                Vector2 dir = (_targetCollider.transform.position - transform.position).normalized;
                transform.Translate(dir * _moveSpeed * Time.deltaTime);
                print(dir + " : " + _moveSpeed);
            }
        }*/

        if (_targetCollider == null)
        {
            _targetCollider = FindTarget(_data.Target, _findTargetRange);
            transform.Translate(_moveDir * _moveSpeed * Time.deltaTime);
            return;
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
        }
    }

    private void HandleWaveAdvanceMovement()
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
        if (!_targetCollider.enabled)
        {
            _targetCollider = null;
            _curState = CharacterState.Move;
        }

        // 몬스터 전멸
        if (BattleStateManager.Instance.CurrentState == BattleState.Victory)
        {
            _curState = CharacterState.Move;
            return;
        }

        if (_isAttacking)
            return;

        _isAttacking = true;

        if (_curMp >= _data.Mp)
        {
            _actionType = ActionSlot.Skill;
            _animator.SetTrigger("Skill");
        }
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

    public void OnSoloBuff()
    {
        _action[(int)ActionCategory.Buff].Excute();
    }

    public void OnAllBuff()
    {
        _clipName = _data.ClipName[(int)_actionType];
        _duration = _data.Duration[(int)_actionType];

        _action[(int)ActionCategory.Buff].SetBuffInfo(_actionType, _clipName, _data.EffectValue, _duration);
        _action[(int)ActionCategory.Buff].Excute();
    }

    public override void OnAttack()
    {
        float finalDamage = 0.0f;

        switch (_actionType)
        {
            case ActionSlot.Base:
                finalDamage = _data.Atk * _atkBuff;
                break;
            case ActionSlot.Skill:
                _curMp -= _data.Mp;
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

        _action[(int)ActionCategory.Attack].SetAttackInfo(_targetCollider, _actionType, _clipName, _duration, finalDamage);

        base.OnAttack();
    }

    private IEnumerator RegenerateMp()
    {
        while (BattleStateManager.Instance.IsBattle)
        {
            if (_curMp >= _data.Mp)
            {
                yield return new WaitForSeconds(1.0f);
                continue;
            }

            _curMp += 10.0f;
            yield return new WaitForSeconds(1.0f);
        }
    }

    public CharacterState CurState()
    {
        return _curState; 
    }

    public override void SetCharacterData(CharacterData data)
    { 
        _data = data;
        _damageReceiver.MaxHp = data.Hp;
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
}