using UnityEngine;

public class Monster : Character
{
    private float _finalHp;
    private float _finalAtk;

    public void SetMonsterStat(MonsterData data, int wave)
    {
        _criRate = data.CriRate;
        _range = data.Range;
        _atkCoolTime = data.AtkCoolTime;

        float hpLinear = data.Hp + data.HpPerWave * wave;
        float hpExp = data.Hp * Mathf.Pow(data.HpGrowthRate, wave);
        _finalHp = (hpLinear + hpExp) / 2;

        float atkLinear = data.Atk + data.AtkPerWave * wave;
        float atkExp = data.Atk * Mathf.Pow(data.AtkGrowthRate, wave);
        _finalAtk = (atkLinear + atkExp) / 2;
    }
}
