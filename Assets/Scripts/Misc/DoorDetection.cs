using UnityEngine;

public class DoorDetection : MonoBehaviour
{
    public bool doorLeft;
    public bool doorRight;
    public bool doorUp;
    public bool doorDown;
    public bool startFollow;
    public EnemyManager enemyManager;
    public int numenem;
    

    void OnTriggerEnter(Collider collision)
    {
        if (numenem == 0)
        {
            if (collision.gameObject.tag != "unpassable")
                {
           
                if (collision.gameObject.tag == "doorLeft")
                {
                    Destroy(collision.gameObject);
                    doorLeft = true;
                    //Debug.Log("doorLeft hit!!");
                }
                if (collision.gameObject.tag == "doorUp")
                {
                    Destroy(collision.gameObject);
                    doorUp = true;
                    //Debug.Log("doorUp hit!!");
                }
                if (collision.gameObject.tag == "doorRight")
                {
                    Destroy(collision.gameObject);
                    doorRight = true;
                    //Debug.Log("doorRight hit!!");
                }
                if (collision.gameObject.tag == "doorDown")
                {
                    Destroy(collision.gameObject);
                    doorDown = true;
                    //Debug.Log("doorDown hit!!");
                }
            }
        }
        if (collision.gameObject.tag == "entrance")
        {
            Destroy(collision.gameObject);
            startFollow = true;
        }

    }

    public void SetBoolsFalse()
    {
        doorLeft = false;
        doorRight = false;
        doorUp = false;
        doorDown = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        doorLeft = false;
        doorRight = false;
        doorUp = false;
        doorDown = false;
        enemyManager = GameObject.Find("enemyManager").GetComponent<EnemyManager>();
        numenem = 0;
      

    }
    private void Update()
    {
        numenem = enemyManager.numEnemies();
    }

}
