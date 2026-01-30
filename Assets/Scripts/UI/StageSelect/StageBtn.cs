/*using UnityEngine;
using UnityEngine.UI;

public class StageBtn : MonoBehaviour
{
    private Button _button;
    private Animator _animator;
    private OutLineLight _outline;

    private const int _outLineImageIndex = 0;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _animator = GetComponent<Animator>();
        _outline = GetComponent<OutLineLight>();
    }

    private void Start()
    {
        // 이걸 이제 이 씬 들어오면 최근 열린 스테이지에 외곽선이 활성화 되고
        // 이후 플레이어가 스테이지를 클릭하면 그쪽 외곽선만 활성화
        // 단, 잠겨있는 경우 활성화 안댐

        Transform outlineTrasform = transform.GetChild(_outLineImageIndex);
       
        _outline.InitImage(outlineTrasform);
        _outline.OutLineActive(false);
        _button.onClick.AddListener(OnClickStage);
    }

    private void OnClickStage()
    {
        _animator.SetTrigger("StageClicked");
    }

    public void SetOutLineActive(bool isActive)
    {
        _outline.OutLineActive(isActive);
    }
}*/

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageBtn : MonoBehaviour
{
    private enum BtnImageUI
    {
        StageBtn = 0,
        StageOutLine = 1,
        StageLock = 2
    }

    private int _stageKey = -1;
    public int StageKey
    {
        get { return _stageKey; }
    }

    private Button _button;
    private Animator _animator;
    private OutLineLight _outline;
    private Image[] _images;
    private TextMeshProUGUI _stageText;

    private const int outLineImageIndex = 0;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _animator = GetComponent<Animator>();
        _outline = GetComponent<OutLineLight>();
        _images = GetComponentsInChildren<Image>(true);
        _stageText = GetComponentInChildren<TextMeshProUGUI>(true);
    }

    public void Init(int stageKey, System.Action<int> onClick)
    {
        _stageKey = stageKey;

        bool isUnlocked = StageManager.Instance.IsStageUnlocked(stageKey);

        // 텍스트 세팅
        int stageNumber = StageBtnPosManager.Instance.GetStageKey(stageKey);
        _stageText.text = $"Stage {stageNumber}";

        // 외곽선 초기화
        Transform outlineTransform = transform.GetChild(outLineImageIndex);
        _outline.InitImage(outlineTransform);
        _outline.OutLineActive(false);

        // 잠금 / 해금 UI 처리
        ApplyLockState(isUnlocked);

        _button.onClick.AddListener(() =>
        {
            if (!isUnlocked)
                return;

            _animator.SetTrigger("StageClicked");
            onClick?.Invoke(stageKey);
        });
    }

    private void ApplyLockState(bool isUnlocked)
    {
        if (isUnlocked)
        {
            _animator.enabled = true;

            _images[(int)BtnImageUI.StageBtn].color = Color.white;
            _images[(int)BtnImageUI.StageLock].gameObject.SetActive(false);
        }
        else
        {
            _animator.enabled = false;

            _images[(int)BtnImageUI.StageBtn].color = Color.gray;
            _images[(int)BtnImageUI.StageLock].gameObject.SetActive(true);
        }
    }

    public void SetOutLineActive(bool isActive)
    {
        _outline.OutLineActive(isActive);
    }
}
