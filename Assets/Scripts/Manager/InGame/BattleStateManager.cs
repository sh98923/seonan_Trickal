using System;
using UnityEngine;

public enum BattleState
{
    None,
    Reroll,
    Battle,
    WaveAdvance,
    // GameStatemanager를 따로 스크립트 만들어서
    // battleStatemanager처럼 클래스를 만들자
    Victory,
    GameOver
}

public class BattleStateManager : MonoBehaviour
{
    private static BattleStateManager _instance;
    public static BattleStateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("BattleStateManager가 씬에 존재하지 않습니다!");
            }
            return _instance;
        }
    }

    private event Action _onReroll;
    public event Action OnReroll
    {
        add { _onReroll += value; }//print("구독 : " + value.Method.Name); }
        remove { _onReroll -= value; }//print("해제 : " + value.Method.Name); }
    }
    private event Action _onBattle;
    public event Action OnBattle
    {
        add { _onBattle += value; } //print("구독 : " + value.Method.Name); }
        remove { _onBattle -= value; } //print("해제 : " + value.Method.Name); }
    }
    private event Action _onWaveAdvance;
    public event Action OnWaveAdvance
    {
        add { _onWaveAdvance += value; } //print("구독 : " + value.Method.Name); }
        remove { _onWaveAdvance -= value; } //print("해제 : " + value.Method.Name); }
    }

    private BattleState _currentState = BattleState.None;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // 중복 생성 방지
        }
    }

    public BattleState CurrentState
    {
        get { return _currentState; }
    }

    public bool IsBattle
    {
        get { return _currentState == BattleState.Battle; }
    }

    public bool IsReroll
    {
        get
        {
            return _currentState == BattleState.Reroll;
        }
    }

    public bool IsGameOver
    {
        get { return _currentState == BattleState.GameOver; }
    }

    public void SetState(BattleState newState)
    {
        _currentState = newState;

        switch (_currentState)
        {
            case BattleState.Reroll:
                _onReroll?.Invoke();
                break;
            case BattleState.Battle:
                _onBattle?.Invoke();
                break;
            case BattleState.WaveAdvance:
                _onWaveAdvance?.Invoke();
                break;

        }
    }

    /*public void PrintCurState()
    {
        print(_currentState);
    }

    public void RerollEvent()
    {
        _onReroll?.Invoke();
    }*/
}