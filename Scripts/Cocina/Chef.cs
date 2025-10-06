using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class Chef : MonoBehaviour
{
    public NavMeshAgent agent;

    [Header("Referencias")]
    public Transform kitchenPoint; // posición base del chef
    public Oven oven;              // referencia al horno
    public GameObject pizzaPrefab; // prefab de pizza

    [Header("Manos y objetos")]
    public Transform handPoint;            // punto donde sostiene ingredientes o pizza
    public GameObject[] ingredientObjects; // visuales del chef (pan, salsa, queso, carne)
    private List<GameObject> activeIngredients = new List<GameObject>();

    [Header("Estado actual")]
    private bool hasAllIngredients = false;
    private bool isCooking = false;
    private bool pizzaReady = false;
    private bool delivering = false;

    private Mesero waiter; // referencia al mesero

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(kitchenPoint.position);

        waiter = FindFirstObjectByType<Mesero>();

        // Desactiva todos los ingredientes visuales al iniciar
        foreach (var ing in ingredientObjects)
        {
            if (ing != null)
                ing.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si el chef choca con un ingrediente válido
        switch (other.tag)
        {
            case "Bread":
                ActivateIngredient(0);
                Destroy(other.gameObject);
                break;

            case "Sauce":
                ActivateIngredient(1);
                Destroy(other.gameObject);
                break;

            case "Cheese":
                ActivateIngredient(2);
                Destroy(other.gameObject);
                break;

            case "Meat":
                ActivateIngredient(3);
                Destroy(other.gameObject);
                break;
        }

        // Cuando tiene los 4 ingredientes activos → marca listo, spawnea pizza y va al horno
        if (activeIngredients.Count >= 4 && !isCooking)
        {
            hasAllIngredients = true;

            // 🚀 Aquí spawn la pizza en su mano
            SpawnPizzaInHand();

            GoToOven();
        }
    }

    private void ActivateIngredient(int index)
    {
        if (!activeIngredients.Contains(ingredientObjects[index]))
        {
            ingredientObjects[index].SetActive(true);
            ingredientObjects[index].transform.SetParent(handPoint);
            ingredientObjects[index].transform.localPosition = Vector3.zero;
            activeIngredients.Add(ingredientObjects[index]);
        }
    }

    private void SpawnPizzaInHand()
    {
        GameObject pizza = Instantiate(pizzaPrefab, handPoint.position, Quaternion.identity);
        pizza.transform.SetParent(handPoint);
        pizza.transform.localPosition = Vector3.zero;
    }

    private void GoToOven()
    {
        agent.SetDestination(oven.transform.position);
        StartCoroutine(CheckDistanceToOven());
    }

    private IEnumerator CheckDistanceToOven()
    {
        while (Vector3.Distance(transform.position, oven.transform.position) > 1.5f)
        {
            yield return null;
        }

        StartCoroutine(CookPizza());
    }

    private IEnumerator CookPizza()
    {
        isCooking = true;
        agent.isStopped = true;

        // “Coloca” los ingredientes en el horno visualmente
        foreach (var ing in activeIngredients)
        {
            ing.transform.DOJump(oven.transform.position + Vector3.up * 0.5f, 0.5f, 1, 0.5f)
                .OnComplete(() => ing.SetActive(false));
        }
        activeIngredients.Clear();

        yield return new WaitForSeconds(0.5f);

        // Comienza a hornear
        oven.StartCoroutine(oven.Bake());

        // Espera el proceso de horneado (igual a Bake())
        yield return new WaitForSeconds(6f);

        isCooking = false;
        hasAllIngredients = false;
        pizzaReady = true;
        agent.isStopped = false;

        // Una vez lista la pizza, la lleva al mesero
        TakePizza();
    }

    private void TakePizza()
    {
        GameObject pizza = Instantiate(pizzaPrefab, handPoint.position, Quaternion.identity);
        pizza.transform.SetParent(handPoint);
        pizza.transform.localPosition = Vector3.zero;
    }
}