using UnityEngine;
using UnityEngine.UIElements;

public class MoveBG : MonoBehaviour
{
    private Camera _mainCam;
    private InGameBG _mapBG;
    private SpriteRenderer[] _inGameBGSprites = new SpriteRenderer[3];

    private Vector2 _left = Vector2.left;

    private const float _moveSpeed = 1.2f;
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
            for (int i = 0; i < _inGameBGSprites.Length; i++)
            {
                Vector3 pos = _inGameBGSprites[i].transform.position;
                pos.x += _left.x * _moveSpeed * Time.deltaTime;
                _inGameBGSprites[i].transform.position = pos;
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

            float width = RoundPos(sprite.bounds.size.x);
            float half = width * 0.5f;

            float left = RoundPos(sprite.transform.position.x - half);
            float right = RoundPos(sprite.transform.position.x + half);

            // 왼쪽으로 벗어났을 때 → 오른쪽 끝으로
            if (right < camLeftX)
            {
                float rightMost = sprites[0].transform.position.x;

                for (int j = 1; j < sprites.Length; j++)
                {
                    float x = sprites[j].transform.position.x;
                    if (x > rightMost)
                        rightMost = x;
                }

                sprite.transform.position = new Vector3(
                    RoundPos(rightMost + width),
                    sprite.transform.position.y,
                    sprite.transform.position.z
                );

                sprite.flipX = !sprite.flipX;
            }
            // 오른쪽으로 벗어났을 때 → 왼쪽 끝으로
            else if (left > camRightX)
            {
                float leftMost = sprites[0].transform.position.x;

                for (int j = 1; j < sprites.Length; j++)
                {
                    float x = sprites[j].transform.position.x;
                    if (x < leftMost)
                        leftMost = x;
                }

                sprite.transform.position = new Vector3(
                    RoundPos(leftMost - width),
                    sprite.transform.position.y,
                    sprite.transform.position.z
                );

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
    }

    // 0.1 단위 반올림
    private float RoundPos(float value)
    {
        return Mathf.Round(value * 10.0f) * 0.1f;
    }
}
