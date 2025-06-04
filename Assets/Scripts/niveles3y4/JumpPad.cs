using UnityEngine;

public class JumpPad2D : MonoBehaviour
{
    public float jumpForce = 21f;
    public AudioClip padSFX;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D rb2d = collision.GetComponent<Rigidbody2D>();
            if (rb2d != null)
            {
                rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                AudioManager.Instance.ReproducirSonido(padSFX);
            }
        }
    }
}
