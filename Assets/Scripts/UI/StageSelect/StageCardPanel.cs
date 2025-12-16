using UnityEngine;
using UnityEngine.UI;

public class StageCardPanel : CardPanel
{
    private StagePlayerSpawn _spawnParent;

    private Image _outlineImage;

    private const float _maxHue = 1.0f;

    private float _hue = 0.0f;
    private float _speed = 0.35f; // 무지개 속도
    private float _saturation = 0.5f; // 채도 낮춰 파스텔
    private float _value = 0.95f;     // 밝기 높여 부드럽게

    // 녹색 HSV 값
    /*private const float GreenHue = 0.33f;    // 녹색
    private const float GreenSaturation = 0.7f;
    private const float GreenValue = 0.7f;*/

    private void Awake()
    {
        base.Awake();

        GameManager.Instance.EnableInScenes(this, SceneName.StageSelectScene);
        // 녹색 적용
        //_outlineImage.color = Color.HSVToRGB(GreenHue, GreenSaturation, GreenValue);
    }

    private void Start()
    {
        base.Start();

        _spawnParent = GameObject.Find("SpawnPlayer").GetComponent<StagePlayerSpawn>();
        _outlineImage = _cardChildren[(int)CardUI.OutLineColorImage].GetComponent<Image>();
    }

    private void Update()
    {
        _hue += Time.deltaTime * _speed;

        if (_hue > _maxHue)
        {
            _hue -= _maxHue;
        }

        _outlineImage.color = Color.HSVToRGB(_hue, _saturation, _value);
    }

    protected override void InitUI()
    {
        base.InitUI();
        _cardChildren[(int)CardUI.CostImage].gameObject.SetActive(false);
    }

    protected override void HandleClick()
    {
        if (!CanDeploy(out Vector3 spawnPos))
        {
            OutLineActive(false);
            return;
        }

        OutLineActive(true);

        _spawnParent.SpawnPlayerAtPosition(_playerData, spawnPos);

        GameManager.Instance.AddUnit(_playerData, spawnPos);
    }

    private void OutLineActive(bool isActive)
    {
        _outlineImage.gameObject.SetActive(isActive);
    }

    private bool CanDeploy(out Vector3 spawnPos)
    {
        spawnPos = Vector3.zero;

        // 다시 누르면 제거하기
        if (RemoveDeployed(_playerData))
        {
            GameManager.Instance.RemoveUnit(_playerData);
            Debug.LogWarning($"{_playerData.EngName}를 제거함.");
            return false;
        }

        if (!TryGetSpawnPosition(_playerData, out spawnPos))
        {
            Debug.LogWarning("스폰할 자리가 없습니다.");
            return false;
        }

        return true;
    }

    private bool RemoveDeployed(PlayerData playerData)
    {
        return _spawnParent.CheckAndRemoveDeployed(playerData);
    }

    private bool TryGetSpawnPosition(PlayerData playerData, out Vector3 spawnPos)
    {
        spawnPos = _spawnParent.SetPlayerPos(playerData);
        if (spawnPos != Vector3.zero)
        {
            spawnPos.z = 0.0f;
            return true;
        }
        return false;
    }
}