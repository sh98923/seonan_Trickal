using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitType
{
    Normal,     // 일반 피격
    Fire,       // 화염
    Posion,     // 독
    Cold,       // 추워짐 (느려짐)
    Stone       // 돌 처럼 딱딱하게 굳음 (멈춤)
}

public class CharacterRenderEffect : MonoBehaviour
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
    private Coroutine _statusRoutine;
    private SpriteRenderer _shadowSprite;
    private SpriteRenderer[] _spriteRenderers;

    private readonly Color _normalHitColor = new Color(1.0f, 0.41f, 0.38f);      // 빨간색 계열
    private readonly Color _fireHitColor = new Color(1.0f, 0.56f, 0.32f);        // 주황색 계열
    private readonly Color _poisonHitColor = new Color(0.72f, 0.45f, 1.0f);      // 보라색 계열
    private readonly Color _coldHitColor = new Color(0.5f, 0.8f, 1.0f);          // 파란색 계열
    private readonly Color _stoneHitColor = new Color(0.6f, 0.6f, 0.6f);         // 회색 계열

    private Color _currentColor = new Color();
    private Color _shadowOriginalColor = Color.white;
    private Color[] _originalColors;

    private int _hitCount = 0;
    private bool _isStatusActive = false;

    private void Awake()
    {
        CacheComponents();
        FilterVisibleSprites();
        CacheOriginalColors();
    }

    private void OnEnable()
    {
        RevertSpriteColor();
    }

    private void CacheComponents()
    {
        _character = GetComponent<Character>();
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer sprite in _spriteRenderers)
        {
            if (sprite.name == "Shadow")
            {
                _shadowSprite = sprite;
                break;
            }
        }
    }

    private void FilterVisibleSprites()
    {
        List<SpriteRenderer> result = new List<SpriteRenderer>();

        foreach (SpriteRenderer sprite in _spriteRenderers)
        {
            if (sprite.tag == "IgnoreFlash")
                continue;

            result.Add(sprite);
        }

        _spriteRenderers = result.ToArray();
    }

    private void CacheOriginalColors()
    {
        _originalColors = new Color[_spriteRenderers.Length];

        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            _originalColors[i] = _spriteRenderers[i].color;
        }
    }

    public void PlayFlashHit(HitType hitType = HitType.Normal, float duration = 0.15f)
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
        _hitCount = Mathf.Max(0, _hitCount - 1);

        if (_hitCount <= 0)
        {
            // Cold 유지 중이면 flash 후 cold 색 복귀
            if (_isStatusActive)
            {
                Color curColor = GetCurrentColor();
                SetSpriteColor(_currentColor);
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
            if (sprite == _shadowSprite)
                continue;

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

    /* ───────────────────────────
     *        타겟에게 플립
     * ─────────────────────────── */
    public void FlipTo(Transform self, Transform target)
    {
        if (target == null) return;

        Vector3 scale = self.localScale;
        scale.x = (target.position.x < self.position.x)
            ? Mathf.Abs(scale.x)
            : -Mathf.Abs(scale.x);

        self.localScale = scale;
    }

    /* ───────────────────────────
     *         페이드 아웃
     * ─────────────────────────── */
    public void StartFadeOutAndDisable()
    {
        StartCoroutine(FadeOutAndDisable());
    }

    private IEnumerator FadeOutAndDisable()
    {
        yield return new WaitForSeconds(2.0f);

        float timer = 0.0f;
        //Color shadowOriginal = _shadowSprite ? _shadowSprite.color : Color.white;

        while (timer < _fadeDuration)
        {
            timer += Time.deltaTime;
            float time = Mathf.Clamp01(timer / _fadeDuration);

            foreach (SpriteRenderer sprite in _spriteRenderers)
            {
                Color newColor = sprite.color;
                newColor.a = Mathf.Lerp(1.0f, 0.0f, time);
                sprite.color = newColor;
            }

            yield return null;
        }

        gameObject.SetActive(false);
    }
}