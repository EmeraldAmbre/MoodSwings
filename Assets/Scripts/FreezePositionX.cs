using UnityEngine;

public class FreezePositionX : MonoBehaviour
{
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision OK");
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezePositionX;
    }
}
