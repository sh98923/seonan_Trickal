using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    private static DamageTextManager _instance;
    public static DamageTextManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("DamageTextManager가 null임");
            }

            return _instance;
        }
    }

    private GameObject _normalHitPrefab;
    private GameObject _fireTextPrefab;
    private GameObject _posionTextPrefab;

    private const int _normalHitSize = 100;
    private const int _dotHitSize = 100;

    private void Awake()
    {
        // 싱글톤 인스턴스가 이미 있으면 자기 자신을 파괴
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        // 리소스 로딩
        _normalHitPrefab = Resources.Load<GameObject>("Prefabs/UI/DamageText/NormalHitText");
        _fireTextPrefab = Resources.Load<GameObject>("Prefabs/UI/DamageText/FireHitText");
        _posionTextPrefab = Resources.Load<GameObject>("Prefabs/UI/DamageText/PosionHitText");
    }

    private void Start()
    {
        // PoolingManager에 텍스트 풀 추가
        PoolingManager.Instance.Add("NormalHitText", _normalHitSize, _normalHitPrefab, transform);
        PoolingManager.Instance.Add("FireHitText", _dotHitSize, _fireTextPrefab, transform);
        PoolingManager.Instance.Add("PosionHitText", _dotHitSize, _posionTextPrefab, transform);
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
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
