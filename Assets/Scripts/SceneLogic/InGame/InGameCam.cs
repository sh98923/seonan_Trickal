using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    private Vector3 _camOriginalPos = new Vector3();
    public float CamOriginalPosX
    {
        get { return _camOriginalPos.x; }
    }

    private float _nextWaveX = 0.0f;

    // =========================
    // 줌인아웃 모드 관련
    // =========================
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

    private readonly float _moveDuration = 2.0f;
    private readonly float _zoomDuration = 1.0f;

    private readonly float _rerollToBattleMoveX = 13.0f;
    private readonly float _battleToRerollMoveX = 9.0f;

    private readonly float _camMoveSecond = 0.6f;

    //private readonly float _rightDuration = 2.3f;

    //private bool _isFirstInit = true;

    // =========================
    // Tracking 모드 관련
    // =========================
    private List<ITrackable> _players = new List<ITrackable>();
    private List<ITrackable> _trackedCharacters = new List<ITrackable>();

    private bool _isTrackingMode = false;

    private void Awake()
    {
        _instance = this;

        _cam = GetComponent<Camera>();
        _cam.orthographicSize = _zoomInSize;

        BattleStateManager.Instance.OnBattle += EnableTracking;
        BattleStateManager.Instance.OnEnteringBattle += ZoomRerollToBattle;
        BattleStateManager.Instance.OnEnteringReroll += ZoomBattleToReroll;
        BattleStateManager.Instance.OnEnteringReroll += PlayerMoveNextWave;
    }

    private void Start()
    {
        _camOriginalPos = transform.position;
        _players = InGameManager.Instance.Players;
        _trackedCharacters = InGameManager.Instance.Trackables;
    }

    private void OnDisable()
    {
        BattleStateManager.Instance.OnBattle -= EnableTracking;
        BattleStateManager.Instance.OnEnteringBattle -= ZoomRerollToBattle;
        BattleStateManager.Instance.OnEnteringReroll -= ZoomBattleToReroll;
        BattleStateManager.Instance.OnEnteringReroll -= PlayerMoveNextWave;
    }

    private void OnDestroy()
    {
        if(BattleStateManager.Instance != null)
        {
            BattleStateManager.Instance.OnBattle -= EnableTracking;
            BattleStateManager.Instance.OnEnteringBattle -= ZoomRerollToBattle;
            BattleStateManager.Instance.OnEnteringReroll -= ZoomBattleToReroll;
            BattleStateManager.Instance.OnEnteringReroll -= PlayerMoveNextWave;
        }
    }

    private void Update()
    {
        if (!_isTrackingMode)
        {
            return;
        }

        if (BattleStateManager.Instance.CurrentState != BattleState.Battle)
        {
            return;
        }

        float centerX = GetBattleCenterX();

        Vector3 pos = transform.position; 
        pos.x = Mathf.Lerp(pos.x, centerX, Time.deltaTime);
        transform.position = pos;
    }

    private void PlayerMoveNextWave()
    {
        // WaveAdvance 목표 위치 미리 계산 후 플레이어에 전달
        _nextWaveX = transform.position.x + _battleToRerollMoveX;

        foreach (ITrackable character in _players)
        {
            PlayerMovement player = character.Object.GetComponent<PlayerMovement>();

            if(player.gameObject.activeSelf)
            {
                player.UpdateNextDestX(_nextWaveX);
            }
            else
            {
                player.UpdateSpawnX(_nextWaveX);
            }
        }
    }

    private void ZoomRerollToBattle()
    {
        DisableTracking();  // Reroll → Battle 연출 동안 Tracking X

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

        DisableTracking(); // Tracking 중단

        if (_zoomCoroutine != null)
            StopCoroutine(_zoomCoroutine);

        _zoomCoroutine = StartCoroutine(ZoomInBattleToReroll());
    }

    // Reroll -> Battle
    private IEnumerator ZoomOutRerollToBattle()
    {
        // 줌 아웃
        yield return ZoomCoroutine(_zoomOutSize, _rerollToBattleMoveX);

        _zoomCoroutine = null;
    }

    // Battle -> Reroll
    private IEnumerator ZoomInBattleToReroll()
    {
        // 줌 인
        yield return ZoomCoroutine(_zoomInSize, _battleToRerollMoveX);
        _zoomCoroutine = null;

        _onCameraMoveEnd?.Invoke(); // 인게임에서 상태가 리롤(준비) 상태일때 몬스터 스폰
    }   

    // ======================================
    // 공용 보간 함수
    // ======================================
    private IEnumerator ZoomCoroutine(float targetSize, float moveX)
    {
        float elapsed = 0.0f;
        float camSize = _cam.orthographicSize;

        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(startPos.x + moveX, startPos.y, startPos.z);

        // BG는 무조건 일단 멈춤
        InGameManager.Instance.CanBGMove = false;

        while (elapsed < _moveDuration)
        {
            elapsed += Time.deltaTime;

            float moveT = Mathf.Clamp01(elapsed / _moveDuration);
            float zoomT = Mathf.Clamp01(elapsed / _zoomDuration);

            // Battle 상태이면 줌만 목표값으로 즉시 변경
            if (BattleStateManager.Instance.CurrentState == BattleState.Battle)
            {
                _cam.orthographicSize = targetSize;

                float remainingDuration = Mathf.Max(0, _moveDuration - elapsed);

                // Battle 상태를 인지한 시점에서 0.5초 정도 보간 유지
                yield return StartCoroutine(MoveCameraPositionOverTime(transform.position, targetPos, remainingDuration));

                yield break;
            }
            else
            {
                _cam.orthographicSize = Mathf.Lerp(camSize, targetSize, zoomT);
                transform.position = Vector3.Lerp(startPos, targetPos, moveT);
            }

            yield return null;
        }

        // 연출 정상 종료 시 최종값 보정
        _cam.orthographicSize = targetSize;
        transform.position = targetPos;

        // 리롤 정비 상태로 가는 중이라면 BG를 움직임
        if(BattleStateManager.Instance.CurrentState == BattleState.EnteringReroll)
        {
            InGameManager.Instance.CanBGMove = true;
        }
    }

    private IEnumerator MoveCameraPositionOverTime(Vector3 startPos, Vector3 endPos, float duration)
    {
        float timer = 0.0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            if (timer >= _camMoveSecond)
            {
                yield break;
            }

            float t = Mathf.Clamp01(timer / duration);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
    }

    // X축만 이동하는 보간
    /*private IEnumerator MoveXCoroutine(float moveX, float duration)
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
    }*/

    // =======================
    // Tracking 구간
    // =======================
    private void EnableTracking()
    {
        _isTrackingMode = true;
    }

    private void DisableTracking()
    {
        _isTrackingMode = false;
    }

    private float GetBattleCenterX()
    {
        float left = float.MaxValue;
        float right = float.MinValue;
        bool found = false;

        foreach (ITrackable character in _trackedCharacters)
        {
            if (!character.Object.activeSelf || !character.IsColliderEnable)
            {
                continue;
            }

            float posX = character.Object.transform.position.x;

            found = true;
            left = Mathf.Min(left, posX);
            right = Mathf.Max(right, posX);
        }

        if (!found)
            return transform.position.x;

        return (left + right) * 0.5f;
    }
}