using System.Collections;
using UnityEngine;
using System;

public class DamageReceiver : MonoBehaviour
{
    private Character _owner;
    private CharacterGraphics _graphics;

    private float _curHp = 0.0f;
    private float _maxHp = 0.0f;

    private Coroutine _dotCoroutine = null;
    private Coroutine _slowCoroutine = null;

    public void Initialize(Character owner, float initialHp)
    {
        _owner = owner;
        _graphics = owner.GetComponent<CharacterGraphics>();
        if (_graphics == null)
            _graphics = owner.gameObject.AddComponent<CharacterGraphics>(); // 안전장치

        _curHp = initialHp;
        _maxHp = initialHp;
    }

    public float CurrentHp => _curHp;
    public float MaxHp => _maxHp;

    public void SetHp(float hp)
    {
        _maxHp = hp;
        _curHp = hp;
    }

    public void TakeDamage(float amount)
    {
        if (_owner == null) return;
        
        _curHp -= amount;
        _graphics?.ShowDamageText(amount);
        //print(name + " " + "체력: " + _curHp + "/" + _maxHp);

        if (_curHp <= 0.0f)
        {
            _curHp = 0.0f;
            // 사망 처리 요청
            _owner.CharacterDeath();
        }
    }

    public void TakeDotDamage(float damagePerTick, float duration, float tickInterval)
    {
        if (_dotCoroutine != null) StopCoroutine(_dotCoroutine);
        _dotCoroutine = StartCoroutine(DotCoroutine(damagePerTick, duration, tickInterval));
    }

    private IEnumerator DotCoroutine(float dmg, float duration, float tick)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            yield return new WaitForSeconds(tick);
            if (_owner == null) yield break;

            _curHp -= dmg;
            _graphics?.ShowDotText(dmg);

            if (_curHp <= 0.0f)
            {
                _curHp = 0.0f;
                _owner.CharacterDeath();
                yield break;
            }

            elapsed += tick;
        }
        _dotCoroutine = null;
    }

    /// <summary>
    /// 느려짐(애니메이터 속도 변경)을 적용하고 duration 후에 복구.
    /// speedFactor: 예) 0.7f = 70% 속도 ( 느려지는 경우 ) -> animator.speed = speedFactor
    /// </summary>
    public void ApplyAttackSlow(float duration, float speedFactor)
    {
        if (_slowCoroutine != null) StopCoroutine(_slowCoroutine);
        _slowCoroutine = StartCoroutine(SlowCoroutine(duration, speedFactor));
    }

    private IEnumerator SlowCoroutine(float duration, float speedFactor)
    {
        if (_owner != null)
        {
            _owner.SetAnimatorSpeed(speedFactor);
        }

        yield return new WaitForSeconds(duration);

        if (_owner != null)
        {
            _owner.SetAnimatorSpeed(1.0f);
        }

        _slowCoroutine = null;
    }
}