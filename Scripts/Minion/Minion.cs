using UnityEngine;
using DG.Tweening;

public class Minion : MonoBehaviour
{
    public void Start()
    {
        MinionManager.Instance.minionClicked += Jump;
    }

    public void Jump()
    {
        transform.DOJump(transform.position, 1f, 1, 0.5f, false);
    }
}
