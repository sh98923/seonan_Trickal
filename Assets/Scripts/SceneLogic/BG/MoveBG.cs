using UnityEngine;

public class MoveBG : MonoBehaviour
{
    private Material _bgMaterial;

    private const float _bgSpeed = 0.05f;

    private void Awake()
    {
        _bgMaterial = GetComponent<SpriteRenderer>().material;

    }

    private void Update()
    {
        if (!InGameManager.Instance.IsGameStart)
        {
            return;
        }

        if (InGameManager.Instance.CanBGMove)
        {
            Vector2 offSet = _bgMaterial.mainTextureOffset;
            offSet.x += _bgSpeed * Time.deltaTime;
            _bgMaterial.mainTextureOffset = offSet;
        }
    } 
}
