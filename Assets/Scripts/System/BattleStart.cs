using UnityEngine;

public class BattleStart : MonoBehaviour
{
    [SerializeField] private GameObject _deckPanel;
    [SerializeField] private GameObject _selectCardPanel;
    [SerializeField] private GameObject _rerollImage;
    [SerializeField] private GameObject _startBtn;

    private bool isDeckMode = false;

    public void OnClickStart()
    {
        _deckPanel.SetActive(false);
        _startBtn.SetActive(false);

        _selectCardPanel.SetActive(true);
        _rerollImage.SetActive(true);

        isDeckMode = true;
    }

    private void Update()
    {
        if (isDeckMode && Input.GetKeyDown(KeyCode.Alpha4))
        {
            _selectCardPanel.SetActive(false);
            _rerollImage.SetActive(false);

            _deckPanel.SetActive(true);
            _startBtn.SetActive(true);

            isDeckMode = false;
        }
    }
}
