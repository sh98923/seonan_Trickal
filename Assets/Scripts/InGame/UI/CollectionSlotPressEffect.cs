using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CollectionSlotPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject detailPanel;       // 상세 UI 연결
    public float pressedScale = 0.9f;    // 눌렸을 때 크기
    public float animDuration = 0.15f;   // 애니메이션 지속시간

    private Vector3 originalScale;
    private Coroutine currentCoroutine;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 눌릴 때 Tween
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(ScaleTween(transform.localScale, originalScale * pressedScale, animDuration));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 떼면 원래 크기로 튀는 Tween
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ScaleTweenBounce(transform.localScale, originalScale, animDuration));

        // 상세 UI 열기 (Tween과 동시에 열리거나, 끝나고 열리게 조절 가능)
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
            // SmoothStep로 약간 튀는 느낌
            float scaleT = Mathf.SmoothStep(0f, 1f, t);
            transform.localScale = Vector3.Lerp(from, to * 1.05f, scaleT); // 살짝 크게 튀게
            yield return null;
        }
        transform.localScale = to;
    }
}
