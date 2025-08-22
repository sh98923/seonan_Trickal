using UnityEngine;
using TMPro;

public class DotText : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    Vector3 _startPos;
    private float _duration = 3.0f;
    private float _timer = 0.0f;

    [SerializeField] private float _amplitude;   // 초기 y 진폭
    private float _frequency = 20.0f;   // 튀는 주기
    private readonly float _amplitudeValue = 5.0f;
    private float _speed = 50.0f;       // 오른쪽 이동 속도

    public void Initialize(float damage, Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        transform.position = screenPos;
        _startPos = screenPos;

        _timer = 0.0f;
        _amplitude = _amplitudeValue;

        _text = GetComponent<TMP_Text>();
        _text.text = damage.ToString("F0");

        gameObject.SetActive(true);
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        // 오른쪽으로 이동
        transform.Translate(Vector3.right * _speed * Time.deltaTime);

        // y값 튀는 계산: |sin|, 진폭 점점 감소
        float yOffset = Mathf.Abs(Mathf.Sin(Time.deltaTime * _frequency)) * _amplitude;
        Vector3 pos = transform.position;
        pos.y = _startPos.y + yOffset;
        transform.position = pos;

        // 진폭 점점 줄이기 (주기가 1번 지날 때마다 반으로)
        float decayFactor = Mathf.Pow(0.5f, Time.deltaTime * _frequency / (2 * Mathf.PI));
        _amplitude *= decayFactor;

        // duration 지나면 비활성화
        if (_timer >= _duration)
        {
            gameObject.SetActive(false);
        }
    }
}