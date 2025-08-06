using System;
using UnityEngine;

public class StageMapBG : MapBG
{
    private event Action<StageMapBG> _onSetMapBG;
    public event Action<StageMapBG> OnSetMapBG
    {
        add { _onSetMapBG += value; }
        remove { _onSetMapBG -= value; }
    }

    public void SetMapBGEvent()
    {
        _onSetMapBG?.Invoke(this);
    }
}