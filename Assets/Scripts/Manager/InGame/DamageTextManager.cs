using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    [SerializeField] private GameObject _damageTextPrefab;
    [SerializeField] private GameObject _dotTextPrefab;

    private void Start()
    {
        PoolingManager.Instance.Add("DamageText", 50, _damageTextPrefab, transform);
        PoolingManager.Instance.Add("DotText", 50, _dotTextPrefab, transform);
    }
}