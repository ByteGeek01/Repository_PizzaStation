using System;
using DG.Tweening;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int hitPoints = 1;
    public SpriteRenderer sprite;
    public Collider2D coll;

    public Action onBlockDestoyed;
    public Action<int> onBlockHit;
    public Action<int> onHit;

    public virtual void OnHit()
    {
        sprite.DOColor(new Color(1f, 0.5f, 0.5f), 0.1f).OnComplete(() =>
        {
            sprite.DOColor(Color.white, 0.1f);
        });
    }
    public virtual void OnBlockHit(int damage)
    {
        //
    }
    public virtual void OnDestoyed()
    {
        coll.enabled = false;
        sprite.transform.DOScale(Vector3.zero, 0.1f).OnComplete(() => Destroy(gameObject));
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage (int points)
    {
        hitPoints -= points;
        onBlockHit?.Invoke(points);
        onHit?.Invoke(hitPoints);
        if (hitPoints <= 0)
        {
            OnDestoyed();
        }
    }
}
