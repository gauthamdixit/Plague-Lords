using UnityEngine;
using UnityEngine.UI;
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
    private RoomManagement roomManager;
    private DoorDetection doorDetection;

    // gives each door a chance of entering the boss fight 
    // minRoomsToSpawn is the minimum number of rooms the player has to go through for the boss fight to be possible
    // after maxRoomsToSpawn is reached, the bossfight is guaranteed
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
            SceneManager.LoadScene("BossFight");
        }
    }
    //generates the room prefab
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
    //only allows numRooms amount of rooms to be active at any given point in the game
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
    // checks if the player has entered a door and generates a room based on which door the player passed through
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
    //returns a random point in the room for an enemy to spawn
    public Vector3 RandomSpawnPoint()
    {
        int spawnbuffer = 5;
        Vector3 spawnpoint = new Vector3(Random.Range(transform.position.x - (scaleMultiplier / 2) + spawnbuffer, transform.position.x + (scaleMultiplier / 2) - spawnbuffer)
            , 0.05f, Random.Range(transform.position.z - (scaleMultiplier / 2) + spawnbuffer, transform.position.z + (scaleMultiplier / 2) - spawnbuffer));
        return spawnpoint;
    }
    //Creates the room at position pos and deletes the door child from the room asset depending on which door the player passed through previously
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
        doorDetection = player.gameObject.GetComponent<DoorDetection>();
        enemy = (GameObject)Resources.Load("Zombie");
        SetUpWorldPositions();
        roomManager = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManagement>();
        roomManager.AddRoom(gameObject);
        currentPlane = true;
    }
    //spawns zombies per room
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
    //gets all the objects in the room prefab under persisting room and gives them a 35% chance of being set inactive
    //to make each room slightly different than the others
    private void SpawnObjectsInRoom()
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
    //Updates text field "number of hostiles"
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
    //sets the door the player entered to inactive so the player can pass through and then back to active so the player can't go back through
    IEnumerator InactiveToActive(GameObject door)
    {
        door.SetActive(false);
        yield return new WaitForSeconds(1);
        door.SetActive(true);
    }

    void Start()
    {
        SetUpRoomCreation();
        SpawnObjectsInRoom();
        SpawnEnemies();
    }

    void Update()
    {
        MapAI();
        UpdateNumEnemies();
    }
}
 
