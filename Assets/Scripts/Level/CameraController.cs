using System.Collections.Generic;
using DG.Tweening;
using Lean.Touch;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10f);
    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;
    public float zoomSpeed = 1.0f;
    public float minZoom = 2f;
    public float maxZoom = 10f;
    public float Size
    {
        get => cam.orthographicSize;
        set => cam.orthographicSize = Mathf.Clamp(value, 1f, 20f);
    }
    private void Awake() => cam = GetComponent<Camera>();
    private void Start()
    {
        SceneController.instance.cam = this;
        target = SceneController.instance.player.transform;
    }
    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
    }

    public void Shake(float duration = 0.2f, float strength = 0.5f, int vibrato = 20)
    {
        DOTween.Kill(transform);

        Vector3 startPos = transform.position;
        transform.DOShakePosition(duration, strength, vibrato, 90, false, true)
                 .SetEase(Ease.OutQuad)
                 .OnComplete(() => transform.position = new Vector3(transform.position.x, transform.position.y, startPos.z));
    }

    private void HandleGesture(List<LeanFinger> fingers)
    {
        if (fingers.Count == 2)
        {
            var pinchScale = LeanGesture.GetPinchScale(fingers);
            float newSize = cam.orthographicSize / pinchScale;
            cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
    }
    private void OnEnable()
    {
        LeanTouch.OnGesture += HandleGesture;
    }

    private void OnDisable()
    {
        LeanTouch.OnGesture -= HandleGesture;
    }
}
