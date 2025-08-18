using UnityEngine;

public class Effect : MonoBehaviour
{
    private Animator _animator;
    private Transform _characterTransform;
    private SpriteRenderer _sprite;

    private readonly Vector3 _posOffset = new Vector3(0.0f, 0.3f, 0.0f);

    private readonly int _sortingScale = 100;
    private readonly int _sortingOffset = 31;

    private float _timer = 0.0f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_timer >= 0.0f)
        {
            transform.position = _characterTransform.position + _posOffset;

            _timer -= Time.deltaTime;

            // 타이머 종료 시 이펙트 비활성화
            if (_timer <= 0.0f)
            {
                gameObject.SetActive(false);
            }
        }

        _sprite.sortingOrder = Mathf.RoundToInt(-transform.position.y * _sortingScale + _sortingOffset);
    }

    public void Play(Transform characterTransform, string clipName, float time = 0.0f)
    {
        _characterTransform = characterTransform;

        _animator.Play(clipName);

        // time이 0이면 재생 중인 클립 길이 가져오기
        if (time <= 0.0f)
        {
            // 현재 레이어 0의 스테이트 정보
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            _timer = stateInfo.length;
        }
        else
        {
            _timer = time;
        }
    }
}