using System.Collections;
using UnityEngine;

public class lazercontroller : MonoBehaviour
{
    public GameObject Player;
    public bool startAttack,fireRocks;
    public GameObject rockPrefab;


    public void InstantiateRock()
    {
        if (fireRocks)
        {
            Vector3 start = new Vector3(transform.position.x, 4, transform.position.z);
            GameObject rock = Instantiate(rockPrefab, start, transform.rotation);
        }

    }
    IEnumerator ZombieThrowing(int seconds)
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            InstantiateRock();
            yield return new WaitForSeconds(seconds);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        fireRocks = true;
        StartCoroutine(ZombieThrowing(3));
        Player = GameObject.FindGameObjectWithTag("Player");
        startAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(new Vector3(Player.transform.position.x, 0f, Player.transform.position.z));
        fireRocks = true;
    }
}
