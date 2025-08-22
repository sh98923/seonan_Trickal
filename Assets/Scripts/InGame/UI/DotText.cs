using UnityEngine;
using TMPro;
using System.Collections;

public class DotText : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    private Vector3 _startPos;
    private float _duration = 0.7f;
    private float _timer = 0.0f;

    private float _amplitude;
    private readonly float _amplitudeValue = 90.0f;
    private float _speed = 100.0f;

    private int _bounceCount = 3; // 튕길 횟수
    private float _frequency;     // duration에 맞춰 계산된 주파수

    public void Initialize(float damage, Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        transform.position = screenPos;
        _startPos = screenPos;

        _timer = 0.0f;
        _amplitude = _amplitudeValue;

        // duration 안에 bounceCount번 튀게 하기
        // |sin|은 반 주기마다 1번 튀므로 => bounceCount = duration * frequency / PI
        _frequency = (_bounceCount * Mathf.PI) / _duration;

        _text = GetComponent<TMP_Text>();
        _text.text = damage.ToString("F0");

        Color text = _text.color;
        text.a = 1.0f;
        _text.color = text;

        gameObject.SetActive(true);
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        // 기본적으로 오른쪽 이동
        transform.Translate(Vector3.right * _speed * Time.deltaTime);

        // y축 튀기기 (duration 안에 bounceCount번 발생)
        float yOffset = Mathf.Abs(Mathf.Sin(_timer * _frequency)) * _amplitude;
        Vector3 pos = transform.position;
        pos.y = _startPos.y + yOffset;
        transform.position = pos;

        // 진폭 줄이기
        float decayFactor = Mathf.Pow(0.5f, Time.deltaTime * _frequency / Mathf.PI);
        _amplitude *= decayFactor;

        // duration 지나면 삭제
        if (_timer >= _duration)
        {
            StartCoroutine(FadeOutAndDisable());
        }
    }

    private IEnumerator FadeOutAndDisable()
    {
        float fadeTime = 0.3f; // 페이드 아웃 지속 시간
        float elapsed = 0f;
        Color c = _text.color;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            _text.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
