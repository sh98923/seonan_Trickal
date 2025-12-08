using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionDetailPanel : MonoBehaviour
{
    [Header("Introduction Panel")]
    [SerializeField] private TextMeshProUGUI _sentenceText;
    [SerializeField] private TextMeshProUGUI _explanationText;
    [SerializeField] private TextMeshProUGUI _favoriteText;
    [SerializeField] private TextMeshProUGUI _hateText;

    [Header("Information Panel")]
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Image _layerImage;
    [SerializeField] private Image _typeImage;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _mpText;
    [SerializeField] private TextMeshProUGUI _atkText;

    [Header("Skill Panel")]
    [SerializeField] private Image _skillImage;
    [SerializeField] private TextMeshProUGUI _skillText;
    [SerializeField] private Image _ultImage;
    [SerializeField] private TextMeshProUGUI _ultText;
    private string FixNewLine(string s)
    {
        return s?.Replace("\\n", "\n");
    }
    public void SetData(CollectionData data)
    {
        // Text 직접 채우기
        _nameText.text = data.KrName;

        _sentenceText.text = FixNewLine(data.Sentence);
        _explanationText.text = FixNewLine(data.Explanation);

        _favoriteText.text = data.Favorite;
        _hateText.text = data.Hate;
        _hpText.text = data.Hp.ToString();
        _mpText.text = data.Mp.ToString();
        _atkText.text = data.Atk.ToString();

        _skillText.text = FixNewLine(data.Skill);
        _ultText.text = FixNewLine(data.Ult);

        // 아이콘 로드 (선택)
        _layerImage.sprite = Resources.Load<Sprite>(data.Layer);     // ex: Icons/Position/Front
        _typeImage.sprite = Resources.Load<Sprite>(data.AtkType);    // ex: Icons/AttackType/Melee
        _skillImage.sprite = Resources.Load<Sprite>(data.SkillSpritePath);
        _ultImage.sprite = Resources.Load<Sprite>(data.UltSpritePath);
    }
}
