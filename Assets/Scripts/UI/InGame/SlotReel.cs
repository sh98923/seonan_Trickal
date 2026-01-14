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

    private float _speed = 1500.0f;
    private float _slotHeight = 0.0f;
    private float _totalHeight = 0.0f;

    private int _slotCount = 0;
    private int _resultSlotIndex = -1;
    private int[] _resultSlotIndexs = { 2, 0, 1 };

    private bool _rolling = false;

    private void Awake()
    {
        _reelTransform = GetComponent<RectTransform>();
        // 각 릴당 슬롯 개수
        _slotCount = transform.childCount;

        _slots = new RectTransform[_slotCount];

        for(int i = 0; i< _slotCount; i++)
        {
            Transform child = transform.GetChild(i);
            _slots[i] = child.GetComponent<RectTransform>();
        }

        // 한개의 슬롯 높이
        _slotHeight = _slots[0].rect.height * transform.localScale.y;
        _totalHeight = _reelTransform.anchoredPosition.y;
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
                    Vector2.up * _totalHeight;
            }

            yield return null;
        }

        // 멈출 때 정확히 맞춰줌
        float targetY = _resultSlotIndex * _slotHeight;
        Vector2 pos = _reelTransform.anchoredPosition;
        pos.y = targetY;
        _reelTransform.anchoredPosition = pos;
    }

    private void BuildSlots()
    {
        // 당첨 캐릭터의 인덱스가 처음 또는 마지막이면 true
        bool isResultAtEdge =
        (_resultSlotIndex == 0 || _resultSlotIndex == _slots.Length - 1);

        Sprite edgeSprite = isResultAtEdge
            ? _playerSprite          // 당첨이면
            : GetRandomPlayer();     // 아니면 랜덤

        for (int i = 0; i < _slots.Length; i++)
        {
            // 처음 또는 마지막 슬롯
            if (i == 0 || i == _slots.Length - 1)
            {
                SetSlotImage(i, edgeSprite);
                continue;
            }

            // 당첨 슬롯
            if (i == _resultSlotIndex)
            {
                SetSlotImage(i, _playerSprite);
                continue;
            }

            // 나머지 랜덤
            SetSlotImage(i, GetRandomPlayer());
        }
    }

    private void SetSlotImage(int index, Sprite slotSprite)
    {
        _slots[index].GetComponent<Image>().sprite = slotSprite;
    }

    private Sprite GetRandomPlayer()
    {
        int index = Random.Range(0, _rollingSprites.Count);
        Sprite rollingSprite = _rollingSprites[index];

        _rollingSprites.RemoveAt(index);

        return rollingSprite;
    }

    public void SetResultPlayer(int index, Sprite resultSprite, List<PlayerData> pool)
    {
        _playerSprite = resultSprite;
        _resultSlotIndex = _resultSlotIndexs[index];

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

    public void StopRoll()
    {
        _rolling = false;
    }
}
