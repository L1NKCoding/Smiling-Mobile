using UnityEngine;
using UnityEngine.SceneManagement;


public class NewMonoBehaviourScript : MonoBehaviour
{
         void Start()
    {
        // Load saved settings on app start
        //GameSettings.Load();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("BicycleScene_URP");
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("Title Screen");
    }
}
