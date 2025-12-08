using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageInfoPanel : MonoBehaviour
{
    private enum StageUI
    {
        DeckSetUpBtn = 12, CancelBtn = 14
    }

    private StageMapBG _mapBG;
    private GameObject _deckPanel;
    private GameObject _stageBtn;
    private Transform[] _stageChildren; 
    private Transform _monsterPreviewRoot;

    private void Awake()
    {
        _stageChildren = GetComponentsInChildren<Transform>();

        _mapBG = GameObject.Find("BG").GetComponent<StageMapBG>();
        _monsterPreviewRoot = GameObject.Find("MonsterPreivew").transform;
        
        _deckPanel = transform.parent.Find("DeckPanel").gameObject;
        _stageBtn = transform.parent.Find("StageBtnPanel").gameObject;

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ShowMonsterInfo();

        _mapBG.OnSetMapBG += HandleMapBGSetting;
    }

    private void OnDisable()
    {
        RemovePreview();

        _mapBG.OnSetMapBG -= HandleMapBGSetting;
    }

    private void Start()
    {
        SetBtnEvent(StageUI.DeckSetUpBtn, OnClickDeckSetup);
        SetBtnEvent(StageUI.CancelBtn, OnClickCancel);
    }

    private void OnClickDeckSetup()
    {
        _mapBG.gameObject.SetActive(true);
        _mapBG.SetMapBG();
        _mapBG.SetMapBGEvent();

        _deckPanel.SetActive(true);
        _stageBtn.SetActive(false);
        gameObject.SetActive(false);
    }

    private void OnClickCancel()
    {
        gameObject.SetActive(false);
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

        float spacingX = 3.5f;

        int count = stageMonsterKeys.Count;

        // 전체 너비 계산
        float totalWidth = (count - 1) * spacingX;

        // 시작 위치: 가운데 기준에서 왼쪽으로 totalWidth/2 만큼 이동
        Vector3 startPos = new Vector3(-totalWidth * 0.5f, 0.5f, 0.0f);

        int index = 0;
        foreach (int monsterKey in stageMonsterKeys)
        {
            string prefabPath = MonsterManager.Instance.GetMonsterPrefab(monsterKey);

            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            GameObject obj = Instantiate(prefab, _monsterPreviewRoot);
            obj.transform.localScale *= 2.0f;

            Vector3 pos = startPos + new Vector3(index * spacingX, 0.0f, 0.0f);
            obj.transform.position = pos;

            index++;
        }
    }

    private void SetBtnEvent(StageUI element, Action func)
    {
        Button btn = _stageChildren[(int)element].GetComponent<Button>();
        btn.onClick.AddListener(() => func());
    }

    private void HandleMapBGSetting(MapBG bg)
    {
        bg.SetMapBG();
    }
}