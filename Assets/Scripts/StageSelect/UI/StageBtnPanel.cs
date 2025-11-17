using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageBtnPanel : MonoBehaviour
{
    private enum BtnImageUI
    {
        StageBtn, StageLock
    }

    private GameObject _stageInfo;

    private int _stageCount = 0;
    private int _stageBtnStartKey;

    private void Awake()
    {
        _stageInfo = transform.parent.Find("StageInfoPanel").gameObject;
    }

    private void Start()
    {
        _stageCount = StageManager.Instance.StageCount;
        _stageBtnStartKey = StageBtnPosManager.Instance.StageBtnStartKey;

        GameObject stageObj = Resources.Load<GameObject>("Prefabs/UI/StageBtn");

        CreateStageButtons(stageObj);
    }

    private void CreateStageButtons(GameObject stagePrefab)
    {
        for (int i = 0; i < _stageCount; i++)
        {
            int stageKey = _stageBtnStartKey + i;
            Vector3 pos = StageBtnPosManager.Instance.GetStageBtnPos(stageKey);
            
            GameObject obj = Instantiate(stagePrefab, transform);

            obj.transform.position = pos;

            SetupStageButton(obj, stageKey);
        }
    }

    private void SetupStageButton(GameObject buttonObj, int stageNumber)
    {
        int stage = StageBtnPosManager.Instance.GetStageKey(stageNumber);
        Button btn = buttonObj.GetComponent<Button>();
        Image[] btnImage = buttonObj.GetComponentsInChildren<Image>();
        TextMeshProUGUI txt = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        Animator animator = buttonObj.GetComponent<Animator>();

        txt.text = $"Stage {stage}";
        
        bool isUnlocked = StageManager.Instance.IsStageUnlocked(stageNumber);

        if (isUnlocked)
        {
            animator.enabled = true;

            btnImage[(int)BtnImageUI.StageBtn].color = Color.white; // 버튼 배경 흰색
            btnImage[(int)BtnImageUI.StageLock].gameObject.SetActive(false);
        }
        else
        {
            animator.enabled = false;

            btnImage[(int)BtnImageUI.StageBtn].color = Color.gray; // 버튼 배경 회색
        }

        btn.onClick.AddListener(() => OnClickStage(stageNumber));
    }

    private void OnClickStage(int stageNum)
    {
        bool isUnlocked = StageManager.Instance.IsStageUnlocked(stageNum);

        if(isUnlocked)
        {
            int stageKey = StageManager.Instance.GetStageStartKey(stageNum);
            GameManager.Instance.SetStageKey(stageKey);

            _stageInfo.SetActive(true);
        }
        else
        {
            print("이전 스테이지 깨고 오쇼 ㅋ");
        }
    }
}