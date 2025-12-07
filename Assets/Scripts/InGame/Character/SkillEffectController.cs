using Unity.VisualScripting;
using UnityEngine;

public class SkillEffectController : MonoBehaviour
{
    private Character _target;
    private GameObject _effectObj;
    private SpriteRenderer _renderer;

    private void Awake()
    {
        _effectObj = transform.Find("SkillEffect").gameObject;
        _effectObj.SetActive(false);
    }

    public void Initialize(Character target)
    {
        _target = target;
        _renderer = _effectObj.GetComponent<SpriteRenderer>();
    }

    public void SetSortingPosition(bool isFront)
    {
        if (_renderer == null) return;

        int sortNum = isFront ? _target.SortingIndex + 1 : -101;
        _renderer.sortingOrder = sortNum;
    }

    public void OnSkillEffectActivate()
    {
        _effectObj.SetActive(true);
    }

    public void OnSkillEffectDeactivate()
    {
        if (_effectObj == null) return;

        _effectObj.SetActive(false);
    }

    public Vector2 GetPosition()
    {
        if (_effectObj == null)
            return Vector2.zero;

        return _effectObj.transform.localPosition;
    }

    public Vector2 GetSize()
    {
        if (_renderer == null)
            return Vector2.zero;

        Vector2 localSize = _renderer.transform.localScale;
        Vector2 finalSize = localSize * _renderer.size;

        return finalSize;
    }
}