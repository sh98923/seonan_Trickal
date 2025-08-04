using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageBtnPanel : MonoBehaviour
{
    private GameObject _stageInfo;

    private int _stageCount = 0;

    private void Awake()
    {
        _stageInfo = transform.parent.Find("StageInfoPanel").gameObject;
    }

    private void Start()
    {
        _stageCount = StageManager.Instance.StageCount;

        GameObject stageObj = Resources.Load<GameObject>("Prefabs/UI/StageBtn");

        CreateStageButtons(stageObj);
    }

    private void CreateStageButtons(GameObject stagePrefab)
    {
        float totalSpacing = Screen.width * 0.6f;
        float spacing = totalSpacing / (_stageCount - 1);
        float startPosX = (Screen.width - totalSpacing) / 2.0f;

        for (int i = 0; i < _stageCount; i++)
        {
            int stageNumber = i + 1;
            GameObject obj = Instantiate(stagePrefab, transform);

            float x = startPosX + spacing * i;
            Vector3 pos = new Vector3(x, transform.position.y, transform.position.z);
            obj.transform.position = pos;

            SetupStageButton(obj, stageNumber);
        }
    }

    private void SetupStageButton(GameObject buttonObj, int stageNumber)
    {
        Button btn = buttonObj.GetComponent<Button>();
        Image btnImage = buttonObj.GetComponent<Image>();
        TextMeshProUGUI txt = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

        txt.text = $"Stage {stageNumber}";
        
        bool isUnlocked = StageManager.Instance.IsStageUnlocked(stageNumber);

        if (isUnlocked)
        {
            btnImage.color = Color.white; // 버튼 배경도 흰색
        }
        else
        {
            btnImage.color = Color.gray; // 회색 (RGB: 128,128,128)
        }

        btn.onClick.AddListener(() => OnClickStage(stageNumber));
    }

    private void OnClickStage(int stageNum)
    {
        bool isUnlocked = StageManager.Instance.IsStageUnlocked(stageNum);

        if(isUnlocked)
        {
            int stageKey = StageManager.Instance.GetStageStartKey(stageNum  );
            GameManager.Instance.SetStageKey(stageKey);

            _stageInfo.SetActive(true);
        }
        else
        {
            print("이전 스테이지 깨고 오쇼 ㅋ");
        }
    }
}