using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{
    private List<StageData> _stageDatas = new List<StageData>();
    public List<StageData> StageDatas
    {
        get { return _stageDatas; }
    }

    private int _waveKey;
    public int WaveKey
    {
        get { return _waveKey; }
    }

    private int _mapBGKey;
    public int MapBGKey
    {
        get { return _mapBGKey; }
        set { _mapBGKey = value; }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetStageKey(int waveKey)
    {
        _waveKey = waveKey; 
        _stageDatas = StageManager.Instance.GetStageDataList(_waveKey);
    }
}