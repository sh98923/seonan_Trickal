using UnityEngine;

public class Monster : Character
{
    private void Awake()
    {
        base.Awake();
        _moveDir = Vector2.left;
    }

    // Idle 상태면 Battle모드일 때 move로 넘어옴
    protected override void MoveStateAction()
    {
        base.MoveStateAction();

        if(_targetCollider == null)
        {
            _targetCollider = FindTarget("Player", _findTargetRange);
            transform.Translate(_moveDir * _moveSpeed * Time.deltaTime);
        }
        else
        {
            if (FindTarget("Player", _attackRange) == null)
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

    public void WaveUpgrade(int wave)
    {
        float hpLinear = _characterData.Hp + _characterData.HpPerWave * wave;
        float hpExp = _characterData.Hp * Mathf.Pow(_characterData.HpGrowthRate, wave);

        // 두 가지의 공식을 평균 낸 값 (선형, 지수)
        _maxHp = (hpLinear + hpExp) * 0.5f;
        // Round가 정수로만 반올림 하기 떄문에
        // 소수점 첫째자리까지 나오게 하기위한 수식
        _maxHp = Mathf.Round(_maxHp * 10f) * 0.1f;
        _curHp = _maxHp;

        float atkLinear = _characterData.Atk + _characterData.AtkPerWave * wave;
        float atkExp = _characterData.Atk * Mathf.Pow(_characterData.AtkGrowthRate, wave);
        _atk = (atkLinear + atkExp) * 0.5f;
        _atk = Mathf.Round(_atk * 10f) * 0.1f;

        print(_characterData.EngName + " : " + _curHp.ToString("F1") + ", " + _atk.ToString("F1"));
    }
}