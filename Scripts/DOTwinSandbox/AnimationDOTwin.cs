using UnityEngine;
using DG.Tweening;

public class AnimationDOTwin : MonoBehaviour
{
    public void Start()
    {
        transform.DOMoveX(transform.position.x + 3f, 2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

        transform.DORotate(new Vector3(0, 360, 0), 3f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.InOutSine);

        transform.DOScale(transform.localScale * 2f, 1.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}
