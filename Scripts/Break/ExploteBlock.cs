using DG.Tweening;
using UnityEngine;

public class ExploteBlock : Block
{
    public float explosionRadius = 1f;
    public int explosiveDamage = 3;

    public override void OnDestoyed()
    {
        sprite.DOColor(new Color(1f, 0.5f, 0.5f), 0.1f);
        sprite.transform.DOShakePosition(0.025f, 1f).OnComplete(() =>
        {
            Collider2D[] hitcollider = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (var hit in hitcollider)
            {
                Block block = hit.GetComponent<Block>();
                if (block != null && block != this)
                {
                    block.TakeDamage(explosiveDamage);
                }
            }
            Destroy(gameObject);
        });
    }
}
