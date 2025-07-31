public class GameManager : Singleton<GameManager>
{
    private readonly int _inGameStartCoin = 30;
    public int InGameStartCoin
    {
        get { return _inGameStartCoin; }
    }

    private int _waveCount;
    public int WaveCount
    {
        get { return _waveCount; }
    }

    private int _waveKey;
    public int WaveKey
    {
        get { return _waveKey; }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetStageKey(int waveKey)
    {
        _waveKey = waveKey; 
        _waveCount = StageManager.Instance.GetWaveCount(_waveKey);
    }
}