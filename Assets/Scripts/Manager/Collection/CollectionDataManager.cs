using System.Collections.Generic;
using UnityEngine;

public struct CollectionData
{
    public int Key;
    public string KrName;
    public string CharacterSpritePath;
    public string CharacterPrefabPath;
    public string Explanation;
    public string Sentence;
    public string Layer;
    public string AtkType;
    public int Hp;
    public int Atk;
}

public class CollectionDataManager : Singleton<CollectionDataManager>
{
    private Dictionary<int, CollectionData> _collectionDatas = new Dictionary<int, CollectionData>();

    private int _collectionStartKey;

    public int CollectionStartKey
    {
        get { return _collectionStartKey; }
    }

    private int _collectionCount = 0;

    public int CollectionCount
    {
        get { return _collectionCount; }
    }

    private bool _isCollectionFirstKey = false;
    private void Awake()
    {
        LoadCollectionDatas();
    }

    public CollectionData GetCollectionData(int key)
    {
        return _collectionDatas[key];
    }

    private void LoadCollectionDatas()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/CollectionInfoTable");

        string[] rowData = textAsset.text.Split("\n");

        for (int i = 1; i < rowData.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(rowData[i])) continue;

            string[] colData = rowData[i].Split("\t");

            if (colData.Length < 10) continue;

            CollectionData data;

            data.Key = int.Parse(colData[0]);
            data.KrName = colData[1];
            data.CharacterSpritePath = colData[2];
            data.CharacterPrefabPath = colData[3];
            data.Explanation = colData[4];
            data.Sentence = colData[5];
            data.Layer = colData[6];
            data.AtkType = colData[7];
            data.Hp = int.Parse(colData[8]);
            data.Atk = int.Parse(colData[9]);

            if(!_isCollectionFirstKey)
            {
                _isCollectionFirstKey = true;
                _collectionStartKey = data.Key;
            }

            _collectionDatas.Add(data.Key, data);
            _collectionCount++;
        }
    }
}
