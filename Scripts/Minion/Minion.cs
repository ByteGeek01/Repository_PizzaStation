using UnityEngine;
using DG.Tweening;

public class Minion : MonoBehaviour
{
    public void Start()
    {
        MinionManager.Instance.minionClicked += Jump;
    }

    public void Jump(float height, int jump, float duration)
    {
        transform.DOJump(transform.position, height, jump, duration, false);
    }
}
