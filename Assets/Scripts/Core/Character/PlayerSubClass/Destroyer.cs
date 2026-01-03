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

    private const float _ultWeakPhase = 0.82f;
    private const float _ultMidPhase = 0.9f;
    private const float _ultStrongPhase = 1.0f;

    private const float _yOffsetLimitWeakPhase = 0.6f; 
    private const float _yOffsetLimitMidPhase = 0.8f;
    private const float _yOffsetLimitStrongPhase = 1.15f;

    private float _ultPhasePower = 0.0f;
    private float _yOffsetLimit = 0.0f;

    private int _hitCount = 0;

    private void Awake()
    {
        base.Awake();

        _hitCount = 0;
        _ultPhasePower = _ultWeakPhase;
        _yOffsetLimit = _yOffsetLimitWeakPhase;

        _targets[(int)ActionSlot.Attack] = GetComponent<NearestTargetSelector>();
        _targets[(int)ActionSlot.Skill] = GetComponent<NearestTargetSelector>();
        _targets[(int)ActionSlot.Ult] = GetComponent<NearestTargetSelector>();
    }

    // CalculateDamage는 데미지 담당
    protected override float CalculateDamage(ActionSlot slot)
    {
        switch (slot)
        {
            case ActionSlot.Attack:
                return _data.Atk * _atkBuff;
            case ActionSlot.Ult:
                return _data.Atk * _data.Ultimate * _ultPhasePower * _atkBuff;
        }

        return 0.0f;
    }

    // GetTargets는 스킬 타입에 따라 타겟 수집 방식 변경
    protected override Collider2D[] GetTargets(ActionSlot slot)
    {
        switch(slot) 
        {
            case ActionSlot.Attack:
                return base.GetTargets(slot);

            case ActionSlot.Ult:
                return GetUltTargets();

            default:
                return new Collider2D[0];
        }
    }

    // 최종 궁극기의 데미지를 받을 타겟을 고르는 함수
    private Collider2D[] GetUltTargets()
    {
        Vector2 position = GetUltHitBoxCenter();
        Vector2 size = _skillEffectController.GetSize();

        Collider2D[] overlapped = Physics2D.OverlapBoxAll(position, size, 0.0f);
        List<Collider2D> result = new List<Collider2D>();

        foreach (Collider2D target in overlapped)
        {
            if (!IsMonster(target)) //몬스터가 아니면 건너뜀
                continue;

            // Y값 오차 범위 내에 있는 적만 타겟으로 리스트에 넣음
            if (IsWithinYOffset(target.transform.position.y, _yOffsetLimit)) 
            {
                print("디트 궁 타겟 : " + target.gameObject.name);
                result.Add(target);
            }
        }

        return result.ToArray();
    }

    // AfterHit는 히트 카운트 증가와 위상(phase) 변경 처리
    protected override void PreHit(ActionSlot slot)
    {
        //base.PreHit(slot);

        if (slot != ActionSlot.Ult)
        {
            return;
        }

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

        // 기즈모 색상
        Gizmos.color = Color.black;

        // OverlapBox와 동일하게 중앙 기준으로 큐브 그리기
        Gizmos.DrawWireCube(position, size);
    }
}