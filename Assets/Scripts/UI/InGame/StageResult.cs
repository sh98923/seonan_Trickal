using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StageResult : MonoBehaviour
{
    private enum Result
    { 
        Victory,
        Defeat
    }

    private ResultButton _resultBtn;
    private Image _resultImage;
    private Sprite _victorySprite;
    private Sprite _defeatSprite;

    private void Awake()
    {
        _resultImage = transform.FindDirectChildComponent<Image>("ResultImage");
        _resultBtn = transform.FindDirectChildComponent<ResultButton>("ButtonGroup");

        _victorySprite = Resources.Load<Sprite>("Sprites/UI/Victory");
        _defeatSprite = Resources.Load<Sprite>("Sprites/UI/Defeat");
    }

    private void OnEnable()
    {
        InGameManager.OnGameVictory += OnVictory;
        InGameManager.OnGameDefeat += OnDefeat;
    }

    private void OnDestroy()
    {
        InGameManager.OnGameVictory -= OnVictory;
        InGameManager.OnGameDefeat -= OnDefeat;
    }

    private void OnVictory()
    {
        gameObject.SetActive(true);
        StartCoroutine(ShowResultAfterDelay(Result.Victory));
    }

    private void OnDefeat()
    {
        gameObject.SetActive(true);
        StartCoroutine(ShowResultAfterDelay(Result.Defeat));
    }

    private IEnumerator ShowResultAfterDelay(Result result)
    {
        yield return new WaitForSeconds(4.0f);

        switch (result)
        {
            case Result.Victory:
                _resultImage.sprite = _victorySprite;
                _resultBtn.ShowVictory();
                break;

            case Result.Defeat:
                _resultImage.sprite = _defeatSprite;
                _resultBtn.ShowDefeat();
                break;
        }
    }
}