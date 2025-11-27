using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CollectionSlotPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public GameObject detailPanel;       // 상세 UI 연결
    public float pressedScale = 0.9f;    // 눌렸을 때 크기
    public float animDuration = 0.15f;   // 애니메이션 지속시간

    private Vector3 originalScale;
    private Coroutine currentCoroutine;

    private bool isDragging = false;
    private Vector2 pointerDownPos;
    private const float dragThreshold = 10f; // 이 거리 이상 움직이면 드래그로 판단

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownPos = eventData.position;
        isDragging = false;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(ScaleTween(transform.localScale, originalScale * pressedScale, animDuration));
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Vector2.Distance(pointerDownPos, eventData.position) > dragThreshold)
            isDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ScaleTweenBounce(transform.localScale, originalScale, animDuration));

        // 드래그 상태라면 패널 열지 않음
        if (isDragging)
            return;

        // 실제 클릭일 때만 패널 열기
        if (detailPanel != null)
            detailPanel.SetActive(true);
    }

    private IEnumerator ScaleTween(Vector3 from, Vector3 to, float duration)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }
        transform.localScale = to;
    }

    private IEnumerator ScaleTweenBounce(Vector3 from, Vector3 to, float duration)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float scaleT = Mathf.SmoothStep(0f, 1f, t);
            transform.localScale = Vector3.Lerp(from, to * 1.05f, scaleT);
            yield return null;
        }
        transform.localScale = to;
    }
}
