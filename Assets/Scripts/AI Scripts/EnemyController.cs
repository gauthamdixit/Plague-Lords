using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    private int enemyHealth;
    public float dieTimer;
    public bool playdeathnow, startAttack, isDead;

    public AudioClip hitsound,deathsound;
    private AudioSource zombiesource;
    private Animator enemyAnim;
    private GameObject Player, arAmmoBox, gAmmoBox, healthBox;
    private IEnumerator coroutine;

    void Start()
    {
        dieTimer = 0;
        enemyAnim = GetComponent<Animator>();
        SetBooleans();
        zombiesource = GetComponent<AudioSource>();
        Player = GameObject.FindGameObjectWithTag("Player");
        SetLootDrops();
        enemyHealth = 150;
        coroutine = DamagePlayer(25);
    }

    public void AttackPlayer()
    {
        transform.LookAt(new Vector3(Player.transform.position.x, 0f, Player.transform.position.z));
        enemyAnim.SetBool("walking", false);
        enemyAnim.SetBool("attacking", true);
    }

    public void DropLoot()
    {
        float spawnChance = 30;
        float rand = Random.Range(0, 100);
        if (rand < spawnChance)
        {
            if (rand < 10)
            {
                Instantiate(arAmmoBox, transform.position + new Vector3(0, 0.25f, 0), Quaternion.identity);
            }
            else if (rand < 20)
            {
                Instantiate(gAmmoBox, transform.position + new Vector3(0, 0.25f, 0), Quaternion.identity);
            }
            else
            {
                Instantiate(healthBox, transform.position + new Vector3(0, 0.10f, 0), Quaternion.identity);
            }
        }
        Destroy(gameObject);
    }

    public void EnemyAI()
    {
        if (Vector3.Distance(Player.transform.position, transform.position) > 2.5)
        {
            startAttack = true;
            FollowPlayer();
            StopCoroutine(coroutine);
        }
        else
        {
            AttackPlayer();
            if (startAttack)
                StartCoroutine(coroutine);
        }
    }

    public void EnemyManager()
    {
        if (enemyHealth <= 0)
        {
            StartDeath();
            dieTimer += Time.deltaTime;
            if (dieTimer > 3)
            {
                DropLoot();
            }
        }
        else
        {
            EnemyAI();
        }
    }

    public void FollowPlayer()
    {
        enemyAnim.SetBool("walking", true);
        enemyAnim.SetBool("attacking", false);
    }

    public void PlayDeath()
    {
        if (playdeathnow)
        {
            Player.transform.GetChild(1).GetComponent<AudioSource>().PlayOneShot(deathsound);
        }
        playdeathnow = false;
    }

    public void SetLootDrops()
    {
        arAmmoBox = Resources.Load<GameObject>("AmmoBox");
        gAmmoBox = Resources.Load<GameObject>("AmmoBox");
        healthBox = Resources.Load<GameObject>("HealthBox");
    }

    public void SetBooleans()
    {
        enemyAnim.SetBool("walking", true);
        enemyAnim.SetBool("attacking", false);
        enemyAnim.SetBool("dying", false);
        startAttack = true;
        isDead = false;
        playdeathnow = true;
    }

    public void StartDeath()
    {
        GetComponent<Collider>().enabled = false;
        isDead = true;
        PlayDeath();
        enemyAnim.SetBool("dying", true);
        enemyAnim.SetBool("walking", false);
        enemyAnim.SetBool("attacking", false);
    }

    public void TakeDamage(int damage)
    {
        //To Do add Hit sound and possiblity to drop item
        enemyHealth -= damage;
    }

    IEnumerator DamagePlayer(int damage)
    {
        
        while (true)
        {
            startAttack = false;
            Player.GetComponent<PlayerHealthManager>().GiveDamage(damage);
            zombiesource.PlayOneShot(hitsound);
            yield return new WaitForSeconds(2);
            startAttack = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        EnemyManager();  
    }
}
