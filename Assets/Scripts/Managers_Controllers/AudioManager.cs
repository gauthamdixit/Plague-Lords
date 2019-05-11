using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public AudioClip menuMusic,gameMusic,bossMusic;
    public AudioSource gameAudio;
    private string activeScene = "";

        private void Awake()
        {
            // if the singleton hasn't been initialized yet
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;//Avoid doing anything else
            }

            instance = this;
        
            DontDestroyOnLoad(this.gameObject);
        }
  
    // Start is called before the first frame update
    void Start()
    {
        gameAudio = GetComponent<AudioSource>();
    }
    public void MusicHandler()
    {
        if (SceneManager.GetActiveScene().name == "MainGame" && !activeScene.Equals(SceneManager.GetActiveScene().name))
        {
            gameAudio.clip = gameMusic;
            gameAudio.Play();
            activeScene = SceneManager.GetActiveScene().name;
        }
        else if (SceneManager.GetActiveScene().name == "BossFight" && !activeScene.Equals(SceneManager.GetActiveScene().name))
        {
            gameAudio.clip = bossMusic;
            gameAudio.Play();
            activeScene = SceneManager.GetActiveScene().name;
        }
        else if (SceneManager.GetActiveScene().name == "MainMenu" && !activeScene.Equals(SceneManager.GetActiveScene().name))
        {
            gameAudio.clip = menuMusic;
            gameAudio.Play();
            activeScene = SceneManager.GetActiveScene().name;
        }
    }

    // Update is called once per frame
    void Update()
    {
        MusicHandler();
    }
}
