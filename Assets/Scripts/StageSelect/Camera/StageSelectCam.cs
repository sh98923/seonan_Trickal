using UnityEngine;

public class StageSelectCam : MonoBehaviour
{
    private Camera _cam;
    private readonly float _zoomInSize = 4.5f;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        _cam.orthographicSize = _zoomInSize;
    }
}
