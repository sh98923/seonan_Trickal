using System.Collections.Generic;
using UnityEngine;

public class InGameUIPanelManager : Singleton<InGameUIPanelManager>
{
    private enum InGameUIElement
    {
        DeckContent = 6, SelectCardPanel = 7,
        LeftCardBtn = 10, CenterCardBtn = 15, RightCardBtn = 20,
        RerollImage = 23, RerollBtn = 24
    }

    private Transform[] _inGameUIs;
    private Transform _spawnParent;
    public Transform SpawnParent
    {
        get { return _spawnParent; }
    }
    
    private Transform _inGameUIPanel;

    private bool _isClickedReroll = false;
    public bool IsClickedReroll
    {
        get { return _isClickedReroll; }
        set {  _isClickedReroll = value; }
    }

    private void Awake()
    {
        _inGameUIPanel = GameObject.Find("InGameUIPanel").transform;
        _spawnParent = GameObject.Find("SpawnPlayer").transform;
        _inGameUIs = _inGameUIPanel.GetComponentsInChildren<Transform>();

        SetInit();
    }

    private void SetInit()
    {
        // 시작 시 비활성화할 UI 요소들
        Transform[] panelsToDisable = new Transform[]
        {
            _inGameUIs[(int)InGameUIElement.SelectCardPanel],
            _inGameUIs[(int)InGameUIElement.RerollImage]
        };

        foreach (Transform panel in panelsToDisable)
        {
            panel.gameObject.SetActive(false);
        }
    }

    public List<PlayerData> SetRerollCandidates()
    {
        // 배치된 플레이어 데이터 가져오기
        PlayerSpawn playerSpawn = _spawnParent.GetComponent<PlayerSpawn>();
        List<PlayerData> deployedPlayers = playerSpawn.DeployedDatas;

        return deployedPlayers;
        /*bool isBattleStarted = BattleStateManager.Instance.IsBattleStart;

        if (!isBattleStarted || _isCardSetupComplete)
            return;

        _isCardSetupComplete = true;

        // 리롤 버튼에서 CardReroll 컴포넌트 참조
        bool isReroll = _inGameUIs[(int)InGameUIElement.RerollBtn]
            .GetComponent<CardReroll>().IsReroll;

        // 리롤 후보 세팅
        reroll.SetRerollCandidates(deployedPlayers);*/
    }
}