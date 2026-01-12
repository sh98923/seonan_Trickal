using UnityEngine;
using System.Collections.Generic;

public class SlotMachineController : MonoBehaviour
{
    private SlotReel[] _slots;

    private List<PlayerData> _playerDatas;

    private void Awake()
    {
        _slots = GetComponentsInChildren<SlotReel>();

        _playerDatas = GameManager.Instance.SpawnablePlayerDatas;
    }

    private void Start()
    {
        AssignResults();
        StartSlot();
    }

    private void AssignResults()
    {
        List<PlayerData> pool = new List<PlayerData>(_playerDatas);

        for (int i = 0; i < _slots.Length; i++)
        {
            int rand = Random.Range(0, pool.Count);
            Sprite sprite = PlayerManager.Instance.GetPlayerSlotMachineSprite(pool[rand].Key);

            pool.RemoveAt(rand); // 중복 방지

            _slots[i].SetResultPlayer(sprite, pool);
        }
    }

    public void StartSlot()
    {
        foreach (SlotReel slot in _slots)
        {
            slot.StartRoll();
        }
    }
}