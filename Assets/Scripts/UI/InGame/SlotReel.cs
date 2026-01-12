using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotReel : MonoBehaviour
{
    private enum Slot
    {
        First = 1,
        Second = 2,
        Third = 3,
        Fourth = 4
    }

    private RectTransform[] _slots;
    private RectTransform _reelTransform;

    private Sprite _playerSprite;
    private List<Sprite> _rollingSprites = new List<Sprite>();

    private float _speed = 500.0f;
    private float _slotHeight;

    private bool _rolling = false;

    private void Awake()
    {
        _reelTransform = GetComponent<RectTransform>();
        _slots = GetComponentsInChildren<RectTransform>();

        _slotHeight = _reelTransform.anchoredPosition.y;
    }

    private IEnumerator RollCoroutine()
    {
        while (_rolling)
        {
            _reelTransform.anchoredPosition +=
                Vector2.down * _speed * Time.deltaTime;

            // 전체 슬롯 높이만큼 내려가면 위로 되돌림
            if (_reelTransform.anchoredPosition.y <= 0.0f)
            {
                _reelTransform.anchoredPosition +=
                    Vector2.up * _slotHeight;
            }

            yield return null;
        }
    }

    private void BuildSlots()
    {
        // 1, 4번 = 당첨 캐릭
        SetSlotImage(Slot.First, _playerSprite);
        SetSlotImage(Slot.Fourth, _playerSprite);

        // 2, 3번 = 연출용 랜덤
        SetSlotImage(Slot.Second, GetRandomPlayer());
        SetSlotImage(Slot.Third, GetRandomPlayer());
    }

    private void SetSlotImage(Slot index, Sprite slotSprite)
    {
        _slots[(int)index].GetComponent<Image>().sprite = slotSprite;
    }

    private Sprite GetRandomPlayer()
    {
        int index = Random.Range(0, _rollingSprites.Count);
        Sprite rollingSprite = _rollingSprites[index];

        _rollingSprites.RemoveAt(index);

        return rollingSprite;
    }

    public void SetResultPlayer(Sprite resultSprite, List<PlayerData> pool)
    {
        _playerSprite = resultSprite;

        foreach(PlayerData data in pool)
        {
            Sprite sprite = PlayerManager.Instance.GetPlayerSlotMachineSprite(data.Key);

            _rollingSprites.Add(sprite);
        }

        BuildSlots();
    }

    public void StartRoll()
    {
        _rolling = true;
        StartCoroutine(RollCoroutine());
    }
}
