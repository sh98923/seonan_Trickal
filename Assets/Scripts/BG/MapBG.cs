using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapBG : MonoBehaviour
{
    private enum BG
    {
        LeftSky, RightSky, LeftGround, RightGround
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

        string skyPath = _data.SkyPath;
        string groundPath = _data.GroundPath;
        bool isFlip = _data.Flip;

        Sprite skySprite = Resources.Load<Sprite>(skyPath);
        Sprite groundSprite = Resources.Load<Sprite>(groundPath);

        _sprites[(int)BG.LeftSky].sprite = skySprite;
        _sprites[(int)BG.RightSky].sprite = skySprite;
        _sprites[(int)BG.RightSky].flipX = isFlip;
        _sprites[(int)BG.LeftGround].sprite = groundSprite;
        _sprites[(int)BG.RightGround].sprite = groundSprite;
    }
}