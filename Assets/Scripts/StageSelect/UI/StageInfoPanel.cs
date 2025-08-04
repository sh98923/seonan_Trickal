using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageInfoPanel : MonoBehaviour
{
    private enum StageUI
    {
        DeckSetUpBtn = 1, CancelBtn = 3
    }

    private MapBG _mapBG;
    private GameObject _deckPanel;
    private GameObject _stageBtn;
    private Transform[] _stageChildren; 
    private Transform _monsterPreviewRoot;

    private void Awake()
    {
        _stageChildren = GetComponentsInChildren<Transform>();

        _mapBG = GameObject.Find("BG").GetComponent<MapBG>();
        _monsterPreviewRoot = GameObject.Find("MonsterPreivew").transform;
        
        _deckPanel = transform.parent.Find("DeckPanel").gameObject;
        _stageBtn = transform.parent.Find("StageBtnPanel").gameObject;

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ShowMonsterInfo();

        _mapBG.OnMapFound += HandleMapBGFound;
        _mapBG.OnSetMapBG += HandleMapBGSetting;
    }

    private void OnDisable()
    {
        RemovePreview();

        _mapBG.OnMapFound -= HandleMapBGFound;
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

    private void SetBtnEvent(StageUI element, Action func)
    {
        Button btn = _stageChildren[(int)element].GetComponent<Button>();
        btn.onClick.AddListener(() => func());
    }

    private void HandleMapBGFound(MapBG bg)
    {
        bg.InActiveBG();
    }

    private void HandleMapBGSetting(MapBG bg)
    {
        bg.SetMapBG();
    }
}
