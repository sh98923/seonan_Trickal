using UnityEngine;

public class CollectionManager : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform content;
    public GameObject detailPanel;

    void Start()
    {
        CreateSlots();
    }

    private void CreateSlots()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);

        // CSV 로드 데이터 사용
        for (int i = 0; i < CollectionDataManager.Instance.CollectionCount; i++)
        {
            int key = CollectionDataManager.Instance.CollectionStartKey + i;
            CollectionData data = CollectionDataManager.Instance.GetCollectionData(key);

            GameObject slot = Instantiate(slotPrefab, content);

            // 슬롯 클릭 → 디테일 패널 오픈
            slot.GetComponent<CollectionSlotPressEffect>().detailPanel = detailPanel;

            // 슬롯 UI 반영
            Sprite sprite = Resources.Load<Sprite>(data.CharacterSpritePath);
            slot.GetComponent<CollectionSlotDisplay>().SetData(sprite, data.KrName);

            // Detail Panel에서 정보를 다시 찾기 위해 key 저장
            slot.AddComponent<CollectionSlotDataHolder>().key = key;
        }
    }
}
