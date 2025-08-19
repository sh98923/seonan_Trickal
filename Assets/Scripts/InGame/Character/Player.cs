using UnityEngine;
using System.Collections;

public class Player : Character
{
    private GameObject _skillEffect;

    private Vector3 _originPos;

    private float _atkBuff = 1.0f;
    public float AtkBuff
    {
        get { return _atkBuff; }
        set { _atkBuff = value; }
    }

    private float _curMp = 0.0f;

    private void Awake()
    {
        base.Awake();

        _skillEffect = transform.Find("SkillEffect").gameObject;

        SkillEffectOff();

        _moveDir = Vector2.right;
        
        _scale.x *= -1;
        transform.localScale = _scale;

        _curHp = 5500;
    }

    private void Start()
    {
        _animator.SetTrigger("Intro");
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
        int sortNum = _data.IsEffectInFront[(int)_actionType] ? _sortingGroup.sortingOrder + 1 : -101;

        _skillEffect.GetComponent<SpriteRenderer>().sortingOrder = sortNum;
        _skillEffect.SetActive(true);
    }

    private void SkillEffectOff()
    {
        _skillEffect.SetActive(false);
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

        if (BattleStateManager.Instance.CurrentState == BattleState.MonstersDefeated)
        {
            Vector3 dir = _originPos - transform.position;
            transform.Translate(dir.normalized * _moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _originPos) < 0.01f)
            {
                BattleStateManager.Instance.SetState(BattleState.Reroll);
                _curState = State.Idle;
            }
        }
        else if (BattleStateManager.Instance.CurrentState == BattleState.Battle)
        {
            if (_targetCollider == null)
            {
                _targetCollider = FindTarget(_data.Target, _findTargetRange);
                transform.Translate(_moveDir * _moveSpeed * Time.deltaTime);
            }
            else
            {
                if (FindTarget(_data.Target, _data.AtkRange) == null)
                {
                    Vector2 dir = _targetCollider.transform.position - transform.position;
                    transform.Translate(dir.normalized * _moveSpeed * Time.deltaTime);
                }
                else
                {
                    _animator.SetBool("IdleState", true);
                    _curState = State.Attack;
                }
            }
        }
    }

    protected override void AttackStateAction()
    {
        if (!_targetCollider.enabled)
        {
            _targetCollider = null;
            _curState = State.Move;
        }

        // 몬스터 전멸
        if (BattleStateManager.Instance.CurrentState == BattleState.MonstersDefeated)
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
        _action[(int)ActionCategory.Buff].SetBuffInfo(_actionType, _data.ClipName[(int)_actionType], _data.EffectValue, _data.Duration[(int)_actionType]);
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

        _action[(int)ActionCategory.Attack].SetAttackInfo(_targetCollider, _actionType, finalDamage);
        _action[(int)ActionCategory.Attack].Excute();
    }

    public void PlaySkillEffect()
    {
        //_skillEffect.SetActive(true);
        //_skillEffect.GetComponent<Animator>()?.Play("SkillEffect");
        StartCoroutine(DisableEffectAfterTime(1.0f)); 
    }

    private IEnumerator DisableEffectAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        //_skillEffect.SetActive(false);
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
}