using UnityEngine;
using UnityEngine.UI;

public class StageSelectSceneButton : MonoBehaviour
{
    private Button _stageSelectBtn;

    private void Awake()
    {
        _stageSelectBtn = GetComponent<Button>();

        _stageSelectBtn.onClick.AddListener(GoToStageSelectScene);
    }

    private void GoToStageSelectScene()
    {
        SceneChange.Instance.TransitionToScene(SceneName.StageSelectScene);
    }
}