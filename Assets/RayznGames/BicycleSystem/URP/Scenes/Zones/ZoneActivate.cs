using UnityEngine;

public class ZoneActivate : MonoBehaviour
{
  public GameObject winScreen; // Assign in Inspector

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            winScreen.SetActive(true);
        }
    }
}