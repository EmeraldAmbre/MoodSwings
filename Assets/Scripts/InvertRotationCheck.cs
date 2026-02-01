using UnityEngine;

public class InvertRotationCheck : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision OK");
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        rb.freezeRotation = !rb.freezeRotation;
    }
}
