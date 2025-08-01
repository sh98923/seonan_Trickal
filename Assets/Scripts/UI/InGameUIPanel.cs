using System.Collections.Generic;
using UnityEngine;

public enum InGameUI
{
    DeckPanel = 1, StageName = 2, InGameStartBtn = 3,
    DeckContent = 6, BattleSetUpPanel = 7,
    LeftCardBtn = 10, CenterCardBtn = 17, RightCardBtn = 24,
    RerollImage = 29, RerollBtn = 30, TimeControlPanel = 39, WaveTimerPanel = 42
}

public class InGameUIPanel : MonoBehaviour
{
    private Transform[] _inGameUIs;
    private Transform _spawnParent;
    public Transform SpawnParent
    {
        get { return _spawnParent; }
    }
    private Animator _setUpAnim;
    private BattleState _prevState = BattleState.None;

    private bool _isDeckMode = false;
    public bool IsDeckMode
    {
        get { return _isDeckMode; }
        set { _isDeckMode = value; }    
    }

    private void Awake()
    {
        _spawnParent = GameObject.Find("SpawnPlayer").transform;
        _inGameUIs = GetComponentsInChildren<Transform>();

        Transform setUpPanel = _inGameUIs[(int)InGameUI.BattleSetUpPanel];
        _setUpAnim = setUpPanel.GetComponent<Animator>();

        SetUIInit();
    }

    private void Update()
    {
        SetUIState();

        if (_isDeckMode && Input.GetKeyDown(KeyCode.Alpha4))
        {
            _inGameUIs[(int)InGameUI.DeckPanel].gameObject.SetActive(true);
            _inGameUIs[(int)InGameUI.BattleSetUpPanel].gameObject.SetActive(false);

            _isDeckMode = false;
            BattleStateManager.Instance.SetState(BattleState.None);
        }
    }

    private void SetUIState()
    {
        BattleState currentState = BattleStateManager.Instance.CurrentState;

        // 상태가 변경되었을 때만 처리
        if (currentState != _prevState)
        {
            switch (currentState)
            {
                case BattleState.Reroll:
                    ShowRerollUI(); // 리롤 상태일 때의 UI
                    break;

                case BattleState.Battle:
                    ShowBattleUI(); // 배틀 상태일 때의 UI
                    break;
            }

            _prevState = currentState; // 상태 갱신
        }
    }

    private void SetUIInit()
    {
        _inGameUIs[(int)InGameUI.WaveTimerPanel].gameObject.SetActive(false);
        _inGameUIs[(int)InGameUI.TimeControlPanel].gameObject.SetActive(false);
        _inGameUIs[(int)InGameUI.BattleSetUpPanel].gameObject.SetActive(false);
    }

    private void ShowRerollUI()
    {
        _inGameUIs[(int)InGameUI.WaveTimerPanel].gameObject.SetActive(true);
        _inGameUIs[(int)InGameUI.TimeControlPanel].gameObject.SetActive(true);
        _inGameUIs[(int)InGameUI.BattleSetUpPanel].gameObject.SetActive(true);
        _setUpAnim.SetTrigger("RerollStart");
    }

    private void ShowBattleUI()
    {
        _setUpAnim.SetTrigger("BattleStart");
    }

    // 나중에 필요 없으면 아래 두개 함수 다 지워
    // 이 함수는 인게임 UI에서 필요한 컴포넌트를 자식 오브젝트에서 접근하는 것
    public T GetUIElement<T>(InGameUI element) where T : Component
    {
        return _inGameUIs[(int)element].GetComponent<T>();
    }

    public GameObject GetUIElement(InGameUI element)
    {
        return _inGameUIs[(int)element].gameObject;
    }

    public List<PlayerData> SetRerollCandidates()
    {
        // 배치된 플레이어 데이터 가져오기
        PlayerSpawn playerSpawn = _spawnParent.GetComponent<PlayerSpawn>();
        List<PlayerData> deployedPlayers = playerSpawn.DeployedDatas;

        return deployedPlayers;
    }
}