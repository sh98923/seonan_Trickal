using UnityEngine;

public class MoveBG : MonoBehaviour
{
    private SpriteRenderer[] _skySprites = new SpriteRenderer[2];
    private SpriteRenderer[] _groundSprites = new SpriteRenderer[2];

    private InGameBG _mapBG;

    private Camera _mainCam;

    private readonly float _screenEdgeBuffer = 1.2f;
    private float _screenHalfWidth;

    private bool _isInit = false;

    private Vector3 _lastCamPos;

    private void Awake()
    {
        _mapBG = GetComponent<InGameBG>();
        _mainCam = Camera.main;

        _screenHalfWidth = _mainCam.orthographicSize * _mainCam.aspect;
    }

    private void OnEnable()
    {
        _mapBG.OnMoveBGInit += InitSprites;
    }

    private void Update()
    {
        if (!_isInit) return;

        // 카메라가 이동했을 때만 처리
        if (_mainCam.transform.position.x != _lastCamPos.x)
        {
            MoveLoop(_skySprites);
            MoveLoop(_groundSprites);

            _lastCamPos = _mainCam.transform.position;
        }
    }

    private void MoveLoop(SpriteRenderer[] sprites)
    {
        float camLeftX = _mainCam.transform.position.x - _screenHalfWidth * _screenEdgeBuffer;
        float camRightX = _mainCam.transform.position.x + _screenHalfWidth * _screenEdgeBuffer;

        foreach (SpriteRenderer sprite in sprites)
        {
            float width = sprite.bounds.size.x;
            float half = width * 0.5f;

            float left = sprite.transform.position.x - half;
            float right = sprite.transform.position.x + half;

            // 카메라 왼쪽 밖으로 벗어났을 때
            if (right < camLeftX)
            {
                sprite.transform.position += Vector3.right * width * 2.0f;
            }
            // 카메라 오른쪽 밖으로 벗어났을 때
            else if (left > camRightX)
            {
                sprite.transform.position -= Vector3.right * width * 2.0f;
            }
        }
    }

    public void InitSprites(InGameBG bg)
    {
        _isInit = true;

        SpriteRenderer[] sprites = _mapBG.BGSprites;

        int skyIndex = 0;
        int groundIndex = 0;

        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i].CompareTag("Sky"))
            {
                _skySprites[skyIndex++] = sprites[i];
            }
            else if (sprites[i].CompareTag("Ground"))
            {
                _groundSprites[groundIndex++] = sprites[i];
            }
        }

        _lastCamPos = _mainCam.transform.position; // 초기 카메라 위치 기록
    }
}
