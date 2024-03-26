using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class PrismEnemy : MonoBehaviour
{
    enum enemyMode { PHYSICAL, RANGED }
    [SerializeField]
    enemyMode currentMode;

    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip deathClip;
    [SerializeField]
    private Projectile bulletPrefab;
    [SerializeField]
    private Transform body;
    [SerializeField]
    private Transform fireLocation;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private bool dieInstantly = false;

    bool isDead = false;

    private float fireCount = 0f;
    [SerializeField]
    private float fireRate = 1f;
    public bool IsDead
    {
        get { return isDead; }
    }

    [SerializeField]
    private float followingDistance = 10f;

    private float kamikazeDistance = 5f;
    [SerializeField]
    private float movementSpeed = 3f;

    private bool circling = true;

    [SerializeField]
    private int health = 20;


    private bool hunting = false;
    private bool wasNudged = false;

    private void Update()
    {
        Player player = FindObjectOfType<Player>();
        //We want the speaker to always look at the player
        //Always keep it on the same y level as the player
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        if (health <= 0)
        {
            Death();
        }
        if (player && hunting)
        {
            transform.LookAt(new Vector3(player.transform.position.x, 0.0f,player.transform.position.z) );

            switch (currentMode)
            {
                case enemyMode.PHYSICAL:
                    //Fire();
                    if (wasNudged)
                        return;
                    if (circling)
                    {
                        transform.Translate(Vector3.right * Time.deltaTime * movementSpeed);
                    }
                    //distance to player check.
                    if (Vector3.Distance(player.transform.position, transform.position) >= followingDistance)
                    {
                        transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed);
                    }
                    else if (Vector3.Distance(player.transform.position, transform.position) <= followingDistance)
                    {
                        transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed * -1);
                    }
                    break;
                case enemyMode.RANGED:
                    if (Vector3.Distance(player.transform.position, transform.position) >= followingDistance)
                    {
                        //moves faster when not firing?
                        if(!wasNudged)
                            transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed * 2);
                        Fire();
                    }
                    //else if (Vector3.Distance(player.transform.position, transform.position) >= followingDistance)
                    //{
                    //    Explode();
                    //}
                    break;
            }
        }
        else
        {
            return;
        }
    }

    private void Start()
    {
        //LevelManager.Instance.RegisterEnemy(this);
        //StartCoroutine("IdleSounds");
    }

    public void Reset()
    {
        isDead = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead)
        {
            return;
        }
        bool hitPlayer = other.CompareTag("Player");
        if (other.CompareTag("PlayerBullet") || hitPlayer)
        {
            if (hitPlayer)
                StartCoroutine(WasHitByPlayer());
            if (dieInstantly)
            {
                health = 0;
            }
            else
            {
                health -= 5;
            }
        }
        if (other.CompareTag("EnemyTrigger"))
        {
            hunting = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        bool hitPlayer = collision.transform.CompareTag("Player");
        if (hitPlayer)
            StartCoroutine(WasHitByPlayer());
    }
    private void FireBullet()
    {
        Vector3 fireDirection = fireLocation.position - body.position;
        fireDirection.y = 0;
        fireDirection.Normalize();

        //GameObject bulletGO = Instantiate(bulletPrefab, fireLocation.position, Quaternion.identity);
        GameObject eBulletGO = ObjectPoolManager.Instance.GetPooledObject(ObjectPoolManager.PoolTypes.enemyBullet);
        eBulletGO.transform.position = fireLocation.position;
        eBulletGO.transform.rotation = Quaternion.identity;
        //sounds.FireGun();
        Projectile eBullet = eBulletGO.GetComponent<Projectile>();
        //rifleBurstEffect.Play();
        eBullet.FireDirection = fireDirection;
        eBulletGO.SetActive(true);
    }

    private void Fire()
    {
        fireCount += Time.deltaTime;

        if (fireCount >= fireRate)
        {
            fireCount = 0f;
            FireBullet();
        }
    }

    private void Explode()
    {
        Player player = FindObjectOfType<Player>();
        player.TakeDamage(10);
    }

    void Death()
    {
        audioSource.PlayOneShot(deathClip);
        isDead = true;
        gameObject.SetActive(false);
    }
    IEnumerator WasHitByPlayer()
    {
        wasNudged = true;
        yield return new WaitForSeconds(2);
        wasNudged = false;
        rb.velocity = Vector3.zero;
    }
}