using UnityEngine;

/// <summary>
/// Class that takes care of the player's movement.
/// </summary>
public class Player : MonoBehaviour
{
    #region Variables

    [Header("Movement")]
    float speed = 4;
    float jump = 9.5f;
    [SerializeField] LayerMask groundMask = 0;
    
    [Header("Components")]
    [SerializeField] Rigidbody2D rb = null;
    [SerializeField] Animator anim = null;
    [SerializeField] SpriteRenderer sr = null;
    [SerializeField] AudioSource audioSource = null;

    [Header("Sounds")]
    [SerializeField] AudioSource hurtSound = null;
    #endregion

    /// <summary>
    /// Boolean that indicates through a Raycast if the player is touching the ground.
    /// </summary>
    /// <returns>True if the player is on the ground, false if not.</returns>
    bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(0, sr.bounds.extents.y + 0.01f, 0), Vector2.down, 0.1f, groundMask);

        return hit;
    }

    private void OnEnable()
    {
        transform.position = new Vector2(-6.3f, -5.4f);
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Player1Horizontal");

        Movement(h);

        Animation(h);

        Jump();

        if (Input.GetButtonDown("Cancel"))
        {
            GameManager1.manager.PauseGame();
        }
    }

    /// <summary>
    /// Function called to make the player move.
    /// </summary>
    /// <param name="h">Direction of movement of the player, positive if it is to the right and negative if it is to the left.</param>
    public void Movement(float h)
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime * h);

        if (h > 0)
        {
            transform.localScale = new Vector2(0.85f, 0.85f);
        }
        else if (h < 0)
        {
            transform.localScale = new Vector2(-0.85f, 0.85f);
        }
    }

    /// <summary>
    /// Function that activates character animations.
    /// </summary>
    /// <param name="h">Direction of movement of the player, positive if it is to the right and negative if it is to the left.</param>
    public void Animation(float h)
    {
        anim.SetBool("IsWalking", h != 0 && IsGrounded());
        anim.SetBool("IsJumping", !IsGrounded());
    }

    /// <summary>
    /// Function called to make the player jump.
    /// </summary>
    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) && IsGrounded())
        {
            rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
            audioSource.Play();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if ((other.gameObject.CompareTag("Game1/Enemy")) || (other.gameObject.CompareTag("Game1/Missile")))
        {
            gameObject.SetActive(false);
            GameManager1.manager.GameOver();

            hurtSound.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Game1/Coin"))
        {
            other.gameObject.SetActive(false);
            GameManager1.manager.UpdateScore();
        }
    }
}
