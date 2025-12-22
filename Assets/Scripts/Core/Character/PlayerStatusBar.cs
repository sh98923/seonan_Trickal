using UnityEngine;

public class PlayerStatusBar : MonoBehaviour
{
    private enum Status
    {
        Hp = 1,
        Mp
    }

    private PlayerHp _playerHp;
    private PlayerMp _playerMp;
    
    private Transform _hpBar;
    private Transform _mpBar;

    private void Awake()
    {
        _playerHp = GetComponent<PlayerHp>();
        _playerMp = GetComponent<PlayerMp>();

        Transform[] transforms = GetComponentsInChildren<Transform>();

        _hpBar = transforms[(int)Status.Hp];
        _mpBar = transforms[(int)Status.Mp];

        SetBarScale(_hpBar, 1.0f);
        SetBarScale(_mpBar, 0.0f);
    }

    private void OnEnable()
    {
        _playerHp.OnHpChanged += UpdateHpBar;
        _playerMp.OnMpChanged += UpdateMpBar;
    }

    private void OnDisable()
    {
        _playerHp.OnHpChanged -= UpdateHpBar;
        _playerMp.OnMpChanged -= UpdateMpBar;
    }

    private void SetBarScale(Transform bar, float ratio)
    {
        Vector3 scale = bar.localScale;
        scale.x = Mathf.Clamp01(ratio);
        bar.localScale = scale;
    }

    private void UpdateHpBar(float cur, float max)
    {
        float ratio = cur / max;

        Vector3 scale = _hpBar.localScale;
        scale.x = ratio;
        _hpBar.localScale = scale;
    }

    private void UpdateMpBar(float cur, float max)
    {
        float ratio = cur / max;

        Vector3 scale = _mpBar.localScale;
        scale.x = ratio;
        _mpBar.localScale = scale;
    }
}