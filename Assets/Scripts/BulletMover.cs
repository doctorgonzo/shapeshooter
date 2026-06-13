using System.Collections;
using UnityEngine;

public class BulletMover : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 1.3f;
    [SerializeField] private Rigidbody2D bulletRB;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] float speed = 10f;
    [SerializeField] float turnSpeed = 200f;
    private Player player;
    public bool isSeeking = false;
    private void Start()
    {
        StartCoroutine(KillTimer());
        bulletRB.AddForce(transform.up * bulletSpeed, ForceMode2D.Impulse);
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }
    private IEnumerator KillTimer()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
    void FixedUpdate()
    {
        if (!isSeeking) return;

    Vector2 dirToPlayer = ((Vector2)player.transform.position - bulletRB.position).normalized;
    float angle = Vector2.SignedAngle(bulletRB.linearVelocity.normalized, dirToPlayer);
    float turn = Mathf.Clamp(angle, -turnSpeed * Time.fixedDeltaTime, turnSpeed * Time.fixedDeltaTime);
    bulletRB.linearVelocity = Quaternion.Euler(0, 0, turn) * bulletRB.linearVelocity.normalized * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Destroy the bullet on collision with any object
        Destroy(gameObject);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                Player.Instance.shotsHit++;
                enemy.TakeDamage(1); // Assuming the bullet does 1 damage
            }
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(1);
            }
        }
    }
}
