using UnityEngine;

public class SeekingBullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private GameObject player;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player");
    }
    private void Update()
    {
        transform.LookAt(player.transform);
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, 20);
    }
}
