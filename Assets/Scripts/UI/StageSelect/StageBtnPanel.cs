/*using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageBtnPanel : MonoBehaviour
{
    private enum BtnImageUI
    {
        StageBtn = 0,
        StageOutLine = 1, 
        StageLock = 2
    }

    private GameObject _stageInfo;

    private int _stageCount = 0;
    private int _latestStage = 0;
    private int _stageBtnStartKey = 0;

    private void Awake()
    {
        _stageInfo = transform.parent.Find("StageInfoPanel").gameObject;
    }

    private void Start()
    {
        _stageCount = StageManager.Instance.StageCount;
        _stageBtnStartKey = StageBtnPosManager.Instance.StageBtnStartKey;
        _latestStage = _stageBtnStartKey;

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
            //print("이전 스테이지 깨고 오쇼 ㅋ");
        }
    }
}*/

using System.Collections.Generic;
using UnityEngine;

public class StageBtnPanel : MonoBehaviour
{
    private GameObject _stageInfo;

    private int _stageCount;
    private int _stageBtnStartKey;

    private readonly List<StageBtn> _stageButtons = new List<StageBtn>();

    private void Awake()
    {
        _stageInfo = transform.parent.Find("StageInfoPanel").gameObject;
    }

    private void Start()
    {
        _stageCount = StageManager.Instance.StageCount;
        _stageBtnStartKey = StageBtnPosManager.Instance.StageBtnStartKey;

        GameObject stagePrefab = Resources.Load<GameObject>("Prefabs/UI/StageBtn");
        CreateStageButtons(stagePrefab);

        SetLatestStageOutline();
    }

    private void CreateStageButtons(GameObject stagePrefab)
    {
        for (int i = 0; i < _stageCount; i++)
        {
            int stageKey = _stageBtnStartKey + i;
            Vector3 pos = StageBtnPosManager.Instance.GetStageBtnPos(stageKey);

            GameObject obj = Instantiate(stagePrefab, transform);
            obj.transform.position = pos;

            StageBtn stageBtn = obj.GetComponent<StageBtn>();
            stageBtn.Init(stageKey, OnClickStage);

            _stageButtons.Add(stageBtn);
        }
    }

    private void OnClickStage(int stageKey)
    {
        if (!StageManager.Instance.IsStageUnlocked(stageKey))
        { 
            return; 
        }

        // 외곽선 갱신
        foreach (StageBtn btn in _stageButtons)
        {
            btn.SetOutLineActive(btn.StageKey == stageKey);
        }

        int startKey = StageManager.Instance.GetStageStartKey(stageKey);
        GameManager.Instance.SetStageKey(startKey);

        _stageInfo.SetActive(true);
    }

    private void SetLatestStageOutline()
    {
        int latestStage = StageManager.Instance.HighestStageOutLineOn();

        foreach (StageBtn btn in _stageButtons)
        {
            btn.SetOutLineActive(btn.StageKey == latestStage);
        }
    }
}
