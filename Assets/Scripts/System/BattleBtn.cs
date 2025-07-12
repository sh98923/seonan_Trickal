using UnityEngine;

public class BattleBtn : MonoBehaviour
{
    [SerializeField] private GameObject _deckPanel;
    [SerializeField] private GameObject _selectCardPanel;
    [SerializeField] private GameObject _rerollImage;
    [SerializeField] private GameObject _startBtn;

    private bool _isDeckMode = false;

    public void OnClickStart()
    {
        _deckPanel.SetActive(false);
        _startBtn.SetActive(false);

        _selectCardPanel.SetActive(true);
        _rerollImage.SetActive(true);

        _isDeckMode = true;
        BattleStateManager.Instance.IsBattleStart = true;
    }

    private void Update()
    {
        if (_isDeckMode && Input.GetKeyDown(KeyCode.Alpha4))
        {
            _selectCardPanel.SetActive(false);
            _rerollImage.SetActive(false);

            _deckPanel.SetActive(true);
            _startBtn.SetActive(true);

            _isDeckMode = false;
            BattleStateManager.Instance.IsBattleStart = false;
        }
    }
}
