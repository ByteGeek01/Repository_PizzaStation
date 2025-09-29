using UnityEngine;
using DG.Tweening;

public class AnimationDOTwin : MonoBehaviour
{
    public void Start()
    {
        // Se mueve por 3 unidades, por 2 segundos
        transform.DOMoveX(transform.position.x + 3f, 2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

        // Rota en 360°
        transform.DORotate(new Vector3(0, 360, 0), 3f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.InOutSine);

        // Escala el objeto por 1,5 segundos
        transform.DOScale(transform.localScale * 2f, 1.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}