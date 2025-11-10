using UnityEngine;
using DG.Tweening;

public class Water : MonoBehaviour
{
    [Header("Configuración de pérdida")]
    public GameObject lose;

    [Header("Animación del agua (marea)")]
    public float amplitude = 0.5f;   // Altura máxima de la marea
    public float duration = 6f;      // Tiempo de subida o bajada
    public float lateralMove = 0.2f; // Movimiento lateral opcional
    public Ease easeType = Ease.InOutSine; // Suavizado del movimiento

    private Vector3 startPos;
    private Sequence waveSequence;

    private void Start()
    {
        startPos = transform.position;
        StartWaterAnimation();
    }

    private void StartWaterAnimation()
    {
        waveSequence = DOTween.Sequence();

        // Subida de la marea
        waveSequence.AppendCallback(() =>
        {
            Debug.Log("🌊 Subiendo la marea");
            if (GameManager.instance != null)
                GameManager.instance.ModifySpawnSpeed(true); // Lento
        });

        waveSequence.Append(transform.DOMoveY(startPos.y + amplitude, duration)
            .SetEase(easeType));

        // Bajada de la marea
        waveSequence.AppendCallback(() =>
        {
            Debug.Log("🌊 Bajando la marea");
            if (GameManager.instance != null)
                GameManager.instance.ModifySpawnSpeed(false); // Normal
        });

        waveSequence.Append(transform.DOMoveY(startPos.y - amplitude, duration)
            .SetEase(easeType));

        waveSequence.SetLoops(-1, LoopType.Restart);

        // Movimiento lateral opcional
        transform.DOMoveX(startPos.x + lateralMove, duration * 2)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            lose.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        waveSequence?.Kill();
        DOTween.Kill(transform);
    }
}
