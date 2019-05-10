using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class RoomCreation : MonoBehaviour
{
    public float scaleMultiplier, doorRespawn;
    public int numEnemies;
    public bool currentPlane, startTimer;
    private Vector3 right, left, forw, back;
    public Transform middleTransform;
    public GameObject player, enemy;
    public Text text;
    private roomManagement roomManager;
    private doorDetection doorDetection;


    void Start()
    {
        SetUpRoomCreation();
        SpawnObjects();
        SpawnEnemies();
    }

    public void SetUpWorldPositions()
    {
        GameObject currentPlaneObj = gameObject;
        scaleMultiplier = transform.localScale.x * 10;
        left = currentPlaneObj.transform.position + Vector3.left * scaleMultiplier;
        right = currentPlaneObj.transform.position + Vector3.right * scaleMultiplier;
        forw = currentPlaneObj.transform.position + Vector3.forward * scaleMultiplier;
        back = currentPlaneObj.transform.position + Vector3.back * scaleMultiplier;
    }
    public void SetUpRoomCreation()
    {
        text = GameObject.FindGameObjectWithTag("numEnemies").GetComponent<Text>();
        player = GameObject.FindGameObjectWithTag("Player");
        middleTransform = GameObject.FindGameObjectWithTag("Middle").transform;
        doorDetection = player.gameObject.GetComponent<doorDetection>();
        enemy = (GameObject)Resources.Load("Zombie");
        SetUpWorldPositions();
        roomManager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<roomManagement>();
        roomManager.AddRoom(gameObject);
        currentPlane = true;

    }
    public Vector3 RandomSpawnPoint()
    {
        int spawnbuffer = 5;
        Vector3 spawnpoint = new Vector3(Random.Range(transform.position.x - (scaleMultiplier / 2) + spawnbuffer, transform.position.x + (scaleMultiplier / 2) - spawnbuffer), 0.05f, Random.Range(transform.position.z - (scaleMultiplier / 2) + spawnbuffer, transform.position.z + (scaleMultiplier / 2) - spawnbuffer));
        return spawnpoint;
    }
    public void SpawnEnemies()
    {
        int i = 0;
        int numSpawn = roomManager.numEnemiesToSpawn();
        while (i < numSpawn)
        {
            Vector3 spawn = RandomSpawnPoint();
            GameObject zombie = Instantiate(enemy, spawn, Quaternion.identity);
            i++;
        }
    }
    public void UpdateNumEnemies()
    {
        int i = 0;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject enemy in enemies)
        {
            i++;
        }
        numEnemies = i;
        text.text = i + " hostiles remaining";
    }
    public void CreateRoom(Vector3 pos, int child)
    {
        startTimer = true;
        GameObject RoomResource = Resources.Load<GameObject>("Room");
        GameObject SpawnedRoom = Instantiate(RoomResource, pos, Quaternion.identity);
        SpawnedRoom.name = "Room" + roomManager.NumRooms();
        GameObject doorDestroy = SpawnedRoom.transform.GetChild(child).gameObject;
        doorDetection.SetBoolsFalse();
        StartCoroutine(InactiveToActive(doorDestroy));
        currentPlane = false;
    }
    public void BossSpawnProbability(int minRoomsToSpawn, int maxRoomsToSpawn)
    {
        int spawnBossRoom = 0;
        if (roomManager.NumRooms() > minRoomsToSpawn)
        {
            spawnBossRoom = Random.Range(0, 10);
            Debug.Log(spawnBossRoom);
        }
        if (spawnBossRoom > 6 && spawnBossRoom <= 9 || roomManager.NumRooms() > maxRoomsToSpawn)
        {
            SceneManager.LoadScene("bossfight");
        }
    }

    public void RoomAI(Vector3 pos, int child)
    {
        BossSpawnProbability(4, 7);
        Vector3 temp = middleTransform.position;
        middleTransform.position = pos;
        if (roomManager.checkExisting(middleTransform.gameObject))
        {
            CreateRoom(pos, child);
        }
        else
        {
            middleTransform.position = temp;
        }
    }

    public void DeletePreviousRooms(int numRooms)
    {
        if (roomManager.NumRooms() > numRooms)
        {
            Transform roomToDelete = roomManager.RoomList()[roomManager.NumRooms() - 2].transform;
            for (int i = 0; i < roomToDelete.childCount; i++)
            {
                if (roomToDelete.GetChild(i).gameObject.tag != "persist")
                {
                    roomToDelete.GetChild(i).tag = "unpassable";
                    roomToDelete.GetChild(i).GetComponent<MeshCollider>().isTrigger = false;
                }
            }
        }
    }
    public void MapAI()
    {

        if (currentPlane == true)
        {
            if (doorDetection.doorLeft == true) // pos = left  child == 3
            {
                RoomAI(left, 3);
            }
            else if (doorDetection.doorUp == true)// pos = forw child = 1
            {
                RoomAI(forw, 1);
            }
            else if (doorDetection.doorRight == true) // pos = right  child - 2
            {
                RoomAI(right, 2);
            }
            else if (doorDetection.doorDown == true) // pos = back  child = 0
            {
                RoomAI(back, 0);
            }
        }
        DeletePreviousRooms(1);

    }

    private void SpawnObjects()
    {
        Transform room = transform.Find("Persisting Room");
        Transform props = room.Find("Props");
        var allChildren = props.gameObject.GetComponentInChildren<Transform>();
        foreach (Transform t in allChildren)
        {
            if (t.CompareTag("roomassets") == true)
            {
                float p = Random.Range(0, 100);
                if (p < 35)
                {
                    t.gameObject.SetActive(false);
                }
            }
        }
    }
    IEnumerator InactiveToActive(GameObject door)
    {
        door.SetActive(false);
        yield return new WaitForSeconds(1);
        door.SetActive(true);
    }


    void Update()
    {

        MapAI();
        UpdateNumEnemies();
    }
}
 
