using UnityEngine;

public class RangedAttackPreview : MonoBehaviour
{
    private static readonly int CutoffValue = Shader.PropertyToID("_CutoffValue");
    [SerializeField] private GameObject previewArch; // The object displaying the preview.
    [SerializeField] private float previewTime = 0.5f; // Duration of the preview effect.
    private bool _previewActive; // Tracks if the preview is currently active.
    private float _timer; // Tracks time elapsed during the preview.
    private Material _material; // Reference to the material to manipulate.
    private Transform _playerTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created.
    void Start()
    {
        _material = previewArch.GetComponent<SpriteRenderer>().material;
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        previewArch.SetActive(false);
        if (_material == null)
        {
            Debug.LogError("Material not found on the assigned preview object.");
        }
    }

    // Starts the preview effect.
    public void StartPreview()
    {
        _previewActive = true;
        _timer = 0f;
        _material.SetFloat(CutoffValue, 0f); // Start the effect at 0.
        previewArch.SetActive(true); // Ensure the preview object is visible.
    }

    // Update is called once per frame.
    void Update()
    {
        if (_previewActive)
        {
            RotateTowardsPlayer();

            _timer += Time.deltaTime;

            // Smoothly interpolate the _CutoffValue from 0 to 1 over previewTime.
            float progress = Mathf.Clamp01(_timer / previewTime);
            _material.SetFloat(CutoffValue, progress);

            // End the preview if the timer exceeds the preview time.
            if (_timer >= previewTime)
            {
                EndPreview();
            }
        }
    }

    private void RotateTowardsPlayer()
    {
        var direction = _playerTransform.position - transform.position;
        var rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotationZ);
    }

    // Ends the preview effect.
    private void EndPreview()
    {
        _previewActive = false;
        previewArch.SetActive(false); // Hide the preview object when done.
    }
}