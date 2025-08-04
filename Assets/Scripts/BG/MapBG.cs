using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapBG : MonoBehaviour
{
    private enum BG
    {
        LeftSky, RightSky, Ground
    }

    private event Action<MapBG> _onSetMapBG;
    public event Action<MapBG> OnSetMapBG
    {
        add { _onSetMapBG += value; }
        remove { _onSetMapBG -= value; }
    }

    private SpriteRenderer[] _BGSprites;

    private BGData _BGData;

    private string _sceneName;

    private int _BGStartIndex;
    private int _stageKey;

    private void Awake()
    {
        _sceneName = SceneManager.GetActiveScene().name;
        _BGSprites = GetComponentsInChildren<SpriteRenderer>();
        _BGStartIndex = BGManager.Instance.BGStartKey;

        if(_sceneName == "InGameScene")
        {
            SetMapBG();
        }
    }

    private void LoadBG(int index)
    {
        _BGData = BGManager.Instance.GetBGData(_BGStartIndex + index);

        string skyPath = _BGData.SkyPath;
        string groundPath = _BGData.GroundPath;
        bool isFlip = _BGData.Flip;

        Sprite skySprite = Resources.Load<Sprite>(skyPath);
        Sprite groundSprite = Resources.Load<Sprite>(groundPath);

        _BGSprites[(int)BG.LeftSky].sprite = skySprite;
        _BGSprites[(int)BG.RightSky].sprite = skySprite;
        _BGSprites[(int)BG.RightSky].flipX = isFlip;
        _BGSprites[(int)BG.Ground].sprite = groundSprite;
    }

    public void SetMapBGEvent()
    {
        _onSetMapBG?.Invoke(this);
    }

    public void SetMapBG()
    {
        _stageKey = GameManager.Instance.StageKey;
        int mapBGKey = StageManager.Instance.GetMapBGKey(_stageKey);

        LoadBG(mapBGKey);
    }
}