using UnityEngine;
using UnityEngine.UI;

public class StageBtn : MonoBehaviour
{
    private Button _button;
    private Animator _animator;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _button.onClick.AddListener(OnClickStage);
    }

    private void OnClickStage()
    {
        _animator.SetTrigger("StageClicked");
    }
}
