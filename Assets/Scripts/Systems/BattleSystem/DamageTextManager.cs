using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    private GameObject _normalHitPrefab;
    private GameObject _fireTextPrefab;
    private GameObject _posionTextPrefab;

    private const int _normalHitSize = 100;
    private const int _dotHitSize = 100;

    private void Awake()
    {
        _normalHitPrefab = Resources.Load<GameObject>("Prefabs/UI/DamageText/NormalHitText");
        _fireTextPrefab = Resources.Load<GameObject>("Prefabs/UI/DamageText/FireHitText");
        _posionTextPrefab = Resources.Load<GameObject>("Prefabs/UI/DamageText/PosionHitText");
    }

    private void Start()
    {
        PoolingManager.Instance.Add("NormalHitText", _normalHitSize, _normalHitPrefab, transform);
        PoolingManager.Instance.Add("FireHitText", _dotHitSize, _fireTextPrefab, transform);
        PoolingManager.Instance.Add("PosionHitText", _dotHitSize, _posionTextPrefab, transform);
    }

    public string GetHitText(HitType type = HitType.Normal)
    {
        switch (type)
        {
            case HitType.Fire:
                return "FireHitText";
            case HitType.Posion:
                return "PosionHitText";
            default:
                return "NormalHitText";
        }
    }
}