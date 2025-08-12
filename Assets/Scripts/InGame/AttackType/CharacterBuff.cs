using UnityEngine;

public class CharacterBuff : CharacterAction
{
    public override void SetInit()
    {
        print("초기화");
    }

    public override void Excute()
    {
        print("버프");
    }
}