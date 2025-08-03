using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    [SerializeField] private GameObject _damageTextPrefab;

    private void Start()
    {
        PoolingManager.Instance.Add("DamageText", 10, _damageTextPrefab, transform);
    }

}
