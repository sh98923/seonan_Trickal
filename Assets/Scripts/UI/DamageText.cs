using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    public void Initialize(int damage)
    {
        text.text = damage.ToString();
        gameObject.SetActive(true);
    }
}
