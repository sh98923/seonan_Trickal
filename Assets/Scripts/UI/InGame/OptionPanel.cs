using UnityEngine;
using UnityEngine.UI;

public class OptionPanel : MonoBehaviour
{
    private enum OptionUI
    {
        InGameSpeed, Pause
    }

    private enum GameSpeed
    {
        x1 = 1, x2 = 10 
    }

    private Transform[] _optionChildren;
    private Button[] _optionButtons;
    private Sprite[] _inGameSpeedSprites;
    private Image _speedButtonImage;

    private GameSpeed _curGameSpeed = GameSpeed.x1;

    private float _savedGameSpeed = 1.0f;
    private int _curSpeedIndex = 0;
    private bool _isPaused = false;

    private void Awake()
    {
        _optionChildren = GetComponentsInChildren<Transform>();
        _optionButtons = GetComponentsInChildren<Button>();
        _savedGameSpeed = Time.timeScale;
    }

    private void Start()
    {
        _inGameSpeedSprites = Resources.LoadAll<Sprite>("Sprites/Option");

        _speedButtonImage = _optionButtons[(int)OptionUI.InGameSpeed].GetComponent<Image>();

        _optionButtons[(int)OptionUI.Pause].onClick.AddListener(OnClickPause);
        _optionButtons[(int)OptionUI.InGameSpeed].onClick.AddListener(OnClickInGameSpeed);

        UpdateSpeedSprite();
    }

    private void OnEnable()
    {
        BattleStateManager.Instance.OnBattleEnd += GameSpeedInit;
    }

    private void OnDisable()
    {
        BattleStateManager.Instance.OnBattleEnd -= GameSpeedInit;
    }

    private void GameSpeedInit()
    {
        Time.timeScale = 1.0f;
    }

    private void OnClickPause()
    {
        if (_isPaused)
        {
            Time.timeScale = _savedGameSpeed;
        }
        else
        {
            _savedGameSpeed = Time.timeScale;
            Time.timeScale = 0.0f;
        }

        _isPaused = !_isPaused;
    }

    private void OnClickInGameSpeed()
    {
        _curGameSpeed = _curGameSpeed == GameSpeed.x1 ? GameSpeed.x2 : GameSpeed.x1;
        _curSpeedIndex = (int)_curGameSpeed - 1;
        Time.timeScale = (float)_curGameSpeed;
        _savedGameSpeed = Time.timeScale;

        UpdateSpeedSprite();
    }

    private void UpdateSpeedSprite()
    {
        if (_inGameSpeedSprites.Length >= 2)
        {
            _speedButtonImage.sprite = _inGameSpeedSprites[_curSpeedIndex];
        }
    }
}
