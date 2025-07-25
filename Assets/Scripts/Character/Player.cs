using System.Collections;
using UnityEngine;

public class Player : Character
{
    private float _mp = 0.0f;
    private bool _isUsingSkill = false;

    private void Awake()
    {
        base.Awake();
        
        _moveDir = Vector2.right;
    }

    private void Update()
    {
        base.Update();
        _target = FindTarget("Enemy", _findTargetRange);
    }

    protected override void IdleStateAction()
    {
        base.IdleStateAction();

        if (BattleStateManager.Instance.IsBattle)
        {
            _curState = State.Move;
        }
    }

    protected override void MoveStateAction()
    {
        base.MoveStateAction();

        if (_target == null)
        {
            transform.Translate(_moveDir * _moveSpeed * Time.deltaTime);
        }
        else
        {
            if (FindTarget("Enemy", _attackRange) == null)
            {
                Vector2 dir = _target.transform.position - transform.position;
                transform.Translate(dir.normalized * _moveSpeed * Time.deltaTime);
            }
            else
            {
                _curState = State.Attack;
            }
        }
    }

    protected override void AttackStateAction()
    {
        base.AttackStateAction();

        if (_target == null)
        {
            _curState = State.Move;
        }
    }

    public void OnAttackHit()
    {
        if (_isUsingSkill) return;

        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRange);

        foreach (Collider2D hitTarget in hitTargets)
        {
            if (hitTarget.CompareTag("Enemy"))
            {
                Character enemy = hitTarget.GetComponent<Character>();
                if (enemy != null)
                {
                    enemy.TakeDamage(_atk);
                }
                _mp += 20f;
                if (_mp >= 100f)
                {
                    StartCoroutine(CastSkill());
                }
            }
        }
    }

    private IEnumerator CastSkill()
    {
        if (_isUsingSkill) yield break;

        _isUsingSkill = true;
        _mp = 0;
        _animator.SetTrigger("Skill");

        // Skill 애니메이션 반영까지 1 프레임 대기
        yield return new WaitForEndOfFrame();

        // 실제로 Skill 상태가 시작될 때까지 대기
        while (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Skill"))
            yield return null;

        float skillDuration = _animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(skillDuration);

        _isUsingSkill = false;
    }

    public void SetPlayerStat(PlayerStatData data)
    {
        _hp = data.Hp * 10;
        _mp = 0.0f;
    }
}
