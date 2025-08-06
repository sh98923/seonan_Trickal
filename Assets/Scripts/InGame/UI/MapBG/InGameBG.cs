using System;

public class InGameBG : MapBG
{
    private event Action<InGameBG> _onMoveBGInit;
    public event Action<InGameBG> OnMoveBGInit
    {
        add { _onMoveBGInit += value; }
        remove { _onMoveBGInit -= value; }
    }

    private void Awake()
    {
        base.Awake();
        SetMapBG();
    }

    protected override void LoadBG(int index)
    {
        base.LoadBG(index);

        _onMoveBGInit?.Invoke(this);
    }
}