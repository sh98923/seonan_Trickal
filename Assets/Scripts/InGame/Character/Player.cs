using UnityEngine;
using System.Collections;

public class Player : Character
{
    private GameObject _skillEffect;

    private Vector3 _originPos;

    private float _curMp = 0.0f;

    private bool _isFront = false;

    private void Awake()
    {
        base.Awake();

        Transform lastChild = transform.GetChild(transform.childCount - 1);
        _skillEffect = lastChild.gameObject;
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
            _isFront = true;
            _animator.SetTrigger("Ult"); 
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            _animator.SetTrigger("Attack");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _isFront = true;
            _animator.SetTrigger("Skill");
        }
    }

    private void SkillEffectOn()
    {
        int sortNum = _isFront ? _sortingGroup.sortingOrder + 1 : -101;

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
                if (FindTarget(_data.Target, _attackRange) == null)
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

        if (Input.GetKeyDown(KeyCode.A))
        {
            _animator.SetTrigger("Skill");
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            _animator.SetTrigger("Attack");
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            _animator.SetTrigger("Ult");
        }

        //StartCoroutine(SetCoolTime());
    }

    /* public void OnAttackHit()
     {
         _targetCollider.GetComponent<Monster>().TakeDamage(_characterData.Atk);
     }*/

    public void OnSkillBuff()
    {
        _action[(int)ActionType.Buff].Excute();
    }

    public void OnSkillHit()
    { 
        float skillAtk = _data.Atk * _data.SkillRate;
        _curMp -= _data.Mp;
        _action[(int)ActionType.Attack].SetAttackInfo(_targetCollider, AttackType.Skill, skillAtk, _data.IsRangeAttack[(int)AttackType.Skill]);
        _action[(int)ActionType.Attack].Excute();
    }

    public void OnUltHit()
    {
        float skillAtk = _data.Atk * _data.Ultimate;
        _action[(int)ActionType.Attack].SetAttackInfo(_targetCollider, AttackType.Ult, skillAtk, _data.IsRangeAttack[(int)AttackType.Ult]);
        _action[(int)ActionType.Attack].Excute();
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