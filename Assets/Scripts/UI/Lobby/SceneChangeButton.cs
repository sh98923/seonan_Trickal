using UnityEngine;

public class SceneChangeButton : MonoBehaviour
{
    [SerializeField] private string _sceneName;

    public void ChangeScene()
    {
        SceneChange.Instance.TransitionToScene(_sceneName);
    }
}
