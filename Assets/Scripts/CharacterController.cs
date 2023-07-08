using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour
{
    public enum State { Walk, Jump }

    public float speed = 10;
    public float jump = 30;
    public float coyoteTime = 0.1f;
    public float jumpTime = 1;
    public float jumpHeight = 3;
    public float lowFallSpeed = 1f;
    public AnimationCurve curve;

    Rigidbody2D body;
    float timer = 0.0f;
    float lastCurveValue;

    public State state = State.Walk;

    private float JumpMultiplier = 1;
    private Vector3 JumpVector = Vector2.up;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (state == State.Walk) {
            if (timer > 0)
            timer -= Time.deltaTime; // coyote time
        }

        if (state == State.Jump) {
            timer += Time.deltaTime / jumpTime;
            float curveValue = curve.Evaluate(timer);
            float delta = curveValue - lastCurveValue;
            lastCurveValue = curveValue;
            transform.position += JumpVector * delta * jumpHeight * JumpMultiplier;
            if (timer >= 1)
            {
                state = State.Walk;
                timer = 0;
            }
        }

        float horizontal = Input.GetAxis("Horizontal") * speed;
        float vertical = body.velocity.y;

        if (state == State.Walk) {
            if (Input.GetAxis("Jump") > 0.01f) {
                body.gravityScale = 0;
                vertical = -lowFallSpeed;
                if (timer > 0) {
                    Jump(1, Vector3.up);
                    vertical = 0;
                }
            } else {
                body.gravityScale = 1.0f;
            }
        }

        body.velocity = Vector2.right * horizontal + Vector2.up * vertical;
    }

    public void Jump(float multiplier, Vector3 vector)
    {
        if (state == State.Jump) return;
        lastCurveValue = curve.Evaluate(0);
        state = State.Jump;
        timer = 0;
        body.gravityScale = 0;
        body.velocity = new Vector2();
        JumpMultiplier = multiplier;
        JumpVector = vector;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.isTrigger) return;
        if (state == State.Walk)
            timer = coyoteTime;
    }
}
