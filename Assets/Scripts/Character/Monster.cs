using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Monster : Character
{
    private float _finalHp;
    private float _finalAtk;

    [SerializeField] private float _searchRadius = 5f;
    [SerializeField] private float _moveSpeed = 2f;

    private Transform _target;

    private void Update()
    {
        FindPlayer();

        if (_target != null)
        {
            Vector2 dir = (_target.position - transform.position).normalized;
            transform.position += (Vector3)(dir * _moveSpeed * Time.deltaTime);
        }
    }

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

    private void FindPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _searchRadius);

        float closestDistance = float.MaxValue;
        Transform closestPlayer = null;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestPlayer = hit.transform;
                }
            }
        }

        _target = closestPlayer;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _searchRadius);
    }
}