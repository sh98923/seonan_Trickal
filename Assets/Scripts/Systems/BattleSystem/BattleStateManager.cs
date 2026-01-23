using System;
using UnityEngine;

public enum BattleState
{
    None,
    Reroll,
    Battle,
    EnteringReroll,
    EnteringBattle,
    // GameStatemanager를 따로 스크립트 만들어서
    // battleStatemanager처럼 클래스를 만들자
    Victory,
    Defeat
}

public class BattleStateManager : MonoBehaviour
{
    private static BattleStateManager _instance;
    public static BattleStateManager Instance
    {
        get { return _instance;}
    }

    private bool _isWaveCleared = false;
    public bool IsWaveCleared
    {
        get { return _isWaveCleared; }
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
    private event Action _onEnteringReroll;
    public event Action OnEnteringReroll
    {
        add { _onEnteringReroll += value; } //print("구독 : " + value.Method.Name); }
        remove { _onEnteringReroll -= value; } //print("해제 : " + value.Method.Name); }
    }
    private event Action _onEnteringBattle;
    public event Action OnEnteringBattle
    {
        add { _onEnteringBattle += value; } //print("구독 : " + value.Method.Name); }
        remove { _onEnteringBattle -= value; } //print("해제 : " + value.Method.Name); }
    }
    private event Action _onGameVictory;
    public event Action OnGameVictory
    {
        add { _onGameVictory += value; }
        remove { _onGameVictory -= value; }
    }
    private event Action _onGameDefeat;
    public event Action OnGameDefeat
    {
        add { _onGameDefeat += value; }
        remove { _onGameDefeat -= value; }
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

    private void InitWaveMark()
    {
        _isWaveCleared = false;
    }

    public void OnInGameEnter()
    {
        SetState(BattleState.Reroll);
    }

    public void MarkWaveCleared()
    {
        _isWaveCleared = true;
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
        get { return _currentState == BattleState.Reroll; }
    }

    public bool IsEnteringBattle
    {
        get { return _currentState == BattleState.EnteringBattle; }
    }

    public bool IsGameOver
    {
        get { return _currentState == BattleState.Defeat; }
    }

    public void SetState(BattleState newState)
    {
        _currentState = newState;

        print("현재 배틀 상태 : " + _currentState);

        switch (_currentState)
        {
            case BattleState.Reroll:
                _onReroll?.Invoke();
                InitWaveMark();
                break;
            case BattleState.Battle:
                _onBattle?.Invoke();
                break;
            case BattleState.EnteringReroll:
                _onEnteringReroll?.Invoke();
                break;
            case BattleState.EnteringBattle:
                _onEnteringBattle?.Invoke();
                break;
            case BattleState.Victory:
                _onGameVictory?.Invoke();
                break;
            case BattleState.Defeat:
                _onGameDefeat?.Invoke();
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