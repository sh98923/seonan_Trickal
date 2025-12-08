using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapBG : MonoBehaviour
{
    private enum BG
    {
        LeftBG, MiddleBG, RightBG
    }

    private SpriteRenderer[] _sprites;
    public SpriteRenderer[] BGSprites
    {
        get { return _sprites; }
    }

    private BGData _data;

    private int _startIndex;
    private int _stageKey;

    protected void Awake()
    {
        _sprites = GetComponentsInChildren<SpriteRenderer>();
        _startIndex = BGManager.Instance.BGStartKey;
    }

    public void SetMapBG()
    {
        _stageKey = GameManager.Instance.StageKey;
        int mapBGKey = StageManager.Instance.GetMapBGKey(_stageKey);

        LoadBG(mapBGKey);
    }

    protected virtual void LoadBG(int index)
    {
        _data = BGManager.Instance.GetBGData(_startIndex + index);

        string bgPath = _data.BGPath;

        Sprite bgSprite = Resources.Load<Sprite>(bgPath);

        for(int i = 0; i < _sprites.Length; i++)
        {
            _sprites[i].sprite = bgSprite;
        }

        _sprites[(int)BG.MiddleBG].flipX = true;
    }
}