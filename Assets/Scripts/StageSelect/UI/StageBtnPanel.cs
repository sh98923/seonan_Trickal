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
        //Vector3[] positions = new Vector3[3];
        //
        //positions[0] = new Vector3(Screen.width * 0.68f, Screen.height * 0.2f, 0); // 하단 오른쪽
        //positions[1] = new Vector3(Screen.width * 0.33f, Screen.height * 0.5f, 0); // 중단 왼쪽
        //positions[2] = new Vector3(Screen.width * 0.68f, Screen.height * 0.78f, 0); // 상단 오른쪽
        //
        //for (int i = 0; i < _stageCount; i++)
        //{
        //    int stageNumber = i + 1;
        //    GameObject obj = Instantiate(stagePrefab, transform);
        //
        //    int index = i % 3;
        //    obj.transform.position = positions[index];
        //
        //    SetupStageButton(obj, stageNumber);
        //}
        for (int i = 0; i < _stageCount; i++)
        {
            int stageNumber = i + 1;
            GameObject obj = Instantiate(stagePrefab, transform);

            int index = i % 3;

            float x = (index == 1) ? Screen.width * 0.33f : Screen.width * 0.68f;
            float y = (index == 0) ? Screen.height * 0.2f :
                      (index == 1) ? Screen.height * 0.5f : Screen.height * 0.78f;

            obj.transform.position = new Vector3(x, y, 0f);

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