using UnityEngine;

public class SeekingBullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameObject player;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = Player.Instance != null ? Player.Instance.gameObject : null;
    }
    private void Update()
    {
        // Player is scene-local now — re-acquire if needed and skip homing when there's no ship.
        if (player == null)
        {
            player = Player.Instance != null ? Player.Instance.gameObject : null;
        }
        if (player == null)
        {
            return;
        }
        transform.LookAt(player.transform);
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, 20);
    }
}
