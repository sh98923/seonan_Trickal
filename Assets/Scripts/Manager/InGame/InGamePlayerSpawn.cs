using System.Collections.Generic;
using UnityEngine;

public class InGamePlayerSpawn : MonoBehaviour
{
    private List<PlayerData> _deployableDatas = new List<PlayerData>();
    public List<PlayerData> DeployableData
    {
        get { return _deployableDatas; }
    }

    private List<Player> _players = new List<Player>();

    private void Awake()
    {
        LoadPlayerPos();

        // 룰렛 만들게되면 룰렛에 나온 애들만 스폰
        InGameManager.Instance.RegisterDeckUnits(transform);

        for(int i = 0; i < transform.childCount; i++)
        {
            Player player = transform.GetChild(i).GetComponent<Player>();
            _players.Add(player);
        }
    }

    private void LoadPlayerPos()
    {
        List<PlayerData> datas = GameManager.Instance.SpawnablePlayerDatas;

        for(int i = 0; i < datas.Count; i++)
        {
            _deployableDatas.Add(datas[i]);
        }
    }

    public List<Player> GetActivePlayers()
    {
        List<Player> activePlayers = new List<Player>();

        for(int i = 0; i < _players.Count; i++)
        {
            if (_players[i].gameObject.activeSelf)
            {
                activePlayers.Add(_players[i]);
            }    
        }

        return activePlayers;
    }
}