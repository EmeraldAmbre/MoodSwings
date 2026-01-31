using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PushableBlock : MonoBehaviour
{
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Push(Vector2 direction, float force)
    {
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public void Stop()
    {
        _rb.linearVelocity = Vector2.zero;
    }
}
