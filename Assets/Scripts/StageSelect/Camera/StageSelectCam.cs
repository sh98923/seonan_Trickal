using UnityEngine;

public class StageSelectCam : MonoBehaviour
{
    private Camera _cam;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        _cam.orthographicSize = 4;
    }
}
