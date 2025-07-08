using UnityEngine;

public class Scaling : MonoBehaviour
{
    private SpriteRenderer[] _sprites;

    private void Awake()
    {
        _sprites = GetComponentsInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        ResizeSky(_sprites);
    }

    private void ResizeSky(SpriteRenderer[] bgs)
    {
        float screenRatio = (float)Screen.width / Screen.height;

        for (int i = 0; i < bgs.Length; i++)
        {
            float targetRatio = bgs[i].sprite.bounds.size.x / bgs[i].sprite.bounds.size.y;

            Vector3 scale = bgs[i].transform.localScale;

            if (screenRatio >= targetRatio)
            {
                scale.x = screenRatio / targetRatio;
                scale.y = 1.0f;
            }
            else
            {
                scale.x = 1.0f;
                scale.y = targetRatio / screenRatio;
            }

            bgs[i].transform.localScale = scale;
        }
    }
}
