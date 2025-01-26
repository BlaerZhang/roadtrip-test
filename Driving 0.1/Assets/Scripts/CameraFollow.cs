using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private float speedOffsetMultiplier = 0.3f;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -10);
    
    private Vector3 currentVelocity;
    private Rigidbody2D targetRb;
    private Vector3 targetPosition;

    private void Start()
    {
        targetRb = target.GetComponent<Rigidbody2D>();
        targetPosition = transform.position;
    }

    private void LateUpdate()
    {
        if (!target) return;

        // 计算目标位置
        Vector3 desiredPosition = target.position + offset;
        desiredPosition.x += targetRb.linearVelocity.x * speedOffsetMultiplier;

        // 更平滑的位置更新
        targetPosition = Vector3.SmoothDamp(
            targetPosition,
            desiredPosition,
            ref currentVelocity,
            smoothTime,
            Mathf.Infinity,
            Time.deltaTime
        );

        transform.position = targetPosition;
    }
}