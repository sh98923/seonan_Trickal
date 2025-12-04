using UnityEngine;
using UnityEngine.UIElements;

public class MoveBG : MonoBehaviour
{
    private SpriteRenderer[] _inGameBGSprites = new SpriteRenderer[3];

    private InGameBG _mapBG;

    private Camera _mainCam;

    private Vector2 _left = Vector2.left;
    private Vector3 _lastCamPos;

    private const float _moveSpeed = 0.8f;
    private float _screenWidth;

    private bool _isInit = false;

    private void Awake()
    {
        _mapBG = GetComponent<InGameBG>();
        _mainCam = Camera.main;
    }

    private void OnEnable()
    {
        _mapBG.OnMoveBGInit += InitSprites;
    }

    private void Update()
    {
        if (!_isInit)
        {
            return;
        }

        if(!InGameManager.Instance.IsGameStart)
        {
            return;
        }

        // 카메라가 이동했을 때만 처리
        MoveLoop(_inGameBGSprites);

        if(InGameManager.Instance.CanBGMove)
        {
            for(int i = 0; i < _inGameBGSprites.Length; i++)
            {
                _inGameBGSprites[i].transform.Translate(_left * _moveSpeed * Time.deltaTime);
            }
        }
    }

    private void MoveLoop(SpriteRenderer[] sprites)
    {
        float camLeftX = _mainCam.transform.position.x - _screenWidth;
        float camRightX = _mainCam.transform.position.x + _screenWidth;

        for (int i = 0; i < sprites.Length; i++)
        {
            SpriteRenderer sprite = sprites[i];

            float width = sprite.bounds.size.x;
            float half = width * 0.5f;

            float left = sprite.transform.position.x - half;
            float right = sprite.transform.position.x + half;

            // BG가 왼쪽으로 벗어났을 때 → 오른쪽 끝으로 이동
            if (right < camLeftX)
            {
                float rightMost = sprites[0].transform.position.x;

                for (int j = 1; j < sprites.Length; j++)
                {
                    float posX = sprites[j].transform.position.x;
                    if (posX > rightMost)
                        rightMost = posX;
                }

                sprite.transform.position = new Vector3(rightMost + width, sprite.transform.position.y, sprite.transform.position.z);
                sprite.flipX = !sprite.flipX;
            }
            // BG가 오른쪽으로 벗어났을 때 → 왼쪽 끝으로 이동
            else if (left > camRightX)
            {
                float leftMost = sprites[0].transform.position.x;

                for (int j = 1; j < sprites.Length; j++)
                {
                    float posX = sprites[j].transform.position.x;
                    if (posX < leftMost)
                        leftMost = posX;
                }

                sprite.transform.position = new Vector3(leftMost - width, sprite.transform.position.y, sprite.transform.position.z);
                sprite.flipX = !sprite.flipX;
            }
        }
    }

    public void InitSprites(InGameBG bg)
    {
        _isInit = true;

        SpriteRenderer[] sprites = _mapBG.BGSprites;

        for (int i = 0; i < sprites.Length; i++)
        { 
            _inGameBGSprites[i] = sprites[i];
        }

        _screenWidth = _inGameBGSprites[0].sprite.bounds.size.x;
        _lastCamPos = _mainCam.transform.position; // 초기 카메라 위치 기록
    }
}
