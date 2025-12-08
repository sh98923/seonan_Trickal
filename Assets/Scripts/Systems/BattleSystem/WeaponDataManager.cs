using System.Collections.Generic;
using UnityEngine;

public struct WeaponData
{
    public int Key;
    public string Name;
    public string PrefabPath;
    public int PoolSize;
}

public class WeaponDataManager : Singleton<WeaponDataManager>
{
    private Dictionary<int, WeaponData> _weaponDatas = new Dictionary<int, WeaponData>();

    private int _weaponStartKey = 5000;
    public int WeaponStartKey
    {
        get { return _weaponStartKey; }
    }

    private void Awake()
    {
        LoadWeaponData();
    }

    public int GetWeaponCount()
    {
        return _weaponDatas.Count;
    }

    public WeaponData GetWeaponData(int key)
    {
        return _weaponDatas[key];
    }

    private void LoadWeaponData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/WeaponTable");

        string[] rowData = textAsset.text.Split("\r\n");

        for (int i = 1; i < rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(',');

            if (colData.Length <= 1) continue;
            if (rowData[i] == "") continue;

            WeaponData data;

            data.Key = int.Parse(colData[0]);
            data.Name = colData[1];
            data.PrefabPath = colData[2];
            data.PoolSize = int.Parse(colData[3]);

            _weaponDatas.Add(data.Key, data);
        }
    }
}