using System.Collections.Generic;
using UnityEngine;

public struct BGData
{
    public int Key;
    public string SkyPath;
    public string GroundPath;
    public bool Flip;
}

public class BGManager : MonoBehaviour
{
    public static BGManager Instance;

    private Dictionary<int, BGData> _BGDatas = new Dictionary<int, BGData>();
    
    private readonly int _firstBGKey = 1;
    private int _BGstartKey;
    public int BGStartKey
    {
        get { return _BGstartKey; }
    }

    private void Awake()
    {
        Instance = this;

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
            data.SkyPath = colData[1];
            data.GroundPath = colData[2];
            data.Flip = bool.Parse(colData[3]);

            if(_firstBGKey == i)
            {
                _BGstartKey = data.Key;
            }

            _BGDatas.Add(data.Key, data);
        }
    }
}
