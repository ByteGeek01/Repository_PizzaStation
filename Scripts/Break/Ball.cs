using UnityEngine;

public class Ball : MonoBehaviour
{
    public Rigidbody2D rb2d;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.linearVelocity = new Vector2(8f, 8f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
