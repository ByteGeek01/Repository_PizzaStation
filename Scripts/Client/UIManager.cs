using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Transform clientsPanel; // Panel contenedor en el Canvas

    void Awake()
    {
        instance = this;
    }
}