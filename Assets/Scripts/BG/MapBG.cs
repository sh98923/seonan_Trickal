using UnityEngine;

public class MapBG : MonoBehaviour
{
    private enum BG
    {
        leftSky, rightSky, ground
    }

    private SpriteRenderer[] _BGSprites;
    private BGData _BGData;
    private int _BGStartIndex;

    private void Awake()
    {    
        _BGSprites = GetComponentsInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        _BGStartIndex = BGManager.Instance.BGStartKey;
    }

    private void Update()
    {
        for (int i = 1; i <= 3; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                LoadBGByIndex(i);
            }
        }
    }

    private void LoadBGByIndex(int index)
    {
        _BGData = BGManager.Instance.GetBGData(_BGStartIndex + index - 1);

        string skyPath = _BGData.SkyPath;
        string groundPath = _BGData.GroundPath;
        bool isFlip = _BGData.Flip;

        Sprite skySprite = Resources.Load<Sprite>(skyPath);
        Sprite groundSprite = Resources.Load<Sprite>(groundPath);

        _BGSprites[(int)BG.leftSky].sprite = skySprite;
        _BGSprites[(int)BG.rightSky].sprite = skySprite;
        _BGSprites[(int)BG.rightSky].flipX = isFlip;
        _BGSprites[(int)BG.ground].sprite = groundSprite;
    }
}