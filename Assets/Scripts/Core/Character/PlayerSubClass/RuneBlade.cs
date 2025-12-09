using System.Collections.Generic;
using UnityEngine;

public class RuneBlade : Player
{
    private HashSet<Collider2D> _ultHitTargets = new HashSet<Collider2D>();

    private float _yOffsetLimit = 0.8f;     // Y 오차 범위
    private bool _canDamaged = false;

    private void Awake()
    {
        base.Awake();

        _targets[(int)ActionSlot.Attack] = GetComponent<FarthestTargetSelector>();
        _targets[(int)ActionSlot.Skill] = GetComponent<FarthestTargetSelector>();
        _targets[(int)ActionSlot.Ult] = GetComponent<FarthestTargetSelector>();
    }

    protected void Update()
    {
        base.Update();

        // 궁극기 상태 + 스킬이펙트 켜져 있을 때만 판정
        if (_actionType == ActionSlot.Ult && _canDamaged)
        {
            UltHitCheck();
        }
    }

    private void UltHitCheck()
    {
        // ① 스킬 이펙트 실제 중앙 위치 구함
        Vector2 worldPos = transform.position;
        Vector2 localPos = _skillEffectController.GetPosition();
        localPos.x *= transform.localScale.x;
        Vector2 center = worldPos + localPos;

        // ② 사이즈 구함
        Vector2 size = _skillEffectController.GetSize();

        // ③ 몬스터 Overlap
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0);

        float finalDamage = CalculateDamage(ActionSlot.Ult);

        foreach (Collider2D target in hits)
        {
            if (!IsMonster(target))
            {
                continue;
            }

            float targetY = target.transform.position.y;

            // y 오차 확인
            if (!IsWithinYOffset(targetY, _yOffsetLimit))
            {
                continue;
            }

            // 최초 충돌일 때만 true (재충돌이면 false)
            bool firstHit = _ultHitTargets.Add(target);

            // 이미 맞았던 몬스터는 데미지 안줌
            if (!firstHit)
            {
                continue;
            }

            // 타겟에게 공격
            _action[(int)ActionCategory.Attack].SetAttackInfo(target, _actionType, _clipName, _duration, finalDamage);
            _action[(int)ActionCategory.Attack].Excute();
        }
    }

    public void OnCanHit()
    {
        _canDamaged = true;
    }

    public void OnCantHit()
    {
        // 초기화
        _clipName = _data.ClipName[(int)_actionType];
        _duration = _data.Duration[(int)_actionType];

        _ultHitTargets.Clear();

        _canDamaged = false;
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        if (_skillEffectController == null) return;

        Vector2 worldPos = transform.position;
        Vector2 localPos = _skillEffectController.GetPosition();
        localPos.x *= transform.localScale.x;

        Vector2 position = worldPos + localPos;
        Vector2 size = _skillEffectController.GetSize();

        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(position, size);
    }
}