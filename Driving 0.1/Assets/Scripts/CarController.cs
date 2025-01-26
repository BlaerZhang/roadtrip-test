using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private float accelerationForce = 10f;
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float verticalSpeed = 5f;
    
    private Rigidbody2D rb;
    private Vector2 movement;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = 1f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void Update()
    {
        // 计算移动输入
        movement = Vector2.zero;
        
        if (Input.GetKey(KeyCode.Space))
        {
            movement.x = accelerationForce;
        }

        if (Input.GetKey(KeyCode.W)) movement.y = verticalSpeed;
        if (Input.GetKey(KeyCode.S)) movement.y = -verticalSpeed;
    }

    private void FixedUpdate()
    {
        // 应用水平力
        rb.AddForce(Vector2.right * movement.x);

        // 限制速度
        rb.linearVelocity = new Vector2(
            Mathf.Clamp(rb.linearVelocity.x, 0, maxSpeed),
            movement.y
        );
    }
}