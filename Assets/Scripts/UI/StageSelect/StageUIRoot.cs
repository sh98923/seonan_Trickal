using UnityEngine;

public class StageUIRoot : MonoBehaviour
{
    private enum StageUI
    {
        StageBtnPanel = 1,
        StageInfoPanel = 2,
        DeckPanel = 18
    }

    private MapBG _mapBG;
    private Transform[] _stageUIs;

    private void Awake()
    {
        _stageUIs = GetComponentsInChildren<Transform>();
        _mapBG = GameObject.Find("BG").GetComponent<MapBG>();
    }

    private void Start()
    {
        SetDeckPanelActive(false);
        SetStageInfoPanelActive(false);

        if (GameManager.Instance.OpenDeckPanel)
        {
            SetMapBG();
            OpenDeckPanel();
            GameManager.Instance.OnStageSelectSceneLoaded(); // 플래그 초기화
        }
    }

    private void OpenDeckPanel()
    {
        _stageUIs[(int)StageUI.DeckPanel].gameObject.SetActive(true);
        _stageUIs[(int)StageUI.StageBtnPanel].gameObject.SetActive(false);
        _stageUIs[(int)StageUI.StageInfoPanel].gameObject.SetActive(false);
    }

    public void SetStageBtnPanelActive(bool isActive)
    {
        _stageUIs[(int)StageUI.StageBtnPanel].gameObject.SetActive(isActive);
    }

    public void SetStageInfoPanelActive(bool isActive)
    {
        _stageUIs[(int)StageUI.StageInfoPanel].gameObject.SetActive(isActive);
    }

    public void SetDeckPanelActive(bool isActive)
    {
        _stageUIs[(int)StageUI.DeckPanel].gameObject.SetActive(isActive);
    }

    public void SetMapBG()
    {
        _mapBG.SetMapBG();
    }
}