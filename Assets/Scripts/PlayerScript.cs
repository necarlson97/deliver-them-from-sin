using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {

    internal List<KeyCode> dashKeys = new List<KeyCode>{KeyCode.LeftShift, KeyCode.JoystickButton0};
    internal List<KeyCode> jumpKeys = new List<KeyCode>{KeyCode.W, KeyCode.Space, KeyCode.JoystickButton3};
    internal List<KeyCode> diveKeys = new List<KeyCode>{KeyCode.S, KeyCode.LeftControl, KeyCode.JoystickButton8};

    internal float walkSpeed = 10f;
    protected float dashSpeed = 100f;
    protected float jumpForce = 1000f;
    protected float accelSpeed = 100f;
 
    void Start() {
    }

    void Update() {
        var input = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
        UpdateJump(jumpKeys.Any(k => Input.GetKeyDown(k)));
        UpdateDive(diveKeys.Any(k => Input.GetKeyDown(k)));
        UpdateWalk(input);
        UpdateDash(dashKeys.Any(k => Input.GetKey(k)));

        Animate();
    }

    protected void UpdateWalk(Vector3 direction) {
        var rc = GetComponent<Rigidbody2D>(); // Shorthand
        float moveSpeed = walkSpeed;

        // Normalize vector, and scale by speed
        var accel = direction.magnitude * direction.normalized * accelSpeed;
        var walkVelocity = AccelerationToVelocity(accel, moveSpeed);
        rc.velocity = new Vector3(walkVelocity.x, rc.velocity.y, walkVelocity.z);
    }

    protected void UpdateDive(bool diving) {
        // TODO
    }

    protected void UpdateDash(bool diving) {
        // TODO
    }

    float jumpTimer;
    protected void UpdateJump(bool jumped) {
        // Check to see if the being wishes to jump,
        // and handle duplicate jumps, coyote jumps, etc
        jumpTimer -= Time.deltaTime;
        if (jumped && !InAir()) Jump();
    }
    void Jump() {
        GetComponent<Rigidbody2D>().AddForce(transform.up * jumpForce);
        jumpTimer = 1f; // Time before a grounded player can jump again
    }

    float coyoteMax = 0.2f;
    float coyoteTimer;
    public bool InAir() {
        // Return true when the player is considered
        // airborn - both for animation sake, and jumping sake
        // TODO sloppy, changes dependent on # of times called
        var distance = .1f;
        var start = BottomPoint();
        var direction = new Vector3(0, -distance, 0);

        var rc = GetComponent<Rigidbody2D>();
        // Unity layer mask for everything except the 'Player' layer
        var notPlayerMask =~ LayerMask.GetMask("Player");
        var hit = Physics2D.Raycast(start, direction, distance, notPlayerMask);
 
        if (hit.collider != null) {
            // If we are at walking on the ground, we get coyote back
            coyoteTimer = 0;
        } else {
            coyoteTimer = coyoteTimer + Time.deltaTime;
        }

        return coyoteTimer > coyoteMax || jumpTimer > 0;
    }
    public Vector3 BottomPoint() {
        // Find a low point in the center, just above the ground, to use for
        // telling if we are airborne
        var y = GetComponent<Collider2D>().bounds.min.y;
        return new Vector3(transform.position.x, y+.001f, transform.position.z);
    }

    Vector3 prevVelocity;
    public Vector3 AccelerationToVelocity(Vector3 acceleration, float maxVelocity) {
        // We want the player to move with constant,
        // frame independant acceleration - so we apply
        // acceleration, and return the velocity

        Vector3 currentVelocity = prevVelocity + acceleration * Time.deltaTime; 
        prevVelocity = currentVelocity;
        if (acceleration.magnitude < 0.001f) {
            prevVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, 30 * Time.deltaTime); // Dampening    
        }

        if (currentVelocity.magnitude > maxVelocity) return currentVelocity.normalized * maxVelocity;
        
        return currentVelocity;
    }

    internal void Animate() {
        // TODO is this needed?
        var a = GetComponent<Animator>();
        var rc = GetComponent<Rigidbody2D>();
        var speedRatio = Mathf.Abs(rc.velocity.x) / walkSpeed;

        var anim = GetComponent<Animator>();
        anim.SetFloat("Speed", Mathf.Abs(rc.velocity.x));
        var t = transform.Find("Canvas/Speed Text").GetComponent<Text>();
        t.text = ""+Mathf.Round(rc.velocity.magnitude);
        anim.speed = speedRatio * 2;

        // Flip sprite
        GetComponent<SpriteRenderer>().flipX = rc.velocity.x < 0;
    }


}
