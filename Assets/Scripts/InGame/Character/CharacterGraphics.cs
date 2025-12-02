using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGraphics : MonoBehaviour
{
    private const float _fadeDuration = 0.5f;
    private const float _flashHitDuration = 0.1f;
    
    private Character _character;
    private SpriteRenderer[] _spriteRenderers;
    private SpriteRenderer _shadowSprite;

    private Color _red = new Color(1f, 0.41f, 0.38f);
    private Color[] _originalColors;

    private int _hitCount = 0;

    public void Initialize(Character character, SpriteRenderer[] spriteRenderers, SpriteRenderer shadowSprite)
    {
        _character = character;
        _shadowSprite = shadowSprite;

        List<SpriteRenderer> spritesWithoutShadow = new List<SpriteRenderer>();

        foreach (SpriteRenderer sprite in spriteRenderers)
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
            _originalColors[i] = _spriteRenderers[i].color;
    }

    public void PlayFlashHit()
    {
        _hitCount++;
        StartCoroutine(FlashHitColor());
    }

    private IEnumerator FlashHitColor()
    {
        // 바로 빨강으로 변경
        foreach (SpriteRenderer sprite in _spriteRenderers)
        {
            sprite.color = _red;
        }

        yield return new WaitForSeconds(_flashHitDuration);

        // 마지막 피격이 끝나야 색 복구
        _hitCount--;
        if (_hitCount <= 0)
        {
            _hitCount = 0;
            for (int i = 0; i < _spriteRenderers.Length; i++)
            {
                _spriteRenderers[i].color = _originalColors[i];
            }
        }
    }

    public void ShowDamageText(float damage)
    {
        GameObject damageText = PoolingManager.Instance.Pop("DamageText");
        Vector3 worldPos = transform.position + Vector3.up * 1.8f;
        damageText.GetComponent<DamageText>().Initialize(damage, worldPos);
    }

    public void ShowDotText(float damage)
    {
        GameObject dotText = PoolingManager.Instance.Pop("DotText");
        Vector3 worldPos = transform.position + Vector3.up;
        dotText.GetComponent<DotText>().Initialize(damage, worldPos);
    }

    public void StartFadeOutAndDisable()
    {
        // Character 소유자에서 코루틴 실행하게 함
        if (_character != null)
        { 
            _character.StartCoroutine(FadeOutAndDisable());
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
