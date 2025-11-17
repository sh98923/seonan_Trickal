using System.Collections;
using UnityEngine;

public class CharacterGraphics : MonoBehaviour
{
    private Character _character;
    private SpriteRenderer[] _spriteRenderers;
    private SpriteRenderer _shadowSprite;
    private float _fadeDuration = 3.0f;

    public void Initialize(Character character, SpriteRenderer[] spriteRenderers, SpriteRenderer shadowSprite, float fadeDuration = 3.0f)
    {
        _character = character;
        _spriteRenderers = spriteRenderers;
        _shadowSprite = shadowSprite;
        _fadeDuration = fadeDuration;
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
            _character.StartCoroutine(FadeOutAndDisable());
    }

    private IEnumerator FadeOutAndDisable()
    {
        yield return new WaitForSeconds(1.5f);

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
