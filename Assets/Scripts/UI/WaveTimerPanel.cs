using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveTimerPanel : MonoBehaviour
{
    private enum WaveTimerElement
    {
        TimerImage = 2,
        TimerText = 3,
        WaveText = 5
    }

    private Image _timerImage;
    private Transform[] _waveTimerChildren;
    private TextMeshProUGUI _timerText;
    private InGameUIPanel _inGameUIPanel;
    private Animator _animator;

    private readonly float _rerollDuration = 60.0f;
    private readonly float _battleDuration = 70.0f; // 1분 10초
    private readonly float _minute = 60.0f;
    private float _curDuration;
    private float _timer;

    private BattleState _prevState = BattleState.None;

    private void Awake()
    {
        _inGameUIPanel = GetComponentInParent<InGameUIPanel>();
        _waveTimerChildren = GetComponentsInChildren<Transform>();
        _timerImage = _waveTimerChildren[(int)WaveTimerElement.TimerImage].GetComponent<Image>();
        _timerText = _waveTimerChildren[(int)WaveTimerElement.TimerText].GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        _animator = _inGameUIPanel.GetUIElement<Animator>(InGameUIElement.BattleStartBtnPanel);
    }

    private void Update()
    {
        BattleState currentState = BattleStateManager.Instance.CurrentState;

        // 상태 변경 시 타이머 초기화
        if (_prevState != currentState)
        {
            _prevState = currentState;

            switch (currentState)
            {
                case BattleState.Reroll:
                    _curDuration = _rerollDuration;
                    break;
                case BattleState.Battle:
                    _curDuration = _battleDuration;
                    break;
                default:
                    _curDuration = 0.0f;
                    break;
            }

            _timer = _curDuration;
        }

        // 타이머 작동
        if (_timer >= 0.0f)
        {
            _timer -= Time.deltaTime;

            float normalizedTime = Mathf.Clamp01(_timer / _curDuration);
            _timerImage.fillAmount = normalizedTime;

            int minutes = Mathf.FloorToInt(_timer / _minute);
            int seconds = Mathf.FloorToInt(_timer % _minute);
            _timerText.text = $"{minutes:D2}:{seconds:D2}";
        }
        else
        {
            if(currentState == BattleState.Reroll)
            {
                _animator.SetTrigger("BattleStart");
                BattleStateManager.Instance.SetState(BattleState.Battle);
            }
            else if(currentState == BattleState.Battle)
            {
                _animator.SetTrigger("RerollStart");
                BattleStateManager.Instance.SetState(BattleState.Reroll);
            }
        }
    }
}
