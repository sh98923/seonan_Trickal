using UnityEngine;

public class CharacterHp : MonoBehaviour
{
    private GameObject _hpBar;
    private Transform _hpBarTransform;
   
    private void Awake()
    {
        GameManager.Instance.EnableInScenes(this, SceneName.InGameScene);
    }

    private void Start()
    {
        GameObject hpBarPrefab = Resources.Load<GameObject>("Prefabs/UI/StatusBar/HpBar");
        Transform hpBarUITransform = GameObject.Find("CharacterStatusPanel").transform;

        _hpBarTransform = transform.Find("HpBarAnchor");
        _hpBar = Instantiate(hpBarPrefab, hpBarUITransform);

        _hpBar.transform.position = _hpBarTransform.position;
    }

    private void Update()
    {
        //_hpBar.transform.position = _hpBarTransform.position;
    }
}