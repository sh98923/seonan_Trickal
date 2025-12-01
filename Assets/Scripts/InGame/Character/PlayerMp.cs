using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어 MP 관리 컴포넌트
/// - Battle 상태에서만 자동 회복
/// - MP 사용/소모 가능
/// </summary>
public class PlayerMp : MonoBehaviour
{
    private Coroutine _regenCoroutine;

    private float _mpPerTick = 10.0f;      // 1회 회복량
    private float _tickInterval = 0.0f;

    private float _curMp = 0.0f;
    private float _maxMp = 0.0f;

    private void OnEnable()
    {
        BattleStateManager.Instance.OnBattle += StartMpRegen;
        BattleStateManager.Instance.OnEnteringReroll += StopMpRegen; // 웨이브 이동 시 중지
    }

    private void OnDisable()
    {
        BattleStateManager.Instance.OnBattle -= StartMpRegen;
        BattleStateManager.Instance.OnEnteringReroll -= StopMpRegen;
    }

    private void OnDestroy()
    {
        if (BattleStateManager.Instance != null)
        {
            BattleStateManager.Instance.OnBattle -= StartMpRegen;
            BattleStateManager.Instance.OnEnteringReroll -= StopMpRegen;
        }
    }

    public void SetMpData(float maxMp, float tickRate)
    {
        _maxMp = maxMp;
        _tickInterval = tickRate;
    }

    private void StartMpRegen()
    {
        if (_regenCoroutine == null)
            _regenCoroutine = StartCoroutine(RegenerateMp());
    }

    private void StopMpRegen()
    {
        if (_regenCoroutine != null)
        {
            StopCoroutine(_regenCoroutine);
            _regenCoroutine = null;
        }
    }

    private IEnumerator RegenerateMp()
    {
        while (true)
        {
            yield return new WaitForSeconds(_tickInterval);

            if (_curMp < _maxMp)
            {
                _curMp += _mpPerTick;

                if (_curMp > _maxMp)
                { 
                    _curMp = _maxMp;
                }
            }
        }
    }

    public void UseMp()
    {
        _curMp = 0.0f;
    }

    public float GetCurMp()
    {
        return _curMp;
    }
}