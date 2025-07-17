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

        // 두 가지의 공식을 평균 낸 값 (선형, 지수)
        _finalHp = (hpLinear + hpExp) * 0.5f;
        // Round가 정수로만 반올림 하기 떄문에
        // 소수점 첫째자리까지 나오게 하기위한 수식
        _finalHp = Mathf.Round(_finalHp * 10f) * 0.1f;

        float atkLinear = data.Atk + data.AtkPerWave * wave;
        float atkExp = data.Atk * Mathf.Pow(data.AtkGrowthRate, wave);
        _finalAtk = (atkLinear + atkExp) * 0.5f;
        _finalAtk = Mathf.Round(_finalAtk * 10f) * 0.1f;

        print(data.Name + " : " + _finalHp.ToString("F1") + ", " + _finalAtk.ToString("F1"));
    }
}