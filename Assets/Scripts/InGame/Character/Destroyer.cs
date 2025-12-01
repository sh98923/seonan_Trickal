using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Toolbars;
using UnityEngine;

public class Destroyer : Player
{
    private enum HitCount
    {
        First = 1,   // 1타
        Second,  // 2타
        Third,   // 3타
        Fourth,  // 4타
        Fifth    // 5타
    }

    private Collider2D[] _attackTargets;

    private const float _ultWeakPhase = 0.82f;
    private const float _ultMidPhase = 0.9f;
    private const float _ultStrongPhase = 1.0f;

    private const float _yOffsetLimitWeakPhase = 0.6f; 
    private const float _yOffsetLimitMidPhase = 0.8f;
    private const float _yOffsetLimitStrongPhase = 1.3f;

    private float _ultPhasePower = 0.0f;
    private float _yOffsetLimit = 0.0f;

    private int _hitCount = 0;

    private void Awake()
    {
        base.Awake();

        _hitCount = 0;
        _ultPhasePower = _ultWeakPhase;
        _yOffsetLimit = _yOffsetLimitWeakPhase;
    }

    /*public override void OnAttack()
    {
        // 기본 공격일 때 타겟을 넣지 않아서 null 뜸 이따 해결 ㄱ

        if (BattleStateManager.Instance.CurrentState == BattleState.EnteringReroll)
        {
            return;
        }

        switch (_actionType)
        {
            case ActionSlot.Base:
                _finalDamage = _data.Atk * _atkBuff;
                break;
            case ActionSlot.Ult:
                _playerMp.UseMp();
                UltSpikeHit();
                _finalDamage = _data.Atk * _data.Ultimate * _ultPhasePower * _atkBuff;
                break;
        }

        if (tag == "Player")
        {
            _clipName = _data.ClipName[(int)_actionType];
            _duration = _data.Duration[(int)_actionType];
        }

        _action[(int)ActionCategory.Attack].SetAttackInfo(_attackTargets, _actionType, _clipName, _duration, _finalDamage);
        _action[(int)ActionCategory.Attack].Excute();
    }*/

    // CalculateDamage는 데미지 담당
    protected override float CalculateDamage(ActionSlot slot)
    {
        switch (slot)
        {
            case ActionSlot.Base:
                return _data.Atk * _atkBuff;
            case ActionSlot.Ult:
                _playerMp.UseMp();
                // 위상 파워가 AfterHit에서 관리되므로 여기선 적용
                return _data.Atk * _data.Ultimate * _ultPhasePower * _atkBuff;
        }

        return 0.0f;
    }

    // GetTargets는 스킬 타입에 따라 타겟 수집 방식 변경
    protected override Collider2D[] GetTargets(ActionSlot slot)
    {
        if (slot == ActionSlot.Base)
        {
            // 기본 공격은 단일 타겟 (Player가 이미 _attackTarget에 넣어둠)
            if (_attackTarget != null)
            {
                return new Collider2D[] { _attackTarget };
            }

            return new Collider2D[0];
        }
        else if (slot == ActionSlot.Ult)
        {
            // 기존 UltSpikeHit의 OverlapBox 로직을 재사용해서 범위 내 몬스터만 반환
            Vector2 worldPos = transform.position;
            Vector2 localPos = _skillEffectController.GetPosition();
            localPos.x *= transform.localScale.x;

            Vector2 position = worldPos + localPos;
            Vector2 size = _skillEffectController.GetSize();

            // OverlapBoxAll은 Collider2D[]를 반환
            Collider2D[] targets = Physics2D.OverlapBoxAll(position, size, 0.0f);

            // y-offset 필터 적용 (기존 UltHitTargets의 동작)
            List<Collider2D> finalTargets = new List<Collider2D>();

            foreach (Collider2D target in targets)
            {
                if(target.tag != "Monster")
                {
                    continue;
                }

                if (Mathf.Abs(target.transform.position.y - transform.position.y) <= _yOffsetLimit)
                {
                    print("디트 궁 타겟 : " + target.gameObject.name);

                    finalTargets.Add(target);
                }
            }

            return finalTargets.ToArray();
        }
        else // Skill (범위 스킬 등)
        {
            // 기본 구현: 단일 타겟 또는 targeting logic 추가 가능
            if (_attackTarget != null)
                return new Collider2D[] { _attackTarget };
            return new Collider2D[0];
        }
    }

    // AfterHit는 히트 카운트 증가와 위상(phase) 변경 처리
    protected override void PreHit(ActionSlot slot)
    {
        if (slot != ActionSlot.Ult) return;

        _hitCount++;

        switch ((HitCount)_hitCount)
        {
            case HitCount.First:
            case HitCount.Second:
            case HitCount.Third:
                _ultPhasePower = _ultWeakPhase;
                _yOffsetLimit = _yOffsetLimitWeakPhase;
                break;
            case HitCount.Fourth:
                _ultPhasePower = _ultMidPhase;
                _yOffsetLimit = _yOffsetLimitMidPhase;
                break;
            case HitCount.Fifth:
                _ultPhasePower = _ultStrongPhase;
                _yOffsetLimit = _yOffsetLimitStrongPhase;
                _hitCount = 0;
                break;
        }
    }

    private void UltSpikeHit()
    {
        // Player 월드 위치
        Vector2 worldPos = transform.position;
        // 스킬 이펙트 로컬 위치
        Vector2 localPos = _skillEffectController.GetPosition();
        localPos.x *= transform.localScale.x;
        // 실제 스킬 이펙트 위치
        Vector2 position = worldPos + localPos;
        Vector2 size = _skillEffectController.GetSize();

        _attackTargets = Physics2D.OverlapBoxAll(position, size, 0.0f, LayerMask.GetMask("Monster"));

        _hitCount++;

        switch ((HitCount)_hitCount)
        {
            case HitCount.First:
            case HitCount.Second:
            case HitCount.Third:
                _ultPhasePower = _ultWeakPhase;
                break;
            case HitCount.Fourth:
                _ultPhasePower = _ultMidPhase;
                break;
            case HitCount.Fifth:
                _ultPhasePower = _ultStrongPhase;
                _hitCount = 0;
                break;
        }

        UltHitTargets(_attackTargets);
    }

    private void UltHitTargets(Collider2D[] targets)
    {
        List<Collider2D> targetList = new List<Collider2D>();

        foreach (Collider2D target in targets)
        {
            if (Mathf.Abs(target.transform.position.y - transform.position.y) <= _yOffsetLimitWeakPhase)
            {
                targetList.Add(target);
            }
        }

        _attackTargets = targetList.ToArray();
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        if (_skillEffectController == null) return;

        // Player 월드 위치
        Vector2 worldPos = transform.position;
        // 스킬 이펙트 로컬 위치
        Vector2 localPos = _skillEffectController.GetPosition(); 
        localPos.x *= transform.localScale.x;

        Vector2 position = worldPos + localPos;
        // 스킬 이펙트 로컬 크기
        Vector2 size = _skillEffectController.GetSize();

        /*// 부모 스케일 반영
        Vector3 lossyScale = _skillEffectController.transform.lossyScale;
        Vector2 worldSize = new Vector2(localSize.x * lossyScale.x, localSize.y * lossyScale.y);*/

        //print("기즈모 : " + position + " " + size);

        // 기즈모 색상
        Gizmos.color = Color.black;

        // OverlapBox와 동일하게 중앙 기준으로 큐브 그리기
        Gizmos.DrawWireCube(position, size);
    }
}