using UnityEngine;

public class MapBG : MonoBehaviour
{
    private SpriteRenderer _bgSprite;

    private int _startIndex;

    protected void Awake()
    {
        _bgSprite = GetComponent<SpriteRenderer>();
        _startIndex = BGManager.Instance.BGStartKey;
    }

    private void LoadBG(int index)
    {
        BGData data = BGManager.Instance.GetBGData(_startIndex + index);

        string bgPath = data.BGPath;

        Sprite bgSprite = Resources.Load<Sprite>(bgPath);

        _bgSprite.sprite = bgSprite;
    }

    public void SetMapBG()
    {
        int stageKey = GameManager.Instance.StageKey;
        int mapBGKey = StageManager.Instance.GetMapBGKey(stageKey);

        LoadBG(mapBGKey);
    }
}