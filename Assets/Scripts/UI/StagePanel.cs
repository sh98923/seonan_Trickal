using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StagePanel : MonoBehaviour
{
    private enum StageElement
    {
        StageInfoPanel = 1,
        StageStartBtn = 2,
        CancelBtn = 3
    }

    private Transform[] _stageChildren;
    private readonly int _stageCount = 3;

    private void Awake()
    {
        _stageChildren = GetComponentsInChildren<Transform>();
        SetStageInfoPanelActive(false);
    }

    private void Start()
    {
        GameObject stageObj = Resources.Load<GameObject>("Prefabs/UI/StageBtn");

        float totalSpacing = Screen.width * 0.6f;
        float spacing = totalSpacing / (_stageCount - 1);
        float startPosX = (Screen.width - totalSpacing) / 2.0f;

        for (int i = 0; i < _stageCount; i++)
        {
            int index = i;
            GameObject obj = Instantiate(stageObj, transform);
            obj.transform.SetSiblingIndex(i);

            float x = startPosX + spacing * i;
            Vector3 pos = new Vector3(x, transform.position.y, transform.position.z);
            obj.transform.position = pos;

            obj.GetComponent<Button>().onClick.AddListener(() => OnClickStage(index));
            obj.GetComponentInChildren<TextMeshProUGUI>().text = $"Stage {i + 1}";
        }

        SetBtnEvent(StageElement.StageStartBtn, OnClickStart);
        SetBtnEvent(StageElement.CancelBtn, OnClickCancel);
    }

    private void OnClickStage(int index)
    {
        int stageNumber = index + 1;
        int stageKey = StageManager.Instance.GetStageStartKey(stageNumber);
        GameManager.Instance.SetStageKey(stageKey);
        SetStageInfoPanelActive(true);
    }

    private void OnClickStart()
    {
        SceneManager.LoadScene("InGameScene");
    }

    private void OnClickCancel()
    {
        SetStageInfoPanelActive(false);
    }

    private void SetStageInfoPanelActive(bool active)
    {
        _stageChildren[(int)StageElement.StageInfoPanel].gameObject.SetActive(active);
    }

    private void SetBtnEvent(StageElement element, Action func)
    {
        Button btn = _stageChildren[(int)element].GetComponent<Button>();
        btn.onClick.AddListener(() => func());
    }
}
