using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveTimerPanel : MonoBehaviour
{
    private enum WaveTimerUI
    {
        TimerImage = 2, TimerText = 3, WaveText = 5
    }

    private Image _timerImage;
    private Transform[] _waveTimerChildren;
    private TextMeshProUGUI _timerText;
    private InGameUIPanel _inGameUIPanel;

    private BattleState _prevState = BattleState.None;

    private readonly float _rerollDuration = 60.0f;
    private readonly float _battleDuration = 70.0f; // 1분 10초
    private readonly float _minute = 60.0f;
    private float _curDuration;
    private float _timer;
    private bool IsTimerRunning => _timer > 0.0f;

    private void Awake()
    {
        _inGameUIPanel = GetComponentInParent<InGameUIPanel>();
        _waveTimerChildren = GetComponentsInChildren<Transform>();
        _timerImage = _waveTimerChildren[(int)WaveTimerUI.TimerImage].GetComponent<Image>();
        _timerText = _waveTimerChildren[(int)WaveTimerUI.TimerText].GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        BattleState currentState = BattleStateManager.Instance.CurrentState;

        if (_prevState != currentState)
        {
            HandleStateChange(currentState);
        }

        if (currentState != BattleState.Victory)
        { 
            UpdateTimer(currentState); 
        }
    }

    private void HandleStateChange(BattleState currentState)
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

    private void UpdateTimer(BattleState currentState)
    {
        if (IsTimerRunning)
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
            HandleTimerEnd(currentState);
        }
    }

    private void HandleTimerEnd(BattleState currentState)
    {
        if (currentState == BattleState.Reroll)
        {
            BattleStateManager.Instance.SetState(BattleState.Battle);
        }
        else if (currentState == BattleState.Battle)
        {
            BattleStateManager.Instance.SetState(BattleState.Reroll);
        }
    }
}