using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class dontdestroyonload : MonoBehaviour
{
    private static dontdestroyonload instance;
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
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "WillTest")
        {
            Destroy(this.gameObject);
        }
        
    }
}
