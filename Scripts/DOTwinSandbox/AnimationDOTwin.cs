using UnityEngine;
using DG.Tweening;

public class AnimationDOTwin : MonoBehaviour
{
    public void Start()
    {
        // Secuencia
        //Sequence mySequence = DOTween.Sequence();
        //mySequence.Append(transform.DOMoveX(transform.position.x + 3f, 2f).SetEase(Ease.InOutSine));
        //mySequence.Append(transform.DORotate(new Vector3(0, 360, 0), 3f, RotateMode.FastBeyond360).SetEase(Ease.Linear));
        //mySequence.Append(transform.DOScale(transform.localScale * 2f, 1.5f).SetEase(Ease.InOutSine));

        // Se mueve por 3 unidades, por 2 segundos
        transform.DOMoveX(transform.position.x + 3f, 2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

        // Rota en 360°
        transform.DORotate(new Vector3(0, 360, 0), 3f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);

        // Escala el objeto por 1,5 segundos
        transform.DOScale(transform.localScale * 2f, 1.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

        //mySequence.AppendCallback(() => Debug.Log("Complete"));
        //mySequence.SetLoops(-1, LoopType.Yoyo);
    }
}