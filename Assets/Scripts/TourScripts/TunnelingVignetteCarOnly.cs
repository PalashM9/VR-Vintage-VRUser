using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class TunnelingVignetteCarOnly : MonoBehaviour
{
    [Header("References")]
    public Camera headCamera;
    public CarMovementController carController;

    [Header("Vignette Behavior")]
    [Tooltip("Aperture when not moving (1 = fully open).")]
    [Range(0f, 1f)] public float apertureAtRest = 1.0f;

    [Tooltip("Aperture at maximum speed (smaller = stronger tunneling).")]
    [Range(0f, 1f)] public float apertureAtMaxSpeed = 0.45f;

    [Tooltip("Edge softness of the vignette.")]
    [Range(0.001f, 0.75f)] public float softness = 0.18f;

    [Tooltip("How quickly the aperture changes.")]
    public float apertureLerpSpeed = 10f;

    [Tooltip("Minimum speed before vignette activates.")]
    public float activationSpeedThreshold = 0.05f;

    [Header("Visual")]
    public Color vignetteColor = new Color(0, 0, 0, 1);

    private Canvas _canvas;
    private Image _image;
    private Material _mat;

    private float _currentAperture = 1f;

    private void OnEnable()
    {
        var netObj = GetComponentInParent<NetworkObject>();
        if (netObj != null && !netObj.IsOwner)
        {
            enabled = false;
            return;
        }

        EnsureOverlay();

        _currentAperture = apertureAtRest;
        ApplyToMaterial(_currentAperture);
    }

    private void Update()
    {
        if (_mat == null || headCamera == null || carController == null)
        {
            SetApertureInstant(apertureAtRest);
            return;
        }

        if (!carController.IsDriving)
        {
            SetTargetAperture(apertureAtRest);
            return;
        }

        float speed = carController.CurrentSpeed;

        if (speed <= activationSpeedThreshold)
        {
            SetTargetAperture(apertureAtRest);
            return;
        }

        float speed01 = Mathf.Clamp01(speed / carController.MaxSpeed);
        float targetAperture = Mathf.Lerp(apertureAtRest, apertureAtMaxSpeed, speed01);

        SetTargetAperture(targetAperture);
    }

    private void SetTargetAperture(float target)
    {
        _currentAperture = Mathf.Lerp(
            _currentAperture,
            target,
            apertureLerpSpeed * Time.deltaTime
        );

        ApplyToMaterial(_currentAperture);
    }

    private void SetApertureInstant(float value)
    {
        _currentAperture = value;
        ApplyToMaterial(_currentAperture);
    }

    private void ApplyToMaterial(float aperture)
    {
        if (_mat == null) return;

        _mat.SetFloat("_Aperture", Mathf.Clamp01(aperture));
        _mat.SetFloat("_Softness", Mathf.Max(0.001f, softness));
        _mat.SetColor("_Color", vignetteColor);
    }

    private void EnsureOverlay()
    {
        if (headCamera == null)
            headCamera = GetComponentInChildren<Camera>(true);

        if (headCamera == null) return;

        if (_canvas == null)
        {
            var canvasGO = new GameObject("TunnelingVignetteCanvas");
            canvasGO.transform.SetParent(headCamera.transform, false);

            _canvas = canvasGO.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _canvas.worldCamera = headCamera;
            _canvas.sortingOrder = short.MaxValue;

            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        if (_image == null)
        {
            var imageGO = new GameObject("Vignette");
            imageGO.transform.SetParent(_canvas.transform, false);

            _image = imageGO.AddComponent<Image>();
            var rt = _image.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        if (_mat == null)
        {
            Shader shader = Shader.Find("UI/VignetteMask");
            if (shader == null)
            {
                Debug.LogWarning("UI/VignetteMask shader not found.");
                return;
            }

            _mat = new Material(shader);
            _image.material = _mat;

            ApplyToMaterial(apertureAtRest);
        }
    }
}

