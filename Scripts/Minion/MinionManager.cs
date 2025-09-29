using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class MinionManager : MonoBehaviour
{
    public static MinionManager Instance;

    public Transform spawnPoint;
    public GameObject minionPrefab;
    public UnityEvent minionEvent;

    public System.Action minionClicked;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        for(int i = 0; i < ClickerDataManager.minions; i++)
        {
            SpawnMinion();
        }
    }

    public void Update()
    {
        Vector3 mousepos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousepos);
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                if (hitInfo.collider.CompareTag("Minion"))
                {
                    minionClicked?.Invoke();
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                if (hitInfo.collider.CompareTag("Minion"))
                {
                    minionClicked -= hitInfo.collider.GetComponent<Minion>().Jump;
                }
            }
        }
    }

    public void SpawnMinion()
    {
        Vector3 randomOffset = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
        Instantiate(minionPrefab, spawnPoint.position + randomOffset, Quaternion.identity);
        
        minionEvent?.Invoke();
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
