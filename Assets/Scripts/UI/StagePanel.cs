using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StagePanel : MonoBehaviour
{
    private Vector3 stagePos = new Vector3(-200.0f, 0.0f, 0.0f);

    private readonly float _xSpacing = 200.0f;
    private readonly int _stageCount = 3;
    private int _stageIndex;

    private void Awake()
    {
        _stageIndex = StageManager.Instance.StageStartKey;
    }

    private void Start()
    {
        GameObject stageObj = Resources.Load<GameObject>("Prefabs/UI/StageBtn");

        for (int i = 0; i < _stageCount; i++)
        {
            int index = i;
            GameObject obj = Instantiate(stageObj, transform);

            Vector3 pos = transform.position + new Vector3(_xSpacing * i, 0f, 0f);
            obj.transform.position = pos;

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
