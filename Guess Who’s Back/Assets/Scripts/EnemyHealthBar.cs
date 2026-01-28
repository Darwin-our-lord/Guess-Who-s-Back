using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image fillImage;
    [SerializeField] private Canvas canvas;

    [Header("Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 1.0f, 0);
    [SerializeField] private bool scaleWithHealth = true;
    [SerializeField] private float minScale = 0.1f;
    [SerializeField] private float maxScale = 0.5f;

    private Enemy targetEnemy;
    private Camera mainCamera;
    private RectTransform rectTransform;

    private void Awake()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
    }

    public void Initialize(Enemy enemy)
    {
        targetEnemy = enemy;
        UpdateHealthBar();
    }

    private void Update()
    {
        if (targetEnemy == null || targetEnemy.HasDied)
        {
            Hide();
            return;
        }

        UpdatePosition();
        UpdateHealthBar();
    }

    private void UpdatePosition()
    {
        if (targetEnemy == null || mainCamera == null) return;

        Vector3 worldPos = targetEnemy.transform.position + offset;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

        rectTransform.position = screenPos;
    }

    private void UpdateHealthBar()
    {
        if (targetEnemy != null && fillImage != null)
        {
            float healthPercent = targetEnemy.CurrentHealth / targetEnemy.MaxHealth;
            fillImage.fillAmount = healthPercent;

            if (scaleWithHealth)
            {
                float scale = Mathf.Lerp(minScale, maxScale, healthPercent);
                rectTransform.localScale = new Vector3(scale, 1f, 1f);
            }

            if (healthPercent > 0.6f)
                fillImage.color = Color.green;
            else if (healthPercent > 0.3f)
                fillImage.color = Color.yellow;
            else
                fillImage.color = Color.red;
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);

    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}