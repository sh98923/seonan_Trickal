using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float _lifeTime = 2.0f;
    [SerializeField] private float _moveSpeed = 2.0f;
    [SerializeField] private float _alphaSpeed = 1.0f;

    private Color _alpha;

    private float _time;
    private float _dirX;

    private void Update()
    {
        if (Time.timeScale == 0.0f)
        {
            return;
        }

        transform.Translate(new Vector3(_dirX, _moveSpeed * Time.deltaTime, 0)); // �ؽ�Ʈ ��ġ

        _alpha.a = Mathf.Lerp(_alpha.a, 0, Time.deltaTime * _alphaSpeed); // �ؽ�Ʈ ���İ�
        _text.color = _alpha;

        _time += Time.deltaTime;

        if (_time > _lifeTime)
        {
            gameObject.SetActive(false);
        }
    }

    public void Initialize(float damage, Vector3 worldPosition)
    {
        _text.text = damage.ToString("F0");

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        transform.position = screenPos;

        _dirX = Random.Range(-0.2f, 0.2f);

        //resetFlyAnim();
        resetFadeoutAnim();

        _time = 0;
        gameObject.SetActive(true);
    }

    private void resetFadeoutAnim()
    {
        _alpha = Color.white;
        _text.color = _alpha;
        //this.gameObject.SetActive(true);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }
}
