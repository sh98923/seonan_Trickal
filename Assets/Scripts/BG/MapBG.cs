using System;
using UnityEngine;

public class MapBG : MonoBehaviour
{
    private enum BG
    {
        LeftSky, RightSky, Ground
    }

    private event Action<MapBG> _onMapFound;
    public event Action<MapBG> OnMapFound
    {
        add { _onMapFound += value; }
        remove { _onMapFound -= value; }
    }

    private event Action<MapBG> _onSetMapBG;
    public event Action<MapBG> OnSetMapBG
    {
        add { _onSetMapBG += value; }
        remove { _onSetMapBG -= value; }
    }

    private SpriteRenderer[] _BGSprites;

    private BGData _BGData;

    private int _BGStartIndex;
    private int _stageKey;

    private void Awake()
    {
        _BGSprites = GetComponentsInChildren<SpriteRenderer>();
        _stageKey = GameManager.Instance.StageKey;
    }

    private void Start()
    {
        _onMapFound?.Invoke(this);
    }

    private void OnEnable()
    {
        _onSetMapBG?.Invoke(this);
    }

    private void LoadBGByIndex(int index)
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

    public void InActiveBG()
    {
        gameObject.SetActive(false);
    }

    public void SetMapBG()
    {
        _stageKey = GameManager.Instance.StageKey;
        _BGStartIndex = BGManager.Instance.BGStartKey;
        LoadBGByIndex(StageManager.Instance.GetStageData(_stageKey).Map);
    }
}