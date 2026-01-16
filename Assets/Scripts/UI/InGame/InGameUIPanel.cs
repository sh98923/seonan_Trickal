using System.Collections.Generic;
using UnityEngine;

public enum InGameUI
{
    OptionPanel = 1, 
    WaveTimerPanel = 4,
    BattleSetUpPanel = 10,
    CoinText = 12,
    RerollUIRoot = 13,
    LeftCardBtn = 18,
    CenterCardBtn = 26, 
    RightCardBtn = 34,
    RerollImage = 39, 
    RerollBtn = 40,
    SlotMachinePanel = 49
}

public class InGameUIPanel : MonoBehaviour
{
    private RerollUIController _rerollUI;
    private Animator _setUpAnim;
    private Transform[] _inGameUIs;

    private void Awake()
    {
        _inGameUIs = GetComponentsInChildren<Transform>();

        _rerollUI = _inGameUIs[(int)InGameUI.RerollUIRoot].GetComponent<RerollUIController>();

        Transform setUpPanel = _inGameUIs[(int)InGameUI.BattleSetUpPanel];
        _setUpAnim = setUpPanel.GetComponent<Animator>();
        _setUpAnim.Play("BtnsIntro");
    }

    private void OnEnable()
    {
        BattleStateManager.Instance.OnReroll += ShowRerollUI;
        BattleStateManager.Instance.OnEnteringBattle += ShowBattleUI;
    }

    private void OnDisable()
    {
        BattleStateManager.Instance.OnReroll -= ShowRerollUI;
        BattleStateManager.Instance.OnEnteringBattle -= ShowBattleUI;
    }

    private void ShowRerollUI()
    {
        //_inGameUIs[(int)InGameUI.WaveTimerPanel].gameObject.SetActive(true);
        //_inGameUIs[(int)InGameUI.OptionPanel].gameObject.SetActive(true);
        //_inGameUIs[(int)InGameUI.BattleSetUpPanel].gameObject.SetActive(true);
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