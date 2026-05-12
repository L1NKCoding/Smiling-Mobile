using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryZone : MonoBehaviour
{
    public GameObject victoryPanel; // Drag the VictoryPanel here

    // This runs automatically when something enters the Trigger Cube
    private void OnTriggerEnter(Collider other)
    {
        // Check if the thing that entered the zone is the Player
        if (other.CompareTag("Player"))
        {
            WinGame();
        }
    }

    void WinGame()
    {
        victoryPanel.SetActive(true);
        Time.timeScale = 0f; // Freeze the game world

        // Unlock the mouse so they can click the button
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Link this to your "Return to Title" button in the Inspector
    public void LoadTitle()
    {
        Time.timeScale = 1f; // Reset time
        SceneManager.LoadScene("Title Screen");
    }
}