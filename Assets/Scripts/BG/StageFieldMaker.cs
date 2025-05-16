using System.Collections.Generic;
using UnityEngine;

public class BG : MonoBehaviour
{
    [SerializeField]
    private Sprite _bgSprite;

    [SerializeField]
    private int _bgNum = 2;

    [SerializeField]
    private float _bgSpeed = 1.0f;

    private List<GameObject> _bgObjects = new List<GameObject>();
    private Vector2 worldScreenLeft;

    private void Awake()
    {
        GameObject bgPrefab = Resources.Load<GameObject>("Prefabs/BG/StageFieldParts");

        _bgObjects.Capacity = _bgNum;

        for(int i = 0; i < _bgNum; i++)
        {
            GameObject bgObject = Instantiate(bgPrefab, transform);
            SpriteRenderer renderer = bgObject.GetComponent<SpriteRenderer>();
            renderer.sprite = _bgSprite;
            renderer.flipX = (i % 2) == 1;

            Vector2 pos = Vector2.zero;
            pos.x = renderer.bounds.size.x * i;

            bgObject.transform.localPosition = pos;

            _bgObjects.Add(bgObject);
        }

        worldScreenLeft = Camera.main.ScreenToWorldPoint(Vector2.zero);
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < _bgObjects.Count; i++)
        {
            Transform bg = _bgObjects[i].transform;
            SpriteRenderer renderer = bg.GetComponent<SpriteRenderer>();

            bg.Translate(Vector2.left * _bgSpeed * Time.deltaTime);

            if (renderer.bounds.max.x < worldScreenLeft.x)
            {
                Vector2 pos = bg.localPosition;
                pos.x += renderer.bounds.size.x * (_bgNum - 1);
                bg.localPosition = pos;
            }
        }
    }
}
