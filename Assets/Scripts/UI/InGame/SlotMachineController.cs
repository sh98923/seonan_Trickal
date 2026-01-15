using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class SlotMachineController : MonoBehaviour
{
    private static SlotMachineController _instance;
    public static SlotMachineController Instance
    {
        get { return _instance; }
    }

    private event Action _onSlotMachineFinished;
    public event Action OnSlotMachineFinished
    {
        add { _onSlotMachineFinished += value; }
        remove { _onSlotMachineFinished -= value;}
    }

    private InGameUIPanel _rootPanel;
    private Animator _slotMahcineAnim;
    private SlotReel[] _slots;
    private List<PlayerData> _playerDatas;

    private const float _rollStartDuartion = 0.95f;   // 전체 릴이 도는 시간
    private const float _rollStopInterval = 0.25f;   // 릴 간 정지 간격
    private const float _resultHoldDuration = 0.55f;  // 결과 보여주는 시간

    private int[] _resultPlayerKeys = new int[3];
    public int[] SlotMachineResultPlayerKeys
    {
        get { return _resultPlayerKeys; }
    }

    private void Awake()
    {
        _instance = this;

        _rootPanel = GetComponentInParent<InGameUIPanel>();
        _slotMahcineAnim = GetComponent<Animator>();
        _slots = GetComponentsInChildren<SlotReel>();

        _playerDatas = GameManager.Instance.SpawnablePlayerDatas;
    }

    private void Start()
    {
        AssignResults();
    }

    private void AssignResults()
    {
        List<PlayerData> playerList = new List<PlayerData>(_playerDatas);

        for (int i = 0; i < _slots.Length; i++)
        {
            int rand = UnityEngine.Random.Range(0, playerList.Count);
            int playerKey = playerList[rand].Key;
            
            _resultPlayerKeys[i] = playerKey;
            
            Sprite sprite = PlayerManager.Instance.GetPlayerSlotMachineSprite(playerKey);

            playerList.RemoveAt(rand); // 중복 방지

            _slots[i].SetResultPlayer(i, sprite, playerList);
        }
    }

    public void StartSlot()
    {
        StartCoroutine(SlotSequence());
    }

    private IEnumerator SlotSequence()
    {
        // 시작
        foreach (SlotReel reel in _slots)
        {
            reel.StartRoll();
        }

        yield return new WaitForSeconds(_rollStartDuartion);

        // 정지
        foreach (SlotReel reel in _slots)
        {
            reel.StopRoll();
            yield return new WaitForSeconds(_rollStopInterval);
        }

        yield return new WaitForSeconds(_resultHoldDuration);

        _onSlotMachineFinished?.Invoke();
        _slotMahcineAnim.Play("SlotMachineOutro");
    }

    public void OnOutroEnd()
    {
        _rootPanel.HideSlotMachine();
    }
}