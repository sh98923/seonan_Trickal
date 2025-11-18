using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class InGameCam : MonoBehaviour
{
    private Camera _cam;
    private Coroutine _zoomCoroutine;

    private readonly float _zoomInSize = 4.5f;
    private readonly float _zoomOutSize = 5.0f;
    private readonly float _firstMoveX = 12.5f;   // 첫 이동
    private readonly float _returnMoveX = -2.0f;   // 되돌아가는 이동

    private bool _isFirstInit = true;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        _cam.orthographicSize = _zoomInSize;

        BattleStateManager.Instance.OnReroll += ZoomBattleToReroll;
        BattleStateManager.Instance.OnBattle += ZoomRerollToBattle;
    }

    private void ZoomRerollToBattle()
    {
        if (_zoomCoroutine != null)
            StopCoroutine(_zoomCoroutine);

        _zoomCoroutine = StartCoroutine(ZoomOutRerollToBattle());
    }

    private void ZoomBattleToReroll()
    {
        if (_isFirstInit)
        {
            _isFirstInit = false;  // 첫 진입 이후에는 이벤트 허용
            return;
        }

        if (_zoomCoroutine != null)
            StopCoroutine(_zoomCoroutine);

        _zoomCoroutine = StartCoroutine(ZoomInBattleToReroll());
    }

    private IEnumerator ZoomOutRerollToBattle()
    {
        // 1️⃣ 줌아웃 + X축 +10
        yield return ZoomCoroutine(_zoomOutSize, _firstMoveX, 3.0f);
        // 2️⃣ X축 -2
        yield return MoveXCoroutine(_returnMoveX, 0.75f);
        _zoomCoroutine = null;
    }

    // Battle -> Reroll: 줌인 + X축 +10
    private IEnumerator ZoomInBattleToReroll()
    {
        yield return ZoomCoroutine(_zoomInSize, _firstMoveX, 3.0f);
        _zoomCoroutine = null;
    }

    // ======================================
    // 공용 보간 함수
    // ======================================
    private IEnumerator ZoomCoroutine(float targetSize, float moveX, float duration)
    {
        float elapsed = 0.0f;
        float startSize = _cam.orthographicSize;
        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(startPos.x + moveX, startPos.y, startPos.z);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            _cam.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        _cam.orthographicSize = targetSize;
        transform.position = targetPos;
    }

    // X축만 이동하는 보간
    private IEnumerator MoveXCoroutine(float moveX, float duration)
    {
        float elapsed = 0.0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(startPos.x + moveX, startPos.y, startPos.z);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;
    }

   /* private void ZoomInLerp()
    {
        // 이전 코루틴이 실행 중이면 멈추고 새로 시작
        if (_zoomCoroutine != null)
            StopCoroutine(_zoomCoroutine);

        _zoomCoroutine = StartCoroutine(ZoomCoroutine(_zoomInSize));
    }

    private void ZoomOutLerp()
    {
        // 이전 코루틴이 실행 중이면 멈추고 새로 시작
        if (_zoomCoroutine != null)
            StopCoroutine(_zoomCoroutine);

        _zoomCoroutine = StartCoroutine(ZoomCoroutine(_zoomOutSize));
    }

    private IEnumerator ZoomCoroutine(float targetSize)
    {
        float duration = 1.5f; // 보간 시간
        float elapsed = 0.0f;
        float startSize = _cam.orthographicSize;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _cam.orthographicSize = Mathf.Lerp(startSize, targetSize, elapsed / duration);
            yield return null;
        }

        _cam.orthographicSize = targetSize;
        _zoomCoroutine = null;
    }*/

    private void OnDestroy()
    {
        // 구독 해제
        if (BattleStateManager.Instance != null)
        {
            BattleStateManager.Instance.OnReroll -= ZoomBattleToReroll;
            BattleStateManager.Instance.OnBattle -= ZoomRerollToBattle;
        }
    }
}