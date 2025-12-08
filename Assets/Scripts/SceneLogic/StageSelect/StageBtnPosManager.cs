using System.Collections.Generic;
using UnityEngine;

public struct StageBtnData
{
    public int Key;
    public int StageKey;
    public Vector3 pos;
}

public class StageBtnPosManager : Singleton<StageBtnPosManager>
{
    private Dictionary<int, StageBtnData> _stageBtnPosDatas = new Dictionary<int, StageBtnData>();

    private int _stageBtnStartKey;
    public int StageBtnStartKey
    {
        get { return _stageBtnStartKey; }
    }

    private void Awake()
    {
        LoadStageBtnPosData();
    }

    public Vector3 GetStageBtnPos(int key)
    {
        return _stageBtnPosDatas[key].pos;
    }

    public int GetStageKey(int key)
    {
        return _stageBtnPosDatas[key].StageKey;
    }

    private void LoadStageBtnPosData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Tables/StageButtonPosTable");

        string[] rowData = textAsset.text.Split("\r\n");

        for(int i = 1; i <  rowData.Length; i++)
        {
            string[] colData = rowData[i].Split(",");

            if (colData.Length <= 1) continue;
            if (rowData[i] == "") continue;

            StageBtnData data;

            data.Key = int.Parse(colData[0]);
            data.StageKey = int.Parse(colData[1]);
            data.pos.x = float.Parse(colData[2]);
            data.pos.y = float.Parse(colData[3]);
            data.pos.z = float.Parse(colData[4]);

            if(i == 1)
            {
                _stageBtnStartKey = data.Key;
            }

            _stageBtnPosDatas.Add(data.Key, data);
        }
    }
}