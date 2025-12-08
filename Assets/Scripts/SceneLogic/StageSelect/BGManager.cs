using System.Collections.Generic;
using UnityEngine;

public struct BGData
{
    public int Key;
    public string BGPath;
}

public class BGManager : Singleton<BGManager>
{
    private Dictionary<int, BGData> _BGDatas = new Dictionary<int, BGData>();
    
    private readonly int _firstBGKey = 1;
    private int _BGstartKey;
    public int BGStartKey
    {
        get { return _BGstartKey; }
    }

    private void Awake()
    {
        LoadBGData();
    }

    public BGData GetBGData(int key)
    {
        return _BGDatas[key];
    }

    private void LoadBGData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/MapBGTable");

        string[] rowData = textAsset.text.Split("\r\n");

        for (int i = 1; i < rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(",");

            if (colData.Length <= 1) return;
            if (rowData[i] == "") return;

            BGData data;
            data.Key = int.Parse(colData[0]);
            data.BGPath = colData[1];

            if(_firstBGKey == i)
            {
                _BGstartKey = data.Key;
            }

            _BGDatas.Add(data.Key, data);
        }
    }
}
