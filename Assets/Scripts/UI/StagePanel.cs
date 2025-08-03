using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StagePanel : MonoBehaviour
{
    private enum StageUI
    {
        StageInfoPanel = 1, StageStartBtn = 2, CancelBtn = 4
    }

    private Transform[] _stageChildren;
    private Transform _monsterPreviewRoot;
    private readonly int _stageCount = 3;

    private void Awake()
    {
        _monsterPreviewRoot = GameObject.Find("MonsterPreivew").transform;
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

        SetBtnEvent(StageUI.StageStartBtn, OnClickStart);
        SetBtnEvent(StageUI.CancelBtn, OnClickCancel);
    }

    private void OnClickStage(int index)
    {
        RemovePreview();

        int stageNumber = index + 1;
        int stageKey = StageManager.Instance.GetStageStartKey(stageNumber);
        GameManager.Instance.SetStageKey(stageKey);
        ShowMonsterInfo();
        SetStageInfoPanelActive(true);
    }

    private void OnClickStart()
    {
        SceneManager.LoadScene("InGameScene");
    }

    private void OnClickCancel()
    {
        RemovePreview();

        SetStageInfoPanelActive(false);
    }

    private void RemovePreview()
    {
        // 기존에 생성된 몬스터 미리보기 제거
        foreach (Transform child in _monsterPreviewRoot)
        {
            Destroy(child.gameObject);
        }
    }

    private void ShowMonsterInfo()
    {
        int waveCnt = GameManager.Instance.WaveCount;
        HashSet<int> stageMonsterKeys = WaveManager.Instance.GetStageMonster(waveCnt);

        // 시작 위치
        Vector3 startPos = new Vector3(-3.75f, 0.5f, 0.0f); // 왼쪽 상단 쯤
        float spacingX = 3.0f; // 가로 간격
        float spacingY = 3.0f; // 세로 간격
        int columnCount = 3;   // 가로로 몇 마리씩 표시할지

        int index = 0;
        foreach (int monsterKey in stageMonsterKeys)
        {
            MonsterData monsterData = MonsterManager.Instance.GetMonsterData(monsterKey);
            CharacterData characterData = CharacterManager.Instance.GetCharacterData(monsterData.CharacterKey);

            string prefabPath = characterData.PrefabPath;

            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            GameObject obj = Instantiate(prefab, _monsterPreviewRoot);

            int row = index / columnCount;
            int col = index % columnCount;

            Vector3 pos = startPos + new Vector3(col * spacingX, -row * spacingY, 0f);
            obj.transform.localScale *= 2.0f;
            obj.transform.position = pos;

            index++;
        }
    }

    private void SetStageInfoPanelActive(bool active)
    {
        _stageChildren[(int)StageUI.StageInfoPanel].gameObject.SetActive(active);
    }

    private void SetBtnEvent(StageUI element, Action func)
    {
        Button btn = _stageChildren[(int)element].GetComponent<Button>();
        btn.onClick.AddListener(() => func());
    }
}