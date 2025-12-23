using System.Collections;
using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    private Character _character;
    private Collider2D _targetCollider;
    private CharacterGraphics _graphics;
    private DamageTextManager _hitText;

    private float _damageReduction = 1.0f;
    public float DamageReduction
    {
        get { return _damageReduction; }
        set { _damageReduction = value; }
    }

    private void Awake()
    {
        GameManager.Instance.EnableScriptInScenes(this, SceneName.InGameScene);
    }

    private void Start()
    {
        _hitText = GameObject.Find("DamageTextPanel").GetComponent<DamageTextManager>();
    }

    public void Initialize(Character character)
    {
        _character = character;
        _targetCollider = character.GetComponent<Collider2D>();
        _graphics = character.GetComponent<CharacterGraphics>();
        if (_graphics == null)
        {
            // 안전장치
            _graphics = character.gameObject.AddComponent<CharacterGraphics>();
        } 
    }

    public void TakeDamage(IDamageableHealth health, float amount)
    {
        if (_character == null || !_targetCollider.enabled)
        { 
            return;
        }

        float finalDamage = amount * _damageReduction;
        health.DecreaseHp(finalDamage);

        string textKey = _hitText.GetHitText();

        _graphics.PlayFlashHit();
        _graphics.ShowDamageText(textKey, finalDamage);
        //print(name + " " + "체력: " + _curHp + "/" + _maxHp);

        if (health.CurHp <= 0.0f)
        {
            // 사망 처리 요청
            _character.CharacterDeath();
        }
    }

    public void UpdateDamageReduction(float damageReduction)
    {
        _damageReduction = damageReduction;
    }

    public void TakeDotDamage(IDamageableHealth health, HitType type, float damagePerTick, float duration, float tickInterval)
    {
        // 중첩 허용
        StartCoroutine(DotCoroutine(health, type, damagePerTick, duration, tickInterval));
    }

    private IEnumerator DotCoroutine(IDamageableHealth health, HitType type, float damage, float duration, float tick)
    {
        float elapsed = 0.0f;
        string textKey = _hitText.GetHitText(type);

        while (elapsed < duration)
        {
            yield return new WaitForSeconds(tick);

            if (!_targetCollider.enabled)
            {
                yield break;
            }

            health.DecreaseHp(damage);
            _graphics.PlayFlashHit(type);
            _graphics?.ShowDotText(textKey, damage);

            if (health.CurHp <= 0.0f)
            {
                _character.CharacterDeath();
                yield break;
            }

            elapsed += tick;
        }
    }
}