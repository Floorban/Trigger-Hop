using Lean.Touch;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PinchZoom : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private Image img;
    [SerializeField] private GameObject child1, child2, child3;
    [SerializeField] private GameObject pinchGuide;
    public float zoomSpeed = 0.8f;
    public float minZoom = 3f;
    public float maxZoom = 10f;
    private bool isPinching = false;
    private Color originalColor, transparent;

    private void Awake()
    {
        cam = FindFirstObjectByType<Camera>();
        originalColor = img.color;
    }
    private void HandleGesture(List<LeanFinger> fingers)
    {
        if (!SceneController.instance.isPaused)
        {
            ResetImageAlpha();
            return;
        }

        if (fingers.Count == 2)
        {
            isPinching = true;

            child1.SetActive(false);
            child2.SetActive(false);
            child3.SetActive(false);
            pinchGuide.SetActive(true);

            // Fade image while pinching
            if (transparent != null)
            {
                transparent = img.color;
                transparent.a = 0.2f;
                img.color = transparent;
            }

            // Apply zoom
            var pinchScale = LeanGesture.GetPinchScale(fingers);
            float newSize = cam.orthographicSize / pinchScale;
            cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
        else
        {
            if (isPinching)
            {
                isPinching = false;
                ResetImageAlpha();
            }
        }
    }
    private void ResetImageAlpha()
    {
        img.color = originalColor;
        child1.SetActive(true);
        child2.SetActive(true);
        child3.SetActive(true);
        pinchGuide.SetActive(false);
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
