using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public enum HitType
{
    Normal,     // 일반 피격
    Fire,       // 화염
    Posion,     // 독
    Cold,       // 추워짐 (느려짐)
    Stone       // 돌 처럼 딱딱하게 굳음 (멈춤)
}

public class CharacterGraphics : MonoBehaviour
{
    private struct StatusEffectData
    {
        public float Duration; // 상태 이상 지속 시간
        public float AnimSpeed; // 애니메이션 속도

        public StatusEffectData(float duration, float animSpeed)
        {
            Duration = duration;
            AnimSpeed = animSpeed;
        }
    }

    private const float _fadeDuration = 0.5f;

    // 상태별 종료 시간 관리
    private Dictionary<HitType, StatusEffectData> _statusEndTimes = new Dictionary<HitType, StatusEffectData>();

    private Character _character;
    private SpriteRenderer[] _spriteRenderers;
    private SpriteRenderer _shadowSprite; 
    private Coroutine _statusRoutine;

    private Color _normalHitColor = new Color(1.0f, 0.41f, 0.38f);      // 빨간색 계열
    private Color _fireHitColor = new Color(1.0f, 0.56f, 0.32f);        // 주황색 계열
    private Color _poisonHitColor = new Color(0.72f, 0.45f, 1.0f);      // 보라색 계열
    private Color _coldHitColor = new Color(0.5f, 0.8f, 1.0f);          // 파란색 계열
    private Color _stoneHitColor = new Color(0.6f, 0.6f, 0.6f);         // 회색 계열

    private Color _currentColor = new Color();
    private Color[] _originalColors;

    // 색상
    private Color _shadowOriginalColor = Color.white;

    private int _hitCount = 0;
    private bool _isStatusActive = false;

    private void Awake()
    {
        _character = GetComponent<Character>();
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        List<SpriteRenderer> spritesWithoutShadow = new List<SpriteRenderer>();

        foreach (SpriteRenderer sprite in _spriteRenderers)
        {
            if (sprite != _shadowSprite && sprite.name != "SkillEffect")
            {
                spritesWithoutShadow.Add(sprite);
            }
        }

        _spriteRenderers = spritesWithoutShadow.ToArray();

        // 스프라이트 원본 색상 저장
        _originalColors = new Color[_spriteRenderers.Length];

        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            _originalColors[i] = _spriteRenderers[i].color;
        }
    }

    private void OnEnable()
    {
        /*foreach (SpriteRenderer sprite in _spriteRenderers)
        {
            Color color = sprite.color;

            if (sprite.name == "Shadow")
            {
                sprite.color = _shadowOriginalColor;
                continue;
            }

            color.a = 1.0f;
            sprite.color = color;
        }*/
    }

    public void PlayFlashHit()
    {
        PlayFlashHit(HitType.Normal);
    }

    public void PlayFlashHit(HitType hitType, float duration = 0.15f)
    {
        _hitCount++;
        StartCoroutine(FlashHitColor(hitType, duration));
    }

    private IEnumerator FlashHitColor(HitType hitType, float duration)
    {
        Color flashColor = _normalHitColor;

        switch (hitType)
        {
            case HitType.Fire:
                flashColor = _fireHitColor;
                break;
            case HitType.Posion:
                flashColor = _poisonHitColor;
                break;
        }

        // 피격 색상으로 변경
        SetSpriteColor(flashColor);

        yield return new WaitForSeconds(duration);

        // 마지막 피격이 끝나야 색 복구
        _hitCount--;
        if (_hitCount <= 0)
        {
            _hitCount = 0;

            // Cold 유지 중이면 flash 후 cold 색 복귀
            if (_isStatusActive)
            {
                Color curColor = GetCurrentColor();
                SetSpriteColor(curColor);
            }
            else
            {
                // Cold 아니면 원본 복귀
                RevertSpriteColor();
            }
        }
    }

    private void UpdateStatusColor()
    {
        // flash 색이 보여지는 동안은 상태색을 건들지 않음
        if (_hitCount > 0)
            return;

        // 상태 이상 우선순위: Stone > Cold > None
        if (_statusEndTimes.ContainsKey(HitType.Stone) && Time.time < _statusEndTimes[HitType.Stone].Duration)
        {
            _currentColor = _stoneHitColor;
            _character.SetAnimatorSpeed(_statusEndTimes[HitType.Stone].AnimSpeed);
        }
        else if (_statusEndTimes.ContainsKey(HitType.Cold) && Time.time < _statusEndTimes[HitType.Cold].Duration)
        {
            _currentColor = _coldHitColor;
            _character.SetAnimatorSpeed(_statusEndTimes[HitType.Cold].AnimSpeed);
        }
        else
        {
            _isStatusActive = false;
            RevertSpriteColor();
            return;
        }

        SetSpriteColor(_currentColor);
    }

    public void PlayStatusColor(HitType hitType, float duration, float animSpeed)
    {
        if (!_statusEndTimes.ContainsKey(hitType))
        {
            _statusEndTimes[hitType] = new StatusEffectData(Time.time + duration, animSpeed);
        }
        else
        {
            StatusEffectData data = _statusEndTimes[hitType];
            data.Duration = Time.time + duration;
            _statusEndTimes[hitType] = data;
        }

        if (_statusRoutine != null)
        {
            StopCoroutine(_statusRoutine);
            _statusRoutine = null;
        }

        _statusRoutine = StartCoroutine(StatusColor());
    }

    private IEnumerator StatusColor()
    {
        _isStatusActive = true;

        while (_isStatusActive)
        {
            if (_character.CurState == CharacterState.Dead)
            {
                _isStatusActive = false;
                _character.SetAnimatorSpeed(1.0f);
                RevertSpriteColor();
                yield break;
            }

            UpdateStatusColor();

            bool anyActive = false;
            foreach (StatusEffectData data in _statusEndTimes.Values)
            {
                if (Time.time < data.Duration)
                {
                    anyActive = true;
                    break;
                }
            }

            if (!anyActive)
                _isStatusActive = false;

            yield return null;
        }

        _character.SetAnimatorSpeed(1.0f);

        if (_hitCount <= 0)
        {
            RevertSpriteColor();
        }
    }

    private Color GetCurrentColor()
    {
        return _currentColor;
    }

    private void SetSpriteColor(Color hitColor)
    {
        foreach (SpriteRenderer sprite in _spriteRenderers)
        {
            sprite.color = hitColor;
        }
    }

    private void RevertSpriteColor()
    {
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            _spriteRenderers[i].color = _originalColors[i];
        }
    }

    public void ShowDamageText(string textKey, float damage)
    {
        GameObject damageText = PoolingManager.Instance.Pop(textKey);
        Vector3 worldPos = transform.position + Vector3.up * 1.8f;
        damageText.GetComponent<DamageText>().Initialize(damage, worldPos);
    }

    public void ShowDotText(string textKey, float damage)
    {
        GameObject dotText = PoolingManager.Instance.Pop(textKey);
        Vector3 worldPos = transform.position + Vector3.up;
        dotText.GetComponent<DotText>().Initialize(damage, worldPos);
    }

    public void StartFadeOutAndDisable()
    {
        if (_character != null)
        { 
            StartCoroutine(FadeOutAndDisable());
        }
    }

    private IEnumerator FadeOutAndDisable()
    {
        yield return new WaitForSeconds(2.0f);

        float timer = 0.0f;
        Color shadowOriginalColor = _shadowSprite != null ? _shadowSprite.color : Color.white;

        while (timer < _fadeDuration)
        {
            timer += Time.deltaTime;
            foreach (SpriteRenderer sprite in _spriteRenderers)
            {
                UpdateSpriteAlpha(sprite, shadowOriginalColor, timer);
            }
            yield return null;
        }

        gameObject.SetActive(false);
    }

    private void UpdateSpriteAlpha(SpriteRenderer sprite, Color shadowColor, float timer)
    {
        Color color = sprite.color;
        float progress = Mathf.Clamp01(timer / _fadeDuration);

        if (sprite == _shadowSprite)
        {
            color.a = Mathf.Lerp(shadowColor.a, 0.0f, progress);
        }
        else
        {
            color.a = Mathf.Lerp(1.0f, 0.0f, progress);
        }

        sprite.color = color;
    }
}