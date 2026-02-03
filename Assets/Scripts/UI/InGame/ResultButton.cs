using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

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
    private HorizontalLayoutGroup _horizontalLayout;

    private const int _paddingWithNextButton = -335;
    private const int _paddingWithoutNextButton = -230;

    private void Awake()
    {
        _resultBtns = new Button[3];
        _resultTexts = new TextMeshProUGUI[3];

        _resultBtns = GetComponentsInChildren<Button>();
        _resultTexts = GetComponentsInChildren<TextMeshProUGUI>();

        _horizontalLayout = GetComponent<HorizontalLayoutGroup>();

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
        GameManager.Instance.ExitStage(StageExitType.Exit);
    }

    private void OnClickRetry()
    {
        GameManager.Instance.ExitStage(StageExitType.Retry);
    }

    private void OnClickNext()
    {
        GameManager.Instance.ExitStage(StageExitType.Next);
    }

    public void ShowVictory()
    {
        // 현재 열린 스테이지 수와 총 스테이지 수를 비교
        if (StageManager.Instance.StageMaxCount > StageManager.Instance.UnlockedStageCount)
        {
            _horizontalLayout.padding.left = _paddingWithNextButton;
        }
        else
        {
            ApplyLayoutWithoutNextButton();
        }
    }

    public void ShowDefeat()
    {
        ApplyLayoutWithoutNextButton();
    }

    private void ApplyLayoutWithoutNextButton()
    {
        _horizontalLayout.padding.left = _paddingWithoutNextButton;
        _resultBtns[(int)ResultBtn.Next].gameObject.SetActive(false);
    }
}