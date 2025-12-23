using UnityEngine;

public class CharacterStatusBar : MonoBehaviour
{
    protected Transform _hpBar;

    protected void Awake()
    {
        SetBarScale(_hpBar, 1.0f);
        GameManager.Instance.SetActiveGameObjectInScenes(gameObject, SceneName.InGameScene);
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