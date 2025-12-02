using System.Collections.Generic;
using UnityEngine;

public struct CollectionData
{
    public int Key;
    public string KrName;
    public string CharacterSpritePath;
    public string CharacterPrefabPath;
    public string Sentence;
    public string Explanation;
    public string Favorite;
    public string Hate;
    public string Layer;
    public string AtkType;
    public int Hp;
    public int Mp;
    public int Atk;
    public string SkillSpritePath;
    public string Skill;
    public string UltSpritePath;
    public string Ult;
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
            data.Sentence = colData[4];
            data.Explanation = colData[5];
            data.Favorite = colData[6];
            data.Hate = colData[7];
            data.Layer = colData[8];
            data.AtkType = colData[9];
            data.Hp = int.Parse(colData[10]);
            data.Mp = int.Parse(colData[11]);
            data.Atk = int.Parse(colData[12]);
            data.SkillSpritePath = colData[13];
            data.Skill = colData[14];
            data.UltSpritePath = colData[15];
            data.Ult = colData[16];
            
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
