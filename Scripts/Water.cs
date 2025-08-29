using UnityEngine;
using UnityEngine.SceneManagement;

public class Water : MonoBehaviour
{
    public GameObject lose;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            lose.SetActive(true);
        }
    }
}
