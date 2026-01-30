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
        InGameManager.OnGameVictory += ShowVictory;
        InGameManager.OnGameDefeat += ShowDefeat;
    }

    private void OnDestroy()
    {
        InGameManager.OnGameVictory -= ShowVictory;
        InGameManager.OnGameDefeat -= ShowDefeat;
    }

    private void ShowVictory()
    {
        _resultImage.sprite = _victorySprite;
        _resultBtn.ShowVictory();
    }

    private void ShowDefeat()
    {
        _resultImage.sprite = _defeatSprite;
        _resultBtn.ShowDefeat();
    }
}