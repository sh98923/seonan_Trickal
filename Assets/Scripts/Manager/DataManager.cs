using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using System.Collections.Generic;

public struct PlayerData
{
    public int Key;
    public string Name;
    public int PositionX;
    public int Hp;
    public int Atk;
    public float CriRate;
    public float Range;
}

public struct EnemyData
{
    public int Key;
    public string Name;
    public int SpawnStage;
    public int Hp;
    public int Atk;
    public float CriRate;
    public float Range;
    public int Count;
}

public class DataManager : SingletonData<DataManager>
{
    private Dictionary<int, PlayerData> playerDatas = new Dictionary<int, PlayerData>();
    private Dictionary<int, EnemyData> enemyDatas = new Dictionary<int, EnemyData>();

    public void Awake()
    {
        LoadPlayerData();
        LoadEnemyData();
    }

    public PlayerData GetPlayerData(int key)
    {
        return playerDatas[key];
    }

    public EnemyData GetEnemyData(int key)
    {
        return enemyDatas[key];
    }

    private void LoadPlayerData()
    {
        LoadPlayerTable();
    }

    private void LoadEnemyData()
    {
        LoadEnemyTable();
    }

    public int GetTotalPlayerCount()
    {
        return playerDatas.Count;
    }

    public int GetTotalEnemyCount()
    {
        return enemyDatas.Count;
    }

    private void LoadPlayerTable()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/PlayableTable");

        string temp = textAsset.text.Replace("\r\n", "\n");

        string[] str = temp.Split('\n');

        for (int i = 1; i < str.Length; i++)
        {
            if (str[i].Length == 0)
                return;

            string[] data = str[i].Split(',');

            PlayerData playerData;
            playerData.Key = int.Parse(data[0]);
            playerData.Name = data[1];
            playerData.PositionX = int.Parse(data[2]);
            playerData.Hp = int.Parse(data[3]);
            playerData.Atk = int.Parse(data[4]);
            playerData.CriRate = float.Parse(data[5]);
            playerData.Range = float.Parse(data[6]);

            if (playerDatas.ContainsKey(playerData.Key))
                continue;

            playerDatas.Add(playerData.Key, playerData);
        }
    }

    private void LoadEnemyTable()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/EnemyTable");

        string temp = textAsset.text.Replace("\r\n", "\n");

        string[] str = temp.Split('\n');

        for (int i = 1; i < str.Length; i++)
        {
            if (str[i].Length == 0)
                return;

            string[] data = str[i].Split(',');

            EnemyData enemyData;
            enemyData.Key = int.Parse(data[0]);
            enemyData.Name = data[1];
            enemyData.SpawnStage = int.Parse(data[2]);
            enemyData.Hp = int.Parse(data[3]);
            enemyData.Atk = int.Parse(data[4]);
            enemyData.CriRate = float.Parse(data[5]);
            enemyData.Range = float.Parse(data[6]);
            enemyData.Count = int.Parse(data[7]);

            if (enemyDatas.ContainsKey(enemyData.Key))
                continue;

            enemyDatas.Add(enemyData.Key, enemyData);
        }
    }

    public List<EnemyData> GetStageEnemies(int stageKey)
    {
        List<EnemyData> stageEnemies = new List<EnemyData>();

        foreach (var enemy in enemyDatas.Values)
        {
            if (enemy.SpawnStage == stageKey)
            {
                stageEnemies.Add(enemy);
            }
        }

        return stageEnemies;
    }
}
