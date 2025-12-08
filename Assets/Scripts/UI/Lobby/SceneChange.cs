using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneChange : MonoBehaviour
{
    public static SceneChange Instance;

    [SerializeField] private Image _circleMask;
    [SerializeField] private float _duration = 0.65f;

    private bool _isTransitioning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void TransitionToScene(string sceneName)
    {
        if (_isTransitioning) return; // 중복 클릭 방지
        _isTransitioning = true;

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        if (!_circleMask.gameObject.activeSelf)
            _circleMask.gameObject.SetActive(true);

        StopAllCoroutines();
        _circleMask.fillAmount = 0f;

        StartCoroutine(Transition(sceneName));
    }

    private IEnumerator Transition(string sceneName)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / _duration;
            _circleMask.fillAmount = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        yield return SceneManager.LoadSceneAsync(sceneName);

        _circleMask.fillAmount = 1f;

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / _duration;
            _circleMask.fillAmount = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        _circleMask.fillAmount = 0f;

        _isTransitioning = false; //  다음 클릭 허용
    }

}
