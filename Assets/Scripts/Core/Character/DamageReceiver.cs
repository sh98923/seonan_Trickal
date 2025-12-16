using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DamageReceiver : MonoBehaviour
{
    private Character _character;
    private Collider2D _targetCollider;
    private CharacterGraphics _graphics;
    private DamageTextManager _hitText;

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

    private void Awake()
    {
        GameManager.Instance.EnableInScenes(this, SceneName.InGameScene);
    }

    private void Start()
    {
        _hitText = GameObject.Find("DamageTextPanel").GetComponent<DamageTextManager>();
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

        string textKey = _hitText.GetHitText();

        _graphics.PlayFlashHit();
        _graphics.ShowDamageText(textKey, finalDamage);
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
        // 중첩 허용
        StartCoroutine(DotCoroutine(type, damagePerTick, duration, tickInterval));
    }

    private IEnumerator DotCoroutine(HitType type, float damage, float duration, float tick)
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

            _curHp -= damage;
            _graphics.PlayFlashHit(type);
            _graphics?.ShowDotText(textKey, damage);

            if (_curHp <= 0.0f)
            {
                _curHp = 0.0f;
                _character.CharacterDeath();
                yield break;
            }

            elapsed += tick;
        }
    }
}