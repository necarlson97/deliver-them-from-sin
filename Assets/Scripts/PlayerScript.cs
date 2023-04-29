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

    internal List<KeyCode> punchKeys = new List<KeyCode>{
        KeyCode.Space, KeyCode.JoystickButton0};
    internal List<KeyCode> jumpKeys = new List<KeyCode>{
        KeyCode.W, KeyCode.UpArrow, KeyCode.JoystickButton3};
    internal List<KeyCode> diveKeys = new List<KeyCode>{
        KeyCode.S, KeyCode.DownArrow, KeyCode.JoystickButton8};

    internal float walkSpeed = 20f;
    protected float jumpForce = 1500f;
    protected float accelSpeed = 20f;

    private bool changingDirection => (
        GetComponent<Rigidbody2D>().velocity.x > 0f && Input.GetAxis("Horizontal") < 0f)
        || (GetComponent<Rigidbody2D>().velocity.x < 0f && Input.GetAxis("Horizontal") > 0f
    );

    public int windowsBroken = 0;
    public int boxesBroken = 0;

    void Update() {
        if (dead) return;
        var input = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
        var rc = GetComponent<Rigidbody2D>();

        UpdateJump(jumpKeys.Any(k => Input.GetKeyDown(k)));
        UpdateDive(diveKeys.Any(k => Input.GetKeyDown(k)));
        UpdatePunch(punchKeys.Any(k => Input.GetKey(k)));
        UpdateWalk(input);

        Animate();
    }


    protected void UpdateWalk(Vector3 direction) {
        var rc = GetComponent<Rigidbody2D>(); // Shorthand

        // Normalize vector, and scale by speed
        var accel = direction.magnitude * direction.normalized * accelSpeed;
        var walkVelocity = AccelerationToVelocity(accel.x, walkSpeed);
        rc.velocity = new Vector2(walkVelocity, rc.velocity.y);

        // For now, keep y acceleration from rigidbody
        // TODO idk wtf is going on - tried to make the function terse,
        // and started damening all y movement
        // can't spend enough time to fix cleanly
        // rc.velocity = AccelerationToVelocity(accel, moveSpeed);

        // However, we could be doing a 'sepcial move'
        var specialSpeed = walkSpeed * .9f;
        if (IsDiving() ) {
            // Move diagonal when diving
            if (rc.velocity.x < 0) rc.velocity = new Vector2(-walkSpeed, -walkSpeed);
            else rc.velocity = new Vector2(walkSpeed, -walkSpeed);
            prevVelocity = rc.velocity.x;
        } else if (IsPunching()) {
            // When punching - just travel horizontally
            if (rc.velocity.x < 0) rc.velocity = new Vector2(-specialSpeed, 0);
            else rc.velocity = new Vector2(specialSpeed, 0);
            prevVelocity = rc.velocity.x;
        } 
    }

    float prevVelocity;
    public float AccelerationToVelocity(float acceleration, float maxVelocity) {
        // We want the player to move with constant,
        // frame independant acceleration - so we apply
        // acceleration, and return the velocity
        // This is only the x axis

        var rc = GetComponent<Rigidbody2D>(); // Shorthand
        float currentVelocity = prevVelocity + acceleration * Time.deltaTime; 
        prevVelocity = currentVelocity;

        // Player isn't adding input to run or jump
        bool lowInput = Mathf.Abs(acceleration) < 0.001f;

        // Damening
        if (!IsDiving() && !IsPunching() && (lowInput || changingDirection)) {
            prevVelocity = Mathf.Lerp(currentVelocity, 0, 10 * Time.deltaTime);
        }
        
        // Clamp
        currentVelocity = Mathf.Max(-maxVelocity, Mathf.Min(maxVelocity, currentVelocity));
        return currentVelocity;
    }



    float diveTimer;
    float diveMax = 0.5f;
    protected void UpdateDive(bool startDive) {
        // Already diving
        if (IsDiving()) diveTimer += Time.deltaTime;
        // Start dive
        else if (startDive)Dive();

        // For now, ends when it times out
        if (diveTimer > diveMax) diveTimer = 0;
    }
    internal void Dive() {
        // Initiate Dive
        // Can't start if busy
        if (IsPunching()) return;

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

    float punchTimer;
    float punchMax = 0.7f;
    bool punched = false;
    protected void UpdatePunch(bool startPunch) {
        // Already punching
        if (IsPunching()) punchTimer += Time.deltaTime;
        // Start punch
        else if (startPunch) Punch();

        // For now, ends when it times out
        if (punchTimer > punchMax) punchTimer = 0;
    }
    internal void Punch() {
        // Can't start if busy, or if haven't touched the
        // ground yet
        if (IsDiving() || punched) return;
        // Initiate punch
        punched = true;
        punchTimer += Time.deltaTime;
        

        // TODO wish I could have this just be 'active when punching',
        // but leaving for now
        var punching = transform.Find("Effects/Punching").GetComponent<ParticleSystem>();
        var main = punching.main;
        if (!punching.isPlaying) main.duration = punchMax;
        punching.Play();
    }
    internal bool IsPunching() {
        return punchTimer > 0;
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
        return !IsPunching() && !IsDiving() && !InAir() && jumpTimer < 0.001f;
    }

    float coyoteMax = 1f;
    public float coyoteTimer;
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
            punched = false;
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

    internal void Animate() {
        // TODO is this needed?
        var a = GetComponent<Animator>();
        var rc = GetComponent<Rigidbody2D>();
        var speedRatio = Mathf.Abs(rc.velocity.x) / walkSpeed;

        var anim = GetComponent<Animator>();
        anim.SetFloat("Speed", speedRatio);
        anim.SetBool("Diving", IsDiving());
        anim.SetBool("Punching", IsPunching());
        anim.SetBool("InAir", InAir());

        var t = transform.Find("Canvas/Speed Text").GetComponent<Text>();
        t.text = ""+Mathf.Abs(Mathf.Round(rc.velocity.x));

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

        var c = Color.white;
        if (IsDiving()) c = HexColor("#00FFC7");
        else if (IsPunching()) c = HexColor("#FFB411");
        else if (InAir()) c = HexColor("#B7B7B7");
        GetComponent<SpriteRenderer>().color = c;
    }

    bool dead = false;
    public void Kill() {
        // TODO kill message, restart, etc
        if (dead) return;
        dead = true;

        Instantiate(deadOverlay, Vector3.zero, Quaternion.identity);
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Rigidbody2D>().simulated = false;
        var gib = transform.Find("Effects/Gib").GetComponent<ParticleSystem>();
        gib.Play();
    }

    public int babiesDelivered = 0;
    public int Deposit() {
        if (!CanDeposit()) return -1;
        // We successsfully delivered a baby to it's destination
        babiesDelivered++;
        Destroy(transform.Find("Baby").gameObject);
        return babiesDelivered;
    }
    public bool CanDeposit() {
        // Do we have a baby to deliver?
        return transform.Find("Baby") != null;
    }

    public void BrokeBox() {
        // Can jump right after breaking boxes
        coyoteTimer = - (punchMax - punchTimer) - 1f;
        boxesBroken++;
    }

    public Color HexColor(string hex) {
        // Simple helper because I want to use hex colors
        var hexStyle = System.Globalization.NumberStyles.HexNumber;
        int red = int.Parse(hex.Substring(1, 2), hexStyle);
        int green = int.Parse(hex.Substring(3, 2), hexStyle);
        int blue = int.Parse(hex.Substring(5, 2), hexStyle);
        return new Color(red / 255f, green / 255f, blue / 255f);
    }
}
