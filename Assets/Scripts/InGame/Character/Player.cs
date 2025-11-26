using UnityEngine;
using System.Collections;
using System;

public class Player : Character
{
    private SkillEffectController _skillEffectController;

    private Vector3 _wayPoint;

    private readonly float _nextWaveDist = 23.0f; // 배틀 → 리롤 이동 시 플레이어 이동 거리

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

        _skillEffectController = GetComponent<SkillEffectController>();
        _skillEffectController.Initialize(this);

        _skillEffectController.Stop();

        _moveDir = Vector2.right;

        _scale.x *= -1;
        transform.localScale = _scale;

    }

    private void Start()
    {
        _wayPoint = transform.position;
        _animator.SetTrigger("Intro");
    }

    private void OnEnable()
    {
        base.OnEnable();
        //BattleStateManager.Instance.OnReroll += MoveToNextWaypoint;
        BattleStateManager.Instance.OnWaveAdvance += MoveToNextWaypoint;
    }

    private void OnDisable()
    {
        //BattleStateManager.Instance.OnReroll -= MoveToNextWaypoint;
        BattleStateManager.Instance.OnWaveAdvance -= MoveToNextWaypoint;
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
        _wayPoint.x += _nextWaveDist;
        print("다음 위치 : " + _wayPoint.x);

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
            _curState = State.Attack;
        }
        else
        {
            Vector2 dir = (_targetCollider.transform.position - transform.position).normalized;
            transform.Translate(dir * _moveSpeed * Time.deltaTime);
        }
    }

    private void HandleWaveAdvanceMovement()
    {
        Vector3 dir = _wayPoint - transform.position;
        transform.Translate(dir.normalized * _moveSpeed * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, _wayPoint) < 0.001f && !_isArrived)
        {
            _isArrived = true;

            print($"{gameObject.name} : 도착 보고");
            InGamePlayerSpawn parent = transform.parent.GetComponent<InGamePlayerSpawn>();
            parent.CheckNextWaveReady();
        }
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
            _curState = State.Move;
        }

        // 몬스터 전멸
        if (BattleStateManager.Instance.CurrentState == BattleState.Victory)
        {
            _curState = State.Move;
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

    public void ResetArrivalState()
    {
        _isArrived = false;
    }
}