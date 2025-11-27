using UnityEngine;

public class CollectionManager : MonoBehaviour
{
    public GameObject _slotPrefab;  // 슬롯 프리팹
    public Transform _content;      // ScrollView Content
    public GameObject detailPanel;
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
            var effect = slot.GetComponent<CollectionSlotPressEffect>();
            effect.detailPanel = detailPanel;
        }
    }
}
