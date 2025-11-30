using UnityEngine;

public class SkillEffectController : MonoBehaviour
{
    private Character _target;
    private GameObject _effectObj;
    private SpriteRenderer _renderer;

    public void Initialize(Character target)
    {
        _target = target;
        _effectObj = transform.Find("SkillEffect").gameObject;
        _renderer = _effectObj.GetComponent<SpriteRenderer>();
    }

    public void Play(bool isFront)
    {
        if (_renderer == null) return;

        int sortNum = isFront ? _target.SortingIndex + 1 : -101;
        _renderer.sortingOrder = sortNum;
        _effectObj.SetActive(true);
    }

    public void Stop()
    {
        if (_effectObj == null) return;

        _effectObj.SetActive(false);
    }
}
