using UnityEngine;

public class Destroyer : Player
{
    // GPT가 만든거 다시 보고 생각하쇼
    /*private int _ultiHitCount = 0;

    // 각 타수별 데미지 설정
    [SerializeField] private float _ultiWeakMultiplier = 1.2f; // 1,2타
    [SerializeField] private float _ultiStrongMultiplier = 1.5f; // 3,4,5타

    // 궁극기 타수별 최대 히트
    private const int MaxUltHits = 5;

    // 애니메이션 이벤트로 호출될 수 있도록 public
    public override void OnAttack()
    {
        if (_actionType == ActionSlot.Ult)
        {
            _ultiHitCount++;

            float finalDamage = 0f;

            // 타수에 따라 데미지 구분
            if (_ultiHitCount == 1 || _ultiHitCount == 2)
            {
                finalDamage = Mathf.Max(_data.Atk * _ultiWeakMultiplier, _data.Atk + 0.01f);
            }
            else
            {
                finalDamage = Mathf.Max(_data.Atk * _ultiStrongMultiplier, _data.Atk + 0.01f);
            }

            if (_attackTarget != null && _attackTarget.enabled)
            {
                _action[(int)ActionCategory.Attack].SetAttackInfo(
                    _attackTarget,
                    _actionType,
                    _data.ClipName[(int)_actionType],
                    _data.Duration[(int)_actionType],
                    finalDamage
                );

                base.OnAttack();
            }

            // 마지막 타수 후 초기화
            if (_ultiHitCount >= MaxUltHits)
            {
                _ultiHitCount = 0;
            }
        }
        else
        {
            // 일반 공격 및 스킬은 기본 Player 로직 사용
            base.OnAttack();
        }
    }

    // 궁극기 발동 시 초기화
    public void StartUltimate()
    {
        _ultiHitCount = 0;
        _actionType = ActionSlot.Ult;
        _animator.SetTrigger("Ult");
    }*/
}