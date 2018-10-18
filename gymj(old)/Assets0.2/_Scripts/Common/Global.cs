using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Global : MonoBehaviour {
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

    }
    // Use this for initialization
    void Start () {

        SceneManager.LoadScene("_login");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
