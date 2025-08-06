using System.Collections.Generic;
using UnityEngine;

public class InGamePlayerSpawn : MonoBehaviour
{
    private List<PlayerData> _deployableDatas = new List<PlayerData>();
    public List<PlayerData> DeployableData
    {
        get { return _deployableDatas; }
    }

    private void Awake()
    {
        LoadPlayerPos();
        // 룰렛 만들게되면 룰렛에 나온 애들만 스폰
        GameManager.Instance.RegisterDeckUnits(transform);
    }

    private void LoadPlayerPos()
    {
        List<PlayerData> datas = GameManager.Instance.SpawnablePlayerDatas;

        for(int i = 0; i < datas.Count; i++)
        {
            _deployableDatas.Add(datas[i]);
        }
    }
}