using UnityEngine;

public class PlayerBreak : MonoBehaviour
{
    public float moveSpeed = 15f;
    public float boundX = 3.5f;

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector2 move = new Vector2(horizontalInput * moveSpeed * Time.deltaTime, 0);
        transform.Translate(move);
        Vector2 clampedPos = transform.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, -boundX, boundX);
        transform.position = clampedPos;
    }
}
