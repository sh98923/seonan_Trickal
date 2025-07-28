using UnityEngine;

public class MapBG : MonoBehaviour
{
    private enum BG
    {
        LeftSky, RightSky, Ground
    }

    private SpriteRenderer[] _BGSprites;
    private BGData _BGData;
    private int _BGStartIndex;
    private int _waveKey;

    private void Awake()
    {    
        _BGSprites = GetComponentsInChildren<SpriteRenderer>();
        _waveKey = GameManager.Instance.WaveKey;
    }

    private void Start()
    {
        _BGStartIndex = BGManager.Instance.BGStartKey;
        LoadBGByIndex(StageManager.Instance.GetStageData(_waveKey).Map);
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
}