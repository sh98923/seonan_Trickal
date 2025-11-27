using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionSlotDisplay : MonoBehaviour
{
    public Image characterImage;
    public TextMeshProUGUI nameText;

    // 슬롯 데이터 세팅 함수
    public void SetData(Sprite sprite, string characterName)
    {
        characterImage.sprite = sprite;
        nameText.text = characterName;
    }
}
