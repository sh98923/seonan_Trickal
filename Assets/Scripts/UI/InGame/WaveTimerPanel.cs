/*using TMPro;
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
    private TextMeshProUGUI _waveText;
    private InGameUIPanel _inGameUIPanel;

    private BattleState _prevState = BattleState.None;

    private const float _minute = 60.0f;
    private const float _rerollDuration = 60.0f;
    private const float _battleDuration = 70.0f; // 1분 10초

    private float _duration = 0.0f;
    private float _timer = _rerollDuration;

    private int _waveStep = 0;
    private int _maxWave = 0;

    private bool IsTimerRunning => _timer > 0.0f;

    private void Awake()
    {
        _inGameUIPanel = GetComponentInParent<InGameUIPanel>();
        _waveTimerChildren = GetComponentsInChildren<Transform>();
        _waveText = _waveTimerChildren[(int)WaveTimerUI.WaveText].GetComponent<TextMeshProUGUI>();
        _timerImage = _waveTimerChildren[(int)WaveTimerUI.TimerImage].GetComponent<Image>();
        _timerText = _waveTimerChildren[(int)WaveTimerUI.TimerText].GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        BattleStateManager.Instance.OnReroll += UpdateWaveText;
        BattleStateManager.Instance.OnReroll += RerollDuration;
        BattleStateManager.Instance.OnBattle += BattleDuration;
    }   

    private void OnDisable()
    {
        BattleStateManager.Instance.OnReroll -= UpdateWaveText;
        BattleStateManager.Instance.OnReroll -= RerollDuration;
        BattleStateManager.Instance.OnBattle -= BattleDuration;
    }

    private void OnDestroy()
    {
        if(BattleStateManager.Instance != null)
        {
            BattleStateManager.Instance.OnReroll -= UpdateWaveText;
            BattleStateManager.Instance.OnReroll -= RerollDuration;
            BattleStateManager.Instance.OnBattle -= BattleDuration;
        }
    }

    private void Update()
    {
        BattleState currentState = BattleStateManager.Instance.CurrentState;

        if (_prevState != currentState)
        {
            HandleStateChange(currentState);
        }

        if (currentState != BattleState.None)
        { 
            UpdateTimer(currentState, _duration); 
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

    private void RerollDuration()
    {
        _duration = _rerollDuration;
        _timer = _rerollDuration;
    }

    private void BattleDuration()
    {
        _duration = _battleDuration;
        _timer = _battleDuration;
    }

    private void UpdateTimer(BattleState currentState, float duration)
    {
        if (IsTimerRunning)
        {
            _timer -= Time.deltaTime; 
            _timer = Mathf.Max(_timer, 0f);

            float normalizedTime = Mathf.Clamp01(_timer / duration);
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
        switch (currentState)
        {
            case BattleState.Reroll:
                InGameManager.Instance.IsGameStart = true;
                BattleStateManager.Instance.SetState(BattleState.EnteringBattle);
                break;
            case BattleState.Battle:
                CheckBattleResultState();
                break;
            case BattleState.EnteringReroll:
                break;
            case BattleState.EnteringBattle:
                break;
            case BattleState.Victory:
                break;
            case BattleState.Defeat:
                break;
        }
    }

    private void UpdateWaveText()
    {
        _waveStep = InGameManager.Instance.WaveStep;
        _maxWave = InGameManager.Instance.MaxWave;

        _waveText.text = $"{_waveStep}/{_maxWave}";
    }

    private void CheckBattleResultState()
    {
        foreach (ITrackable unit in InGameManager.Instance.Monsters)
        {
            if (!unit.IsColliderEnable)
            { 
                continue;
            }

            BattleStateManager.Instance.SetState(BattleState.Defeat);
            print("게임 오버");
            return; // 몬스터가 하나라도 있으면 바로 종료
        }

        // 몬스터가 없음 → 리롤
        BattleStateManager.Instance.SetState(BattleState.Reroll);
    }
}*/
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WaveTimerPanel : MonoBehaviour
{
    private enum WaveTimerUI
    {
        TimerImage = 2,
        TimerText = 3,
        WaveText = 5
    }

    private Image _timerImage;
    private Transform[] _waveTimerChildren;
    private TextMeshProUGUI _timerText;
    private TextMeshProUGUI _waveText;
    private InGameUIPanel _inGameUIPanel;

    private const float _minute = 60.0f;
    private const float _rerollDuration = 60.0f;
    private const float _battleDuration = 70.0f;

    private float _duration;
    private float _timer;

    private int _waveStep;
    private int _maxWave;

    private Coroutine _timerCoroutine;

    private void Awake()
    {
        _inGameUIPanel = GetComponentInParent<InGameUIPanel>();
        _waveTimerChildren = GetComponentsInChildren<Transform>();

        _timerImage = _waveTimerChildren[(int)WaveTimerUI.TimerImage].GetComponent<Image>();

        _waveText = _waveTimerChildren[(int)WaveTimerUI.WaveText].GetComponent<TextMeshProUGUI>();
        _timerText = _waveTimerChildren[(int)WaveTimerUI.TimerText].GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        BattleStateManager.Instance.OnReroll += OnRerollStart;
        BattleStateManager.Instance.OnBattle += OnBattleStart;
    }

    private void OnDisable()
    {
        if (BattleStateManager.Instance == null)
            return;

        BattleStateManager.Instance.OnReroll -= OnRerollStart;
        BattleStateManager.Instance.OnBattle -= OnBattleStart;

        StopTimer();
    }

    // ===============================
    // BattleState 진입 지점
    // ===============================

    private void OnRerollStart()
    {
        UpdateWaveText();
        StartTimer(_rerollDuration);
    }

    private void OnBattleStart()
    {
        StartTimer(_battleDuration);
    }

    // ===============================
    // Timer Control
    // ===============================

    private void StartTimer(float duration)
    {
        StopTimer();
        _timerCoroutine = StartCoroutine(TimerCoroutine(duration));
    }

    private void StopTimer()
    {
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }
    }

    private IEnumerator TimerCoroutine(float duration)
    {
        _duration = duration;
        _timer = duration;

        while (_timer > 0.0f)
        {
            BattleState state = BattleStateManager.Instance.CurrentState;

            // 타이머가 돌아야 하는 상태만 허용
            if (state != BattleState.Reroll &&
                state != BattleState.Battle)
            {
                yield break;
            }

            _timer -= Time.deltaTime;
            _timer = Mathf.Max(_timer, 0f);

            UpdateTimerUI();

            yield return null;
        }

        HandleTimerEnd(BattleStateManager.Instance.CurrentState);
    }

    // ===============================
    // Timer UI
    // ===============================

    private void UpdateTimerUI()
    {
        float normalizedTime = Mathf.Clamp01(_timer / _duration);
        _timerImage.fillAmount = normalizedTime;

        int minutes = Mathf.FloorToInt(_timer / _minute);
        int seconds = Mathf.FloorToInt(_timer % _minute);
        _timerText.text = $"{minutes:D2}:{seconds:D2}";
    }

    // ===============================
    // Timer End
    // ===============================

    private void HandleTimerEnd(BattleState currentState)
    {
        if (currentState == BattleState.Victory ||
            currentState == BattleState.Defeat)
            return;

        switch (currentState)
        {
            case BattleState.Reroll:
                InGameManager.Instance.IsGameStart = true;
                BattleStateManager.Instance.SetState(BattleState.EnteringBattle);
                break;

            case BattleState.Battle:
                CheckBattleResultState();
                break;
        }
    }

    // ===============================
    // Wave / Battle Result
    // ===============================

    private void UpdateWaveText()
    {
        _waveStep = InGameManager.Instance.WaveStep;
        _maxWave = InGameManager.Instance.MaxWave;

        _waveText.text = $"{_waveStep}/{_maxWave}";
    }

    private void CheckBattleResultState()
    {
        foreach (ITrackable unit in InGameManager.Instance.Monsters)
        {
            if (!unit.IsColliderEnable)
                continue;

            BattleStateManager.Instance.SetState(BattleState.Defeat);
            Debug.Log("게임 오버");
            return;
        }

        BattleStateManager.Instance.SetState(BattleState.Reroll);
    }
}
