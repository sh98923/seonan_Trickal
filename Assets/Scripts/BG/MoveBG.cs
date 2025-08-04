using UnityEngine;

public class MoveBG : MonoBehaviour
{
    private SpriteRenderer[] _skySprites = new SpriteRenderer[2];
    private SpriteRenderer[] _groundSprites = new SpriteRenderer[2];

    private MapBG _mapBG;

    private Camera _mainCam;

    private float _screenWidthWorld;

    private bool _isInit = false;

    private void Awake()
    {
        _mapBG = GetComponent<MapBG>();
        _mainCam = Camera.main;

        float screenHalfWidth = _mainCam.orthographicSize * _mainCam.aspect;
        _screenWidthWorld = screenHalfWidth;
    }

    private void OnEnable()
    {
        _mapBG.OnMoveBGInit += InitSprites;
    }

    private void Update()
    {
        if (!_isInit) return;

        MoveLoop(_skySprites);
        MoveLoop(_groundSprites);
    }

    private void MoveLoop(SpriteRenderer[] sprites)
    {
        float camLeftX = _mainCam.transform.position.x - _screenWidthWorld;

        foreach (SpriteRenderer sprite in sprites)
        {
            float spriteWidth = sprite.bounds.size.x;
            float rightMostX = sprite.transform.position.x + spriteWidth * 0.5f;

            // 카메라 왼쪽 경계보다 스프라이트 오른쪽 끝이 더 왼쪽이면 → 벗어났음
            if (rightMostX < camLeftX)
            {
                print(spriteWidth);
                sprite.transform.position += Vector3.right * spriteWidth * 2.0f;
            }
        }
    }

    public void InitSprites()
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
    }
}
