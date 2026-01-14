using System.Collections.Generic;
using UnityEngine;

public enum InGameUI
{
    OptionPanel = 1, 
    WaveTimerPanel = 4,
    BattleSetUpPanel = 10,
    CoinText = 12,
    RerollUIRoot = 13,
    LeftCardBtn = 14,
    CenterCardBtn = 22, 
    RightCardBtn = 30,
    RerollImage = 38, 
    RerollBtn = 39,
    SlotMachinePanel = 48
}

public class InGameUIPanel : MonoBehaviour
{
    private Transform[] _inGameUIs;
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
        _inGameUIs = GetComponentsInChildren<Transform>();

        Transform setUpPanel = _inGameUIs[(int)InGameUI.BattleSetUpPanel];
        _setUpAnim = setUpPanel.GetComponent<Animator>();
    }

    private void Update()
    {
        SetUIState();

        if (_isDeckMode && Input.GetKeyDown(KeyCode.Alpha4))
        {
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

                case BattleState.EnteringBattle:
                    ShowBattleUI(); // 배틀 상태일 때의 UI
                    break;
            }

            _prevState = currentState; // 상태 갱신
        }
    }

    private void ShowRerollUI()
    {
        _inGameUIs[(int)InGameUI.WaveTimerPanel].gameObject.SetActive(true);
        _inGameUIs[(int)InGameUI.OptionPanel].gameObject.SetActive(true);
        _inGameUIs[(int)InGameUI.BattleSetUpPanel].gameObject.SetActive(true);
        _setUpAnim.SetTrigger("RerollStart");
    }

    private void ShowBattleUI()
    {
        _setUpAnim.SetTrigger("EnteringBattle");
    }

    public void HideSlotMachine()
    {
        _inGameUIs[(int)InGameUI.SlotMachinePanel].gameObject.SetActive(false);
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

    public List<PlayerData> GetRerollCandidates()
    {
        // 배치 가능한 플레이어 데이터 가져오기
        List<PlayerData> deployedPlayers = InGamePlayerSpawn.Instance.DeployableData;

        return deployedPlayers;
    }
}