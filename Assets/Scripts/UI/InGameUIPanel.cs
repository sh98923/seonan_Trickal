using System.Collections.Generic;
using UnityEngine;

public enum InGameUIElement
{
    DeckPanel = 1, StageName = 2, InGameStartBtn = 3,
    DeckContent = 6, CardRerollPanel = 7,
    LeftCardBtn = 10, CenterCardBtn = 15, RightCardBtn = 20,
    RerollImage = 23, RerollBtn = 24
}

public class InGameUIPanel : MonoBehaviour
{
    private Transform[] _inGameUIs;
    private Transform _spawnParent;
    public Transform SpawnParent
    {
        get { return _spawnParent; }
    }

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
        if (_isDeckMode && Input.GetKeyDown(KeyCode.Alpha4))
        {
            _inGameUIs[(int)InGameUIElement.DeckPanel].gameObject.SetActive(true);
            _inGameUIs[(int)InGameUIElement.CardRerollPanel].gameObject.SetActive(false);

            _isDeckMode = false;
            BattleStateManager.Instance.IsBattleStart = false;
        }
    }

    private void SetInit()
    {
        _inGameUIs[(int)InGameUIElement.CardRerollPanel].gameObject.SetActive(false);
    }

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