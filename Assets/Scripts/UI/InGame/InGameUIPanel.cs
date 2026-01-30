using System;
using System.Collections.Generic;
using UnityEngine;

public enum InGameUI
{
    OptionPanel = 1, 
    WaveTimerPanel = 4,
    BattleSetUpPanel = 10,
    CoinUI = 12,
    CoinText = 13,
    RerollUIRoot = 14,
    LeftCardBtn = 19,
    CenterCardBtn = 28, 
    RightCardBtn = 37,
    RerollImage = 45, 
    RerollBtn = 46,
    SlotMachinePanel = 55,
    ResultPanel = 80
}

public class InGameUIPanel : MonoBehaviour
{
    private RerollUIController _rerollUI;
    private Animator _setUpAnim;
    private Transform[] _inGameUIs;

    private void Awake()
    {
        _inGameUIs = GetComponentsInChildren<Transform>();

        Transform setUpPanel = _inGameUIs[(int)InGameUI.BattleSetUpPanel];
        _setUpAnim = setUpPanel.GetComponent<Animator>();
        _setUpAnim.Play("BtnsIntro");
    }

    private void Start()
    {
        PanelInit();
    }

    private void OnEnable()
    {
        BattleStateManager.Instance.OnBattleEnd += ShowResult;
        BattleStateManager.Instance.OnReroll += ShowRerollUI;
        BattleStateManager.Instance.OnEnteringBattle += ShowBattleUI;
    }

    private void OnDisable()
    {
        BattleStateManager.Instance.OnBattleEnd -= ShowResult;
        BattleStateManager.Instance.OnReroll -= ShowRerollUI;
        BattleStateManager.Instance.OnEnteringBattle -= ShowBattleUI;
    }

    private void ShowRerollUI()
    {
        _setUpAnim.SetTrigger("RerollStart");
    }

    private void PanelInit()
    {
        _inGameUIs[(int)InGameUI.ResultPanel].gameObject.SetActive(false);
    }

    private void ShowResult()
    {
        _inGameUIs[(int)InGameUI.ResultPanel].gameObject.SetActive(true);
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