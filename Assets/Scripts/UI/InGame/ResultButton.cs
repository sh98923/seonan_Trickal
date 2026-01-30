using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultButton : MonoBehaviour
{
    private enum ResultBtn
    {
        Exit,
        Retry,
        Next
    }

    private Image _nextBtnImage;
    private Button[] _resultBtns;
    private TextMeshProUGUI[] _resultTexts;
    private LayoutElement _nextBtnLayout;

    private void Awake()
    {
        _resultBtns = new Button[3];
        _resultTexts = new TextMeshProUGUI[3];

        _resultBtns = GetComponentsInChildren<Button>();
        _resultTexts = GetComponentsInChildren<TextMeshProUGUI>();

        BtnTextInit();
        BtnEventInit();

        GameObject nextBtnLayOut = _resultBtns[(int)ResultBtn.Next].gameObject;
        _nextBtnImage = nextBtnLayOut.GetComponent<Image>();
        _nextBtnLayout = nextBtnLayOut.GetComponent<LayoutElement>();
    }

    private void BtnTextInit()
    {
        _resultTexts[(int)ResultBtn.Exit].text = "나가기";
        _resultTexts[(int)ResultBtn.Retry].text = "다시하기";
        _resultTexts[(int)ResultBtn.Next].text = "다음 스테이지";
    }

    private void BtnEventInit()
    {
        _resultBtns[(int)ResultBtn.Exit].onClick.AddListener(OnClickExit);
        _resultBtns[(int)ResultBtn.Retry].onClick.AddListener(OnClickRetry);
        _resultBtns[(int)ResultBtn.Next].onClick.AddListener(OnClickNext);
    }

    private void OnClickExit()
    {
        SceneManager.LoadScene("StageSelectScene");
    }

    private void OnClickRetry()
    {
        // 다시 시작
    }

    private void OnClickNext()
    {
        // 다음 스테이지 클릭
        GameManager.Instance.NextStageClicked();
    }

    private void SetNextButton(bool isShow)
    {
        _nextBtnLayout.ignoreLayout = !isShow;

        // 시각 + 입력 제어
        Button nextBtn = _resultBtns[(int)ResultBtn.Next];
        TextMeshProUGUI nextBtnText = _resultTexts[(int)ResultBtn.Next];

        _nextBtnImage.enabled = isShow;
        nextBtnText.enabled = isShow;
        nextBtn.interactable = isShow;
    }

    private IEnumerator RebuildLayoutNextFrame()
    {
        yield return null; // 한 프레임 기다림
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }

    public void ShowVictory()
    {
        SetNextButton(true);
    }

    public void ShowDefeat()
    {
        SetNextButton(false); 
        StartCoroutine(RebuildLayoutNextFrame());
    }
}