using System.Collections;
using UnityEngine;
using UnityEngine.UI;



public class StageResult : MonoBehaviour
{
    public enum ResultType
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

        _resultImage.gameObject.SetActive(false);
        _resultBtn.gameObject.SetActive(false);

        _victorySprite = Resources.Load<Sprite>("Sprites/UI/Victory");
        _defeatSprite = Resources.Load<Sprite>("Sprites/UI/Defeat");
    }

    public void ShowResult(ResultType resultType)
    {
        StartCoroutine(ShowResultAfterDelay(resultType));
    }

    private IEnumerator ShowResultAfterDelay(ResultType result)
    {
        yield return new WaitForSeconds(1.5f);

        _resultImage.gameObject.SetActive(true);
        _resultBtn.gameObject.SetActive(true);

        switch (result)
        {
            case ResultType.Victory:
                _resultImage.sprite = _victorySprite;
                _resultBtn.ShowVictory();
                break;

            case ResultType.Defeat:
                _resultImage.sprite = _defeatSprite;
                _resultBtn.ShowDefeat();
                break;
        }
    }
}