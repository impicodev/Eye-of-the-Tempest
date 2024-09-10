using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10;
    public float jumpStrength = 10;
    public float jumpBuffer = 0.1f;

    Rigidbody2D rb;
    BoxCollider2D collider;
    float spacePressed = -100;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 vel = rb.velocity;
        vel.x = horizontal * speed;

        if (Input.GetKey(KeyCode.Space))
            spacePressed = Time.time;

        if (Time.time - spacePressed <= jumpBuffer) {

            Vector2 pos = rb.position;
            pos.y = collider.bounds.min.y - 0.05f;
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down);
            if (hit.collider != null && pos.y - hit.point.y < 0.05f)
            {
                spacePressed = -100;
                vel.y = jumpStrength;
            }
        }
        rb.velocity = vel;
    }
}
