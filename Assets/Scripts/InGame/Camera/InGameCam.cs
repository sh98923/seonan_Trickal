using System;
using System.Collections;
using UnityEngine;

public class InGameCam : MonoBehaviour
{
    private static InGameCam _instance;
    public static InGameCam Instance
    {
        get
        {
            if (_instance == null && Application.isPlaying)
            {
                Debug.LogError("InGameCam 스크립트가 null임");
            }

            return _instance;
        }
    }

    private Camera _cam;
    private Coroutine _zoomCoroutine;

    private event Action _onCameraMoveEnd; // 카메라 이동 종료 이벤트
    public event Action OnCameraMoveEnd
    {
        add { _onCameraMoveEnd += value; }
        remove { _onCameraMoveEnd -= value; }
    }

    private readonly float _zoomInSize = 4.5f;
    private readonly float _zoomOutSize = 5.0f;

    private readonly float _rightMoveX = 12.5f;   // 오른쪽으로 이동 거리
    private readonly float _leftMoveX = -2.0f;   // 왼쪽으로 되돌아가는 이동 거리

    private readonly float _rightDuration = 3.0f;
    private readonly float _leftDuration = 1.25f;

    private bool _isFirstInit = true;

    private void Awake()
    {
        _instance = this;

        _cam = GetComponent<Camera>();
        _cam.orthographicSize = _zoomInSize;

        BattleStateManager.Instance.OnWaveAdvance += ZoomBattleToReroll;
        BattleStateManager.Instance.OnBattle += ZoomRerollToBattle;
    }

    private void OnDisable()
    {
        BattleStateManager.Instance.OnWaveAdvance -= ZoomBattleToReroll;
        BattleStateManager.Instance.OnBattle -= ZoomRerollToBattle;
    }

    private void ZoomRerollToBattle()
    {
        if (_zoomCoroutine != null)
            StopCoroutine(_zoomCoroutine);

        _zoomCoroutine = StartCoroutine(ZoomOutRerollToBattle());
    }

    private void ZoomBattleToReroll()
    {
        /*if (_isFirstInit)
        {
            _isFirstInit = false;  // 첫 진입 이후에는 이벤트 허용
            return;
        }*/

        if (_zoomCoroutine != null)
            StopCoroutine(_zoomCoroutine);

        _zoomCoroutine = StartCoroutine(ZoomInBattleToReroll());
    }

    private IEnumerator ZoomOutRerollToBattle()
    {
        // 줌아웃 + X축 +10
        yield return ZoomCoroutine(_zoomOutSize, _rightMoveX, _rightDuration);
        // X축 -2
        yield return MoveXCoroutine(_leftMoveX, _leftDuration);
        _zoomCoroutine = null;
    }

    // Battle -> Reroll: 줌인 + X축 +10
    private IEnumerator ZoomInBattleToReroll()
    {
        yield return ZoomCoroutine(_zoomInSize, _rightMoveX, _rightDuration);
        _zoomCoroutine = null;

        _onCameraMoveEnd?.Invoke(); // 배틀 상태가 리롤(준비) 상태일때 몬스터 스폰
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
}