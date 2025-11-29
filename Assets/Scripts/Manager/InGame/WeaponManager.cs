using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponManager : Singleton<WeaponManager>
{
    private List<GameObject> _firedProjectiles = new List<GameObject>();

    private void OnEnable()
    {
        BattleStateManager.Instance.OnEnteringReroll += InitFiredList; 
        BattleStateManager.Instance.OnEnteringReroll += DisableFiredProjectiles;
    }

    private void OnDisable()
    {
        BattleStateManager.Instance.OnEnteringReroll -= InitFiredList;
        BattleStateManager.Instance.OnEnteringReroll -= DisableFiredProjectiles;
    }

    private void OnDestroy()
    {
        if(BattleStateManager.Instance != null)
        { 
            BattleStateManager.Instance.OnEnteringReroll -= InitFiredList;
            BattleStateManager.Instance.OnEnteringReroll -= DisableFiredProjectiles;
        }
    }

    private void InitFiredList()
    {
        _firedProjectiles.Clear();
    }

    private void DisableFiredProjectiles()
    {
        foreach(GameObject projectile in _firedProjectiles)
        {
            projectile.SetActive(false);
        }
    }

    public void CreateWeapon()
    {
        int startKey = WeaponDataManager.Instance.WeaponStartKey;
        int weaponCount = WeaponDataManager.Instance.GetWeaponCount();

        for (int i = startKey; i < startKey + weaponCount; i++)
        {
            WeaponData data = WeaponDataManager.Instance.GetWeaponData(i);

            GameObject weaponPrefab = Resources.Load<GameObject>(data.PrefabPath);
            PoolingManager.Instance.Add(data.Name, data.PoolSize, weaponPrefab, transform);
        }
    }

    public void Fire(ProjectileData data)
    {
        GameObject projectile = PoolingManager.Instance.Pop(data.Key);
        _firedProjectiles.Add(projectile);

        // 발사 될 방향으로 스프라이트, 콜라이더 회전
        Vector3 scale = projectile.transform.localScale;
        scale.x = Mathf.Abs(scale.x); // 절댓값 먼저 적용

        if (data.IsFlipX)
        {
            scale.x = -scale.x;
        }

        projectile.transform.localScale = scale;

        // 스프라이트 적용
        projectile.GetComponent<SpriteRenderer>().sprite = data.Sprite;

        // 실제 발사
        projectile.GetComponent<Projectile>().Fire(data);
    }
}