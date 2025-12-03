using System.Collections;
using UnityEngine;
using System;

public class DamageReceiver : MonoBehaviour
{
    private Character _character;
    private Collider2D _targetCollider;
    private CharacterGraphics _graphics;

    private Coroutine _dotCoroutine = null;
    private Coroutine _slowCoroutine = null;
    
    private float _curHp = 0.0f;
    public float CurHp
    {
        get { return _curHp; }
    }
    private float _maxHp = 0.0f;
    public float MaxHp
    {
        get { return _maxHp; }
        set {  _maxHp = value; }
    }
    private float _damageReduction = 1.0f;
    public float DamageReduction
    {
        get { return _damageReduction; }
        set { _damageReduction = value; }
    }

    public void Initialize(Character character, float initialHp)
    {
        _character = character;
        _targetCollider = character.GetComponent<Collider2D>();
        _graphics = character.GetComponent<CharacterGraphics>();
        if (_graphics == null)
        {
            // 안전장치
            _graphics = character.gameObject.AddComponent<CharacterGraphics>();
        } 

        _curHp = initialHp;
        _maxHp = initialHp;
    }

    public void SetHp(float hp)
    { 
        _curHp = hp;
        // 필요하면 시각적 표시 초기화
        //_graphics?.ResetVisuals();
    }

    public void TakeDamage(float amount)
    {
        if (_character == null || !_targetCollider.enabled)
        { 
            return;
        }

        float finalDamage = amount * _damageReduction;

        _curHp -= finalDamage;

        _graphics.PlayFlashHit();
        _graphics.ShowDamageText(finalDamage);
        //print(name + " " + "체력: " + _curHp + "/" + _maxHp);

        if (_curHp <= 0.0f)
        {
            _curHp = 0.0f;
            // 사망 처리 요청
            _character.CharacterDeath();
        }
    }

    public void UpdateDamageReduction(float damageReduction)
    {
        _damageReduction = damageReduction;
    }

    public void TakeDotDamage(HitType type, float damagePerTick, float duration, float tickInterval)
    {
        /*if (_dotCoroutine != null)
        {
            StopCoroutine(_dotCoroutine);

            _dotCoroutine = null;
        }*/

        // 중첩 허용
        StartCoroutine(DotCoroutine(type, damagePerTick, duration, tickInterval));
    }

    private IEnumerator DotCoroutine(HitType type, float damage, float duration, float tick)
    {
        float elapsed = 0.0f;

        _graphics.PlayFlashHit(type, duration);

        while (elapsed < duration)
        {
            yield return new WaitForSeconds(tick);

            if (!_targetCollider.enabled)
            {
                yield break;
            }

            _curHp -= damage;
            _graphics?.ShowDotText(damage);

            if (_curHp <= 0.0f)
            {
                _curHp = 0.0f;
                _character.CharacterDeath();
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
        if (_character != null)
        {
            _character.SetAnimatorSpeed(speedFactor);
        }

        yield return new WaitForSeconds(duration);

        if (_character != null)
        {
            _character.SetAnimatorSpeed(1.0f);
        }

        _slowCoroutine = null;
    }
}