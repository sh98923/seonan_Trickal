using UnityEngine;

public class MapBG : MonoBehaviour
{
    private enum BG
    {
        sky, ground
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

        Sprite skySprite = Resources.Load<Sprite>(skyPath);
        Sprite groundSprite = Resources.Load<Sprite>(groundPath);

        _BGSprites[(int)BG.sky].sprite = skySprite;
        _BGSprites[(int)BG.ground].sprite = groundSprite;
    }
}