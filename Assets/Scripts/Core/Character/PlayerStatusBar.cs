using UnityEngine;

public class PlayerStatusBar : CharacterStatusBar
{
    private enum PlayerStatus
    {
        Hp = 1,
        Mp
    }

    private PlayerMp _playerMp;
    
    private Transform _mpBar;

    private void Awake()
    {
        _playerMp = GetComponent<PlayerMp>();

        Transform[] transforms = GetComponentsInChildren<Transform>();
        _hpBar = transforms[(int)PlayerStatus.Hp];
        _mpBar = transforms[(int)PlayerStatus.Mp];

        base.Awake();

        SetBarScale(_mpBar, 0.0f);
    }

    private void OnEnable()
    {
        base.OnEnable();
        _playerMp.OnMpChanged += UpdateMpBar;
        UpdateMpBar(_playerMp.CurMp, _playerMp.MaxMp);
    }

    private void OnDisable()
    {
        base.OnDisable();
        _playerMp.OnMpChanged -= UpdateMpBar;
    }

    private void UpdateMpBar(float cur, float max)
    {
        float ratio = cur / max;

        Vector3 scale = _mpBar.localScale;
        scale.x = ratio;
        _mpBar.localScale = scale;
    }
}