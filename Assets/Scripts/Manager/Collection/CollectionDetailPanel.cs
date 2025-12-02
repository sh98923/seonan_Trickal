using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionDetailPanel : MonoBehaviour
{
    [Header("Introduction Panel")]
    public TextMeshProUGUI sentenceText;
    public TextMeshProUGUI explanationText;
    public TextMeshProUGUI favoriteText;
    public TextMeshProUGUI hateText;

    [Header("Information Panel")]
    public TextMeshProUGUI nameText;
    public Image layerImage;
    public Image typeImage;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI mpText;
    public TextMeshProUGUI atkText;

    [Header("Skill Panel")]
    public Image skillImage;
    public TextMeshProUGUI skillText;
    public Image ultImage;
    public TextMeshProUGUI ultText;
    private string FixNewLine(string s)
    {
        return s?.Replace("\\n", "\n");
    }
    public void SetData(CollectionData data)
    {
        // Text 직접 채우기
        nameText.text = data.KrName;

        sentenceText.text = FixNewLine(data.Sentence);
        explanationText.text = FixNewLine(data.Explanation);

        favoriteText.text = data.Favorite;
        hateText.text = data.Hate;
        hpText.text = data.Hp.ToString();
        mpText.text = data.Mp.ToString();
        atkText.text = data.Atk.ToString();

        skillText.text = FixNewLine(data.Skill);
        ultText.text = FixNewLine(data.Ult);

        // 아이콘 로드 (선택)
        layerImage.sprite = Resources.Load<Sprite>(data.Layer);     // ex: Icons/Position/Front
        typeImage.sprite = Resources.Load<Sprite>(data.AtkType);    // ex: Icons/AttackType/Melee
        skillImage.sprite = Resources.Load<Sprite>(data.SkillSpritePath);
        ultImage.sprite = Resources.Load<Sprite>(data.UltSpritePath);
    }
}
