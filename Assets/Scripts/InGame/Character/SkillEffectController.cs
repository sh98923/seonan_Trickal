using UnityEngine;

public class SkillEffectController : MonoBehaviour
{
    private GameObject _effectObj;
    private SpriteRenderer _renderer;
    private Character _owner;

    public void Initialize(Character owner)
    {
        _owner = owner;
        _effectObj = transform.Find("SkillEffect").gameObject;
        _renderer = _effectObj.GetComponent<SpriteRenderer>();
    }

    public void Play(bool isFront)
    {
        if (_renderer == null) return;
        int sortNum = isFront ? _owner.SortingIndex + 1 : -101;
        _renderer.sortingOrder = sortNum;
        _effectObj.SetActive(true);
    }

    public void Stop()
    {
        if (_effectObj == null) return;
        _effectObj.SetActive(false);
    }
}
