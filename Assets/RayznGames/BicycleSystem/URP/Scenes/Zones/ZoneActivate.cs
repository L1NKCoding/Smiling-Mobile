using UnityEngine;

public class ZoneActivate : MonoBehaviour
{
  public GameObject panelToShow;  // Assign UI Panel in Inspector

    private void Start()
    {
        if (panelToShow != null)
            panelToShow.SetActive(false);  // Panel starts hidden
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            panelToShow.SetActive(true);
        }
    }
}