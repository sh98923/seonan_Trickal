using UnityEngine;
using UnityEngine.EventSystems;

public class CollectionPanelClose : MonoBehaviour, IPointerClickHandler
{
    public GameObject contentBox;  // DetailPanel 안의 ContentBox
    public GameObject detailPanel; // DetailPanel 전체

    public void OnPointerClick(PointerEventData eventData)
    {
        // 클릭한 곳이 ContentBox 안이 아니면 패널 닫기
        if (!RectTransformUtility.RectangleContainsScreenPoint(
                contentBox.GetComponent<RectTransform>(),
                eventData.position,
                eventData.pressEventCamera))
        {
            detailPanel.SetActive(false);
        }
    }
}
