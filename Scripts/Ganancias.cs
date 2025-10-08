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
        int dinero = SessionData.dineroDelDia;
        int pizzas = SessionData.pizzasEntregadas;
        int molestos = SessionData.clientesMolestos;

        textoDinero.text = $"Ganancias del día: ${dinero}";
        textoPizzas.text = $"Pizzas entregadas: {pizzas}";
        textoClientesMolestos.text = $"Clientes molestos: {molestos}";
    }

    // Botón para volver a jugar
    public void VolverAJugar()
    {
        SceneManager.LoadScene("PizzaBoat");
    }
}