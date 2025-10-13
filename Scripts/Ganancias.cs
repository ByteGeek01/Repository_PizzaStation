using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ganancias : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textoDinero;
    [SerializeField] private TextMeshProUGUI textoPizzas;
    [SerializeField] private TextMeshProUGUI textoClientesMolestos;

    void Start()
    {
        // Guarda los puntajes de las ganancias, clientes insatisfechos y pizzas entregadas
        int cash = PlayerPrefs.GetInt("Cash", 0);
        int unhappy = PlayerPrefs.GetInt("UnhappyClients", 0);
        int pizzas = PlayerPrefs.GetInt("PizzasDelivered", 0);

        textoDinero.text = $"Ganancias del día: ${cash}";
        textoPizzas.text = $"Pizzas entregadas: {pizzas}";
        textoClientesMolestos.text = $"Clientes molestos: {unhappy}";
    }

    // Botón para volver a jugar
    public void VolverAJugar()
    {
        SceneManager.LoadScene("PizzaBoat");
    }
}