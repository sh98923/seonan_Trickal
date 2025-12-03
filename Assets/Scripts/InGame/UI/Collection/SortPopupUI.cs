using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SortPopupUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private CollectionManager _collectionManager;

    [Header("Buttons (순서대로 넣기)")]
    [SerializeField] private RectTransform[] _sortButtons;

    [Header("Animation")]
    [SerializeField] private float _buttonDistance = 60f;
    [SerializeField] private float _buttonDuration = 0.16f;
    [SerializeField] private float _stagger = 0.05f;

    private bool _isOpen = false;
    private bool _animPlaying = false;

    // 버튼 원래 위치 저장
    private Vector2[] _originalPos;

    private void Awake()
    {
        _originalPos = new Vector2[_sortButtons.Length];
        for (int i = 0; i < _sortButtons.Length; i++)
        {
            _originalPos[i] = _sortButtons[i].anchoredPosition;
            _sortButtons[i].gameObject.SetActive(false);
        }

        _panel.SetActive(false);
    }

    public void Toggle()
    {
        // 애니메이션 중에는 입력 막기
        if (_animPlaying) return;

        if (!_isOpen)
            StartCoroutine(OpenAnim());
        else
            StartCoroutine(CloseAnim());
    }

    private IEnumerator OpenAnim()
    {
        _isOpen = true;
        _animPlaying = true;
        _panel.SetActive(true);

        for (int i = 0; i < _sortButtons.Length; i++)
        {
            var btn = _sortButtons[i];
            btn.gameObject.SetActive(true);

            CanvasGroup cg = btn.GetComponent<CanvasGroup>();
            if (cg == null) cg = btn.gameObject.AddComponent<CanvasGroup>();

            Vector2 startPos = _originalPos[i] - new Vector2(0, _buttonDistance);
            Vector2 endPos = _originalPos[i];

            cg.alpha = 0f;
            btn.anchoredPosition = startPos;

            float time = 0f;
            while (time < _buttonDuration)
            {
                time += Time.deltaTime;
                float t = time / _buttonDuration;
                btn.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
                cg.alpha = t;
                yield return null;
            }

            btn.anchoredPosition = endPos;
            cg.alpha = 1f;
            yield return new WaitForSeconds(_stagger);
        }

        _animPlaying = false;
    }

    private IEnumerator CloseAnim()
    {
        _isOpen = false;
        _animPlaying = true;

        for (int i = _sortButtons.Length - 1; i >= 0; i--)
        {
            var btn = _sortButtons[i];
            CanvasGroup cg = btn.GetComponent<CanvasGroup>();

            Vector2 startPos = _originalPos[i];
            Vector2 endPos = _originalPos[i] - new Vector2(0, _buttonDistance);

            float time = 0f;
            while (time < _buttonDuration)
            {
                time += Time.deltaTime;
                float t = time / _buttonDuration;
                btn.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
                cg.alpha = 1f - t;
                yield return null;
            }

            btn.gameObject.SetActive(false);
            btn.anchoredPosition = _originalPos[i];   // 위치 초기화
        }

        _panel.SetActive(false);
        _animPlaying = false;
    }

    // 정렬 옵션 
    public void SortByName()
    {
        _collectionManager.Sort((a, b) => a.KrName.CompareTo(b.KrName));
        StartCoroutine(CloseAnim());
    }

    public void SortByAtkHigh()
    {
        _collectionManager.Sort((a, b) => b.Atk.CompareTo(a.Atk));
        StartCoroutine(CloseAnim());
    }

    public void SortByAtkLow()
    {
        _collectionManager.Sort((a, b) => a.Atk.CompareTo(b.Atk));
        StartCoroutine(CloseAnim());
    }
}
