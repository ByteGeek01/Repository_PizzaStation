using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToGame : MonoBehaviour
{
    public void LetsStart()
    {
        SceneManager.LoadScene(1);
    }
}
