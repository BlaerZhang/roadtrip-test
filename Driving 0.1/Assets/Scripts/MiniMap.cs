using System;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    [Header("Transforms")]
    public Transform carTransform;
    public Transform destinationTransform;
    public Vector2 centerPos;

    [Header("Icons")] 
    public GameObject iconPrefab;
    public Sprite carIcon;
    public Sprite destinationIcon;
    
    [Header("Map Settings")]
    [Tooltip("多少个Unity单位对应地图的完整宽度")]
    public float worldMapScale = 100f;
    [Tooltip("图标大小(单位:像素)")]
    public Vector2 iconSize = new Vector2(20, 20);

    [Header("Car States")]
    private Rigidbody2D _carRigidbody;
    private bool _isCarDriving = false;
    public static Action onCarStart;
    public static Action onCarStop;

    // References for icons
    private GameObject _carIconObj;
    private GameObject _destinationIconObj;
    private RectTransform _mapRect;

    private void OnEnable()
    {
        onCarStart += UpdateMap;
        onCarStop += () => GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        onCarStart += () => GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -GetComponent<RectTransform>().rect.height);
    }

    private void OnDisable()
    {
        onCarStart -= UpdateMap;
        onCarStop -= () => GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        onCarStart -= () => GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -GetComponent<RectTransform>().rect.height);
    }

    void Start()
    {
        _carRigidbody = carTransform.GetComponentInChildren<Rigidbody2D>();
        _mapRect = GetComponent<RectTransform>();
        
        // Create icons
        _carIconObj = CreateIcon(carIcon);
        _destinationIconObj = CreateIcon(destinationIcon);
        
        UpdateMap();
    }
    
    void Update()
    {
        if (!_isCarDriving)
        {
            if (_carRigidbody.linearVelocity.magnitude > 0.01f)
            {
                _isCarDriving = true;
                onCarStart?.Invoke();
            }
        }

        if (_isCarDriving)
        {
            if (_carRigidbody.linearVelocity.magnitude <= 0.01f)
            {
                _isCarDriving = false;
                onCarStop?.Invoke();
            }
            else
            {
                UpdateMap();
            }
        }
    }

    private GameObject CreateIcon(Sprite iconSprite)
    {
        // 获取地图的RectTransform作为父物体
        RectTransform mapTransform = transform as RectTransform;
        
        // 创建图标并设置父物体
        GameObject icon = Instantiate(iconPrefab, mapTransform);
        
        // 设置UI组件
        Image image = icon.GetComponent<Image>();
        if (image == null)
        {
            image = icon.AddComponent<Image>();
        }
        image.sprite = iconSprite;
        image.raycastTarget = false;
        
        // 配置RectTransform
        RectTransform rectTransform = icon.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.localScale = Vector3.one;
        rectTransform.sizeDelta = iconSize;
        
        // 确保在最上层显示
        rectTransform.SetAsLastSibling();
        
        return icon;
    }

    void UpdateMap()
    {
        if (_carIconObj == null || _destinationIconObj == null || _mapRect == null) return;

        // Calculate relative positions from center
        Vector2 carRelativePos = (Vector2)carTransform.position - centerPos;
        Vector2 destRelativePos = (Vector2)destinationTransform.position - centerPos;

        // Convert world space to minimap space
        float mapScale = _mapRect.rect.width / worldMapScale;
        Vector2 carUIPos = carRelativePos * mapScale;
        Vector2 destUIPos = destRelativePos * mapScale;

        // Clamp positions to map bounds
        float halfWidth = _mapRect.rect.width * 0.5f;
        float halfHeight = _mapRect.rect.height * 0.5f;
        carUIPos = new Vector2(
            Mathf.Clamp(carUIPos.x, -halfWidth, halfWidth),
            Mathf.Clamp(carUIPos.y, -halfHeight, halfHeight)
        );
        destUIPos = new Vector2(
            Mathf.Clamp(destUIPos.x, -halfWidth, halfWidth),
            Mathf.Clamp(destUIPos.y, -halfHeight, halfHeight)
        );

        // Update icon positions
        _carIconObj.GetComponent<RectTransform>().anchoredPosition = carUIPos;
        _destinationIconObj.GetComponent<RectTransform>().anchoredPosition = destUIPos;

        // Update car icon rotation to match actual car rotation
        _carIconObj.transform.rotation = Quaternion.Euler(0, 0, carTransform.eulerAngles.z);
    }
}