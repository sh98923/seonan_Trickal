using System.Collections.Generic;
using UnityEngine;

public class CollectionManager : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform content;
    public GameObject detailPanel;

    // 정렬을 위해 key slot 매핑 저장
    private Dictionary<int, GameObject> slotDictionary = new Dictionary<int, GameObject>();

    void Start()
    {
        CreateSlots();
    }

    private void CreateSlots()
    {
        slotDictionary.Clear();

        foreach (Transform child in content)
            Destroy(child.gameObject);

        for (int i = 0; i < CollectionDataManager.Instance.CollectionCount; i++)
        {
            int key = CollectionDataManager.Instance.CollectionStartKey + i;
            CollectionData data = CollectionDataManager.Instance.GetCollectionData(key);

            GameObject slot = Instantiate(slotPrefab, content);

            slot.GetComponent<CollectionSlotPressEffect>().detailPanel = detailPanel;

            Sprite sprite = Resources.Load<Sprite>(data.CharacterSpritePath);
            slot.GetComponent<CollectionSlotDisplay>().SetData(sprite, data.KrName);

            slot.GetComponent<CollectionSlotDataHolder>().key = key;

            // 슬롯 저장
            slotDictionary.Add(key, slot);
        }
    }

    // 정렬 호출 함수 팝업 버튼에서 사용
    public void Sort(System.Comparison<CollectionData> comparison)
    {
        // 1) 데이터 키 목록 가져오기
        List<int> keys = new List<int>(slotDictionary.Keys);

        // 2) 데이터 기준 비교 후 정렬
        keys.Sort((a, b) => comparison(
            CollectionDataManager.Instance.GetCollectionData(a),
            CollectionDataManager.Instance.GetCollectionData(b)));

        // 3) 슬롯 순서 재배치
        for (int i = 0; i < keys.Count; i++)
        {
            GameObject slot = slotDictionary[keys[i]];
            slot.transform.SetSiblingIndex(i);
        }
    }
}
