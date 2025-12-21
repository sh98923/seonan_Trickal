using UnityEngine;
using UnityEngine.UI;

public class OutLineLight : MonoBehaviour
{
    private Image _outlineImage;

    private const float _maxHue = 1.0f;

    private float _hue = 0.0f;
    private float _speed = 0.35f; // 무지개 속도
    private float _saturation = 0.5f; // 채도 낮춰 파스텔
    private float _value = 0.95f;     // 밝기 높여 부드럽게

    private void Update()
    {
        if( _outlineImage == null )
        {
            return;
        }

        _hue += Time.deltaTime * _speed;

        if (_hue > _maxHue)
        {
            _hue -= _maxHue;
        }

        _outlineImage.color = Color.HSVToRGB(_hue, _saturation, _value);
    }

    public void InitImage(Transform transform)
    {
        if(transform != null)
        {
            _outlineImage = transform.GetComponent<Image>();
        }
    }

    public void OutLineActive(bool isActive)
    {
        _outlineImage.gameObject.SetActive(isActive);
    }
}
