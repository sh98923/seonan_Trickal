using UnityEngine;

public class CharacterStatusBar : MonoBehaviour
{
    protected CharacterHp _hp;
    protected Transform _hpBar;

    private void Awake()
    {
        GameManager.Instance.SetActiveGameObjectInScenes(gameObject, SceneName.InGameScene);

        _hp = GetComponentInChildren<CharacterHp>();
    }

    private void OnEnable()
    {
        if (_hp != null)
            _hp.OnHpZero += SetActiveBar;
    }

    private void OnDisable()
    {
        if (_hp != null)
            _hp.OnHpZero -= SetActiveBar;
    }

    public void SetActiveBar(bool active)
    {   
        gameObject.SetActive(active);
    }
}