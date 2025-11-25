using UnityEngine;

public class CollectionManager : MonoBehaviour
{
    private GameObject _slotPrefab;  // 슬롯 프리팹
    private Transform _content;      // ScrollView Content
    private int _slotCount = 9;      // 자동 생성 개수

    private void Start()
    {
        ClearSlots();
        CreateSlots();
    }

    // 기존 슬롯 제거
    private void ClearSlots()
    {
        for (int i = _content.childCount - 1; i >= 0; i--)
        {
            Destroy(_content.GetChild(i).gameObject);
        }
    }

    // 신규 슬롯 생성
    private void CreateSlots()
    {
        for (int i = 0; i < _slotCount; i++)
        {
            GameObject slot = Instantiate(_slotPrefab, _content);

            // detailPanel 연결도 여기서 해줄 수 있음
            slot.GetComponent<CollectionSlotPressEffect>().detailPanel = // 패널 연결
                GameObject.Find("Collection Info Panel");
        }
    }
}
