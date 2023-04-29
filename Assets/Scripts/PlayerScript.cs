using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {

    // Prefab for 'game over' canvas overlay
    public GameObject deadOverlay;

    internal List<KeyCode> dashKeys = new List<KeyCode>{
        KeyCode.LeftShift, KeyCode.JoystickButton0};
    internal List<KeyCode> jumpKeys = new List<KeyCode>{
        KeyCode.W, KeyCode.UpArrow, KeyCode.Space, KeyCode.JoystickButton3};
    internal List<KeyCode> diveKeys = new List<KeyCode>{
        KeyCode.S, KeyCode.DownArrow, KeyCode.LeftControl, KeyCode.JoystickButton8};

    internal float walkSpeed = 20f;
    protected float dashSpeed = 100f;
    protected float jumpForce = 1500f;
    protected float accelSpeed = 10f;

    private bool changingDirection => (
        GetComponent<Rigidbody2D>().velocity.x > 0f && Input.GetAxis("Horizontal") < 0f)
        || (GetComponent<Rigidbody2D>().velocity.x < 0f && Input.GetAxis("Horizontal") > 0f
    );

    void Update() {
        if (dead) return;
        var input = new Vector3(Input.GetAxis("Horizontal"), 0, 0);

        UpdateJump(jumpKeys.Any(k => Input.GetKeyDown(k)));
        UpdateDive(diveKeys.Any(k => Input.GetKeyDown(k)));

        // Continue full forward / reverse when diving
        if (IsDiving()) {
            if (GetComponent<Rigidbody2D>().velocity.x < 0) {
                input = new Vector3(-1, 0, 0);
            } else {
                input = new Vector3(1, 0, 0);
            }
        }

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


    float diveTimer;
    float diveMax = 1f;
    protected void UpdateDive(bool startDive) {
        if (IsDiving()){
            diveTimer += Time.deltaTime;
        } else if (startDive && InAir()){
            Dive();
        } 

        // For now, dive simply times out
        if (diveTimer > diveMax) diveTimer = 0;
    }
    internal void Dive() {
        // Initiate Dive

        // Cancel out y velocity when initiated
        if (diveTimer == 0) {
            var rc = GetComponent<Rigidbody2D>();
            rc.velocity = new Vector2(rc.velocity.x, 0);
        }
        diveTimer += Time.deltaTime;
        // TODO wish I could have this just be 'active when diving',
        // but leaving for now
        var diving = transform.Find("Effects/Diving").GetComponent<ParticleSystem>();
        var main = diving.main;
        if (!diving.isPlaying) main.duration = diveMax;
        diving.Play();
    }
    internal bool IsDiving() {
        return diveTimer > 0;
    }

    protected void UpdateDash(bool diving) {
        // TODO
    }

    float jumpTimer;
    protected void UpdateJump(bool jumped) {
        // Check to see if the being wishes to jump,
        // and handle duplicate jumps, coyote jumps, etc
        jumpTimer -= Time.deltaTime;
        if (jumped && CanJump()) Jump();
    }
    void Jump() {
        GetComponent<Rigidbody2D>().AddForce(transform.up * jumpForce);
        jumpTimer = 1f; // Time before a grounded player can jump again

        GetComponent<Animator>().SetTrigger("Jump");
    }
    public bool CanJump() {
        return !InAir() && jumpTimer < 0.001f;
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

        return coyoteTimer > coyoteMax;
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

        // Player isn't adding input to run or jump
        bool lowInput = acceleration.magnitude < 0.001f;

        // Damening
        if (!IsDiving() && (lowInput || changingDirection)) {
            prevVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, 10 * Time.deltaTime);    

            // Leave out y from dampening - is this right?
            prevVelocity = new Vector3(prevVelocity.x, currentVelocity.y, prevVelocity.z);
        }
        
        if (currentVelocity.magnitude > maxVelocity) {
            return currentVelocity.normalized * maxVelocity;
        } else if (lowInput && currentVelocity.magnitude < 0.001f) {
            return Vector3.zero;
        }

        return currentVelocity;
    }

    internal void Animate() {
        // TODO is this needed?
        var a = GetComponent<Animator>();
        var rc = GetComponent<Rigidbody2D>();
        var speedRatio = Mathf.Abs(rc.velocity.x) / walkSpeed;

        var anim = GetComponent<Animator>();
        anim.SetFloat("Speed", speedRatio);
        anim.SetBool("Diving", IsDiving());
        anim.SetBool("InAir", InAir());

        var t = transform.Find("Canvas/Speed Text").GetComponent<Text>();
        t.text = ""+Mathf.Round(rc.velocity.magnitude);

        // When walking / runing, speed can vary
        anim.speed = Mathf.Max(0.4f, speedRatio * 2);

        // But not always
        var staticSpeed = new List<string> { "jump"};
        foreach (var name in staticSpeed) {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName(name)) anim.speed = 1;
        }

        // Flip sprite
        GetComponent<SpriteRenderer>().flipX = rc.velocity.x < 0;

        // Edit particles
        var running = transform.Find("Effects/Running").GetComponent<ParticleSystem>();
        var runningEms = running.emission;
        runningEms.rateOverTime = 10 * speedRatio;
        if (InAir()) runningEms.rateOverTime = 0;
        running.startSpeed = 10 * speedRatio;

        if (IsDiving()) {
            GetComponent<SpriteRenderer>().color = HexColor("#00FFC7");
        } else {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    bool dead = false;
    public void Kill() {
        // TODO kill message, restart, etc
        dead = true;

        Instantiate(deadOverlay, Vector3.zero, Quaternion.identity);
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Rigidbody2D>().simulated = false;
        var gib = transform.Find("Effects/Gib").GetComponent<ParticleSystem>();
        gib.Play();
    }

    int babiesDelivered = 0;
    public int Deposit() {
        if (!CanDeposit()) return -1;
        // We successsfully delivered a baby to it's destination
        babiesDelivered++;
        Destroy(transform.Find("Baby"));
        return babiesDelivered;
    }
    public bool CanDeposit() {
        // Do we have a baby to deliver?
        return transform.Find("Baby") != null;
    }

    public Color HexColor(string hex) {
        // Simple helper because I want to use hex colors
        var hexStyle = System.Globalization.NumberStyles.HexNumber;
        int red = int.Parse(hex.Substring(1, 2), hexStyle);
        int green = int.Parse(hex.Substring(3, 2), hexStyle);
        int blue = int.Parse(hex.Substring(5, 2), hexStyle);
        return new Color(red, green, blue);
    }
}
