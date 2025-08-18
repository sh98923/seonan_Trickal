using System.Collections.Generic;
using UnityEngine;

public class CharacterBuff : CharacterAction
{
    private List<Player> _activePlayers = new List<Player>();

    private float _timer = 0.0f;
    private string _clipName = "";

    public override void SetBuffInfo(string clipName, float time)
    {
        _timer = time;
        _clipName = clipName;
    }

    private void Buff()
    {
        _activePlayers = InGameManager.Instance.GetActiveUnits();

        for (int i = 0; i < _activePlayers.Count; i++)
        {
            Transform characterTransform = _activePlayers[i].transform;

            EffectManager.Instance.PlayEffect(characterTransform, _clipName, _timer);
        }
    }

    public override void Excute()
    {
        Buff();
    }
}