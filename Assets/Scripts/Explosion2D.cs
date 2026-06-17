using UnityEngine;

public class Explosion2D : MonoBehaviour
{
    [SerializeField] private float force = 6f;          // impulse magnitude
    [SerializeField, Range(0f, 1f)] private float jitter = 0.35f; // direction/force randomness
    [SerializeField] private float spin = 8f;           // random torque
    [SerializeField] private float gravityScale = 0f;   // 0 = floaty burst; >0 = arcs then fall
    [SerializeField] private float lifetime = 1.25f;

    private void Start()
    {
        Vector2 center = transform.position;

        foreach (Rigidbody2D rb in GetComponentsInChildren<Rigidbody2D>())
        {
            rb.gravityScale = gravityScale;

            Vector2 dir = (Vector2)rb.transform.position - center;
            if (dir.sqrMagnitude < 0.0001f)            // fragments stacked dead-center
                dir = Random.insideUnitCircle;
            dir.Normalize();

            // nudge the angle so 4 pieces don't fly out as a perfect '+'
            dir = Quaternion.Euler(0, 0, Random.Range(-jitter, jitter) * 45f) * dir;

            float mag = force * (1f + Random.Range(-jitter, jitter));
            rb.AddForce(dir * mag, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-spin, spin), ForceMode2D.Impulse);
        }

        Destroy(gameObject, lifetime);
    }
}