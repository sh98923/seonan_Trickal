using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StagePanel : MonoBehaviour
{
    private Vector3[] stagePos = 
        { 
            new Vector3(-200.0f, 0.0f, 0.0f),
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(200.0f, 0.0f, 0.0f)
        };

    private int _stageIndex;

    private void Awake()
    {
        _stageIndex = StageManager.Instance.StageStartKey;
    }

    private void Start()
    {
        GameObject stageObj = Resources.Load<GameObject>("Prefabs/UI/StageBtn");

        for (int i = 0; i < stagePos.Length; i++)
        {
            int index = i;
            GameObject obj = Instantiate(stageObj, transform);
            obj.transform.position = transform.position + stagePos[i];
            obj.GetComponent<Button>().onClick.AddListener(() => OnClickStage(index));
            obj.GetComponentInChildren<TextMeshProUGUI>().text = "Stage " + (i + 1);
        }
    }

    private void OnClickStage(int index)
    {
        int stageKey = _stageIndex + (index * 5);
        GameManager.Instance.SetStageKey(stageKey);
        SceneManager.LoadScene("InGameScene");
    }
}
