using UnityEngine;

public class CharacterStatusBar : MonoBehaviour
{
    protected CharacterHp _hp;
    protected Transform _hpBar;

    protected void Awake()
    {
        _hp = GetComponent<CharacterHp>();

        SetBarScale(_hpBar, 1.0f);

        GameManager.Instance.SetActiveGameObjectInScenes(gameObject, SceneName.InGameScene);
    }

    protected void OnEnable()
    {
        _hp.OnHpZero += SetActiveBar;
        _hp.OnHpChanged += UpdateHpBar;
    }

    protected void OnDisable()
    {
        _hp.OnHpZero -= SetActiveBar;
        _hp.OnHpChanged -= UpdateHpBar;
    }

    public void SetActiveBar(bool active)
    {   
        gameObject.SetActive(active);
    }

    protected void SetBarScale(Transform bar, float ratio)
    {
        Vector3 scale = bar.localScale;
        scale.x = Mathf.Clamp01(ratio);
        bar.localScale = scale;
    }

    protected void UpdateHpBar(float cur, float max)
    {
        float ratio = cur / max;

        Vector3 scale = _hpBar.localScale;
        scale.x = ratio;
        _hpBar.localScale = scale;
    }
}