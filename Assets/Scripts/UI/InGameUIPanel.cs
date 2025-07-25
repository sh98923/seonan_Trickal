using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InGameUIElement
{
    DeckPanel = 1, StageName = 2, InGameStartBtn = 3,
    DeckContent = 6, BattleStartPanel = 7, CardRerollPanel = 11,
    LeftCardBtn = 14, CenterCardBtn = 21, RightCardBtn = 28,
    RerollImage = 33, RerollBtn = 34, TimeControlPanel = 38
}

public class InGameUIPanel : MonoBehaviour
{
    private Transform[] _inGameUIs;
    private Transform _spawnParent;
    public Transform SpawnParent
    {
        get { return _spawnParent; }
    }

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

        SetInit();
    }

    private void Update()
    {
        SetUIState();

        if (_isDeckMode && Input.GetKeyDown(KeyCode.Alpha4))
        {
            _inGameUIs[(int)InGameUIElement.DeckPanel].gameObject.SetActive(true);
            _inGameUIs[(int)InGameUIElement.BattleStartPanel].gameObject.SetActive(false);
            _inGameUIs[(int)InGameUIElement.CardRerollPanel].gameObject.SetActive(false);

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
                    _inGameUIs[(int)InGameUIElement.BattleStartPanel].gameObject.SetActive(true);
                    _inGameUIs[(int)InGameUIElement.CardRerollPanel].gameObject.SetActive(true);
                    _inGameUIs[(int)InGameUIElement.TimeControlPanel].gameObject.SetActive(true);
                    break;

                case BattleState.Battle:
                    _inGameUIs[(int)InGameUIElement.CardRerollPanel].gameObject.SetActive(false);
                    break;
            }

            _prevState = currentState; // 상태 갱신
        }
    }

    private void SetInit()
    {
        _inGameUIs[(int)InGameUIElement.BattleStartPanel].gameObject.SetActive(false);
        _inGameUIs[(int)InGameUIElement.CardRerollPanel].gameObject.SetActive(false);
        _inGameUIs[(int)InGameUIElement.TimeControlPanel].gameObject.SetActive(false);
    }

    // 나중에 필요 없으면 아래 두개 함수 다 지워
    // 이 함수는 인게임 UI에서 필요한 컴포넌트를 자식 오브젝트에서 접근하는 것
    public T GetUIElement<T>(InGameUIElement element) where T : Component
    {
        return _inGameUIs[(int)element].GetComponent<T>();
    }

    public GameObject GetUIElement(InGameUIElement element)
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