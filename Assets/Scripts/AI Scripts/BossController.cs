using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    public GameObject Player,lazer1,lazer2,rockPrefab;

    public GameObject[] enemies;

    public Text warning;

    private IEnumerator coroutine;

    public AudioClip deathsound;
    public AudioSource zombiesource;

    public int laserAttack,enemyHealth;

    public bool lazerZombieTime,fireRocks,startAttack,way,activateWarning;

    // Start is called before the first frame update
    void Start()
    {
        SetBooleans();
        StartCoroutine(ZombieThrowing(3));
        zombiesource = GetComponent<AudioSource>();
        Player = GameObject.FindGameObjectWithTag("Player");
        enemyHealth = 5000;
        laserAttack = 1800;
        
    }
    public void BossAI()
    {
        transform.LookAt(new Vector3(Player.transform.position.x, 0f, Player.transform.position.z));
        if (enemyHealth <= 0)
        {
            PlayDeath();
            Destroy(gameObject);
        }
        Patrol();
    }

    public void CheckForSpawnLazerZombies()
    {
        if (enemyHealth <= laserAttack)
        {
            if (lazerZombieTime)
            {
                StartCoroutine(DisplayWarning());
                InstantiateLazerZombs();
            }
        }
    }

    public void CheckForBossDeath()
    {
        if (enemyHealth <= 0)
        {
            SceneManager.LoadScene("WinScene");
        }
    }
    public void InstantiateRock()
    {
        if (fireRocks)
        {
            Vector3 start = new Vector3(transform.position.x, 6, transform.position.z);
            GameObject rock = Instantiate(rockPrefab, start, transform.rotation);
        }
    }

    public void InstantiateLazerZombs()
    {
        lazer1.SetActive(true);
        lazer2.SetActive(true);
    }

    public void Patrol()
    {
        GameObject[] ways = GameObject.FindGameObjectsWithTag("way");
        if (way)
        {
            transform.position = Vector3.MoveTowards(transform.position, ways[0].transform.position, Time.deltaTime * 2);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, ways[1].transform.position, Time.deltaTime * 2);
        }
    }

    public void PlayDeath()
    {
        Player.GetComponent<AudioSource>().PlayOneShot(deathsound);
    }


    public void SetWarningToInactive()
    {
        warning = GameObject.Find("BehindYOu").GetComponent<Text>();
        warning.gameObject.SetActive(false);
    }

    public void SetBooleans()
    {
        way = false;
        fireRocks = true;
        lazerZombieTime = true;
        activateWarning = false;
        startAttack = true;
    }

    public void TakeDamage(int damage)
    {
        enemyHealth -= damage;
        Debug.Log(enemyHealth);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "way")
        {
            way = !way;
        }
    }

    IEnumerator DisplayWarning()
    {
        warning.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        warning.gameObject.SetActive(false);
        lazerZombieTime = false;
    }

    IEnumerator ZombieThrowing(int seconds)
    {
        while (true)
        {
            InstantiateRock();
            yield return new WaitForSeconds(seconds);
        }
    }

    // Update is called once per frame
    void Update()
    {
        BossAI();
        CheckForSpawnLazerZombies();
        CheckForBossDeath();
    }

  
}
