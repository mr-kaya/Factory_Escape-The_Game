using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScr : MonoBehaviour
{
    void Start()
    {
        
    }
    
    public void Play() {
        SceneManager.LoadScene("Level1");
    }

    public void Exit() {
        Debug.Log("Exit");
    }
}
