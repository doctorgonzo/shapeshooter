using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 13.3f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField, Min(0.01f)] private float singleShotCooldown = 0.01f;
    [SerializeField, Min(0.01f)] private float rapidFireCooldown = 0.08f;
    [SerializeField, Min(0.01f)] private float rapidFireDuration = 6.66f;
    [SerializeField, Min(1)] private int startingGunCount = 1;

    private Rigidbody2D playerRB;

    private bool rapidFire = false;
    private int activeGunCount;
    private float shotCooldownTimer;
    private Coroutine rapidFireCoroutine;
    private Transform[] hardPoints;

    private void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();

        hardPoints = new[]
        {
            transform.Find("HardPoint1"),
            transform.Find("HardPoint2"),
            transform.Find("HardPoint3")
        };

        activeGunCount = Mathf.Clamp(startingGunCount, 1, GetAvailableGunCount());
    }

    private void Update()
    {
        shotCooldownTimer -= Time.deltaTime;
        bool firePressed = Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1");
        bool fireHeld = Input.GetKey(KeyCode.Space) || Input.GetButton("Fire1");
        if (rapidFire ? fireHeld : firePressed)
        {
            TryShoot();
        }
    }
    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetAxis("Vertical") > 0)
        {
            playerRB.AddForce(Vector2.up * moveSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetAxis("Vertical") < 0)
        {
            playerRB.AddForce(Vector2.down * moveSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetAxis("Horizontal") < 0)
        {
            playerRB.AddForce(Vector2.left * moveSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetAxis("Horizontal") > 0)
        {
            playerRB.AddForce(Vector2.right * moveSpeed);
        }
    }
    private void TryShoot()
    {
        activeGunCount = Mathf.Clamp(activeGunCount, GetBaseGunCount(), GetAvailableGunCount());

        if (shotCooldownTimer > 0f)
        {
            return;
        }
        Shoot();
        shotCooldownTimer = rapidFire ? rapidFireCooldown : singleShotCooldown;
    }

    private void Shoot()
    {
        if (activeGunCount == 1 && FireFrom(hardPoints[0]))
        {
            return;
        }
        if (activeGunCount == 2 && hardPoints[1] != null && hardPoints[2] != null)
        {
            FireFrom(hardPoints[1]);
            FireFrom(hardPoints[2]);
            return;
        }
        int shotsFired = 0;
        for (int i = 0; i < hardPoints.Length && shotsFired < activeGunCount; i++)
        {
            if (FireFrom(hardPoints[i]))
            {
                shotsFired++;
            }
        }
    }

    private bool FireFrom(Transform hardPoint)
    {
        if (hardPoint == null)
        {
            return false;
        }
        Instantiate(bulletPrefab, hardPoint.position, transform.rotation);
        Player.Instance.shotsFired++;
        return true;
    }

    public void ActivateRapidFire()
    {
        rapidFire = true;

        if (rapidFireCoroutine != null)
        {
            StopCoroutine(rapidFireCoroutine);
        }

        rapidFireCoroutine = StartCoroutine(RapidFireTimer());
    }

    public void AddGun()
    {
        int availableGunCount = GetAvailableGunCount();
        if (activeGunCount >= availableGunCount)
        {
            return;
        }

        activeGunCount = Mathf.Min(activeGunCount + 1, availableGunCount);
        StartCoroutine(AddedGunTimer());
    }
    public void RemoveGun()
    {
        activeGunCount = Mathf.Clamp(activeGunCount - 1, GetBaseGunCount(), GetAvailableGunCount());
    }
    private IEnumerator AddedGunTimer()
    {
        yield return new WaitForSeconds(15.5f);
        RemoveGun();
    }
    private IEnumerator RapidFireTimer()
    {
        yield return new WaitForSeconds(rapidFireDuration);
        rapidFire = false;
        rapidFireCoroutine = null;
    }

    private int GetAvailableGunCount()
    {
        int count = 0;
        for (int i = 0; i < hardPoints.Length; i++)
        {
            if (hardPoints[i] != null)
            {
                count++;
            }
        }
        return Mathf.Max(1, count);
    }

    private int GetBaseGunCount()
    {
        return Mathf.Clamp(startingGunCount, 1, GetAvailableGunCount());
    }
}
