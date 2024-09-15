using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PlayerState
{
    Locked, Regular, Beam, Wind,
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float JumpForce;
    [SerializeField] private float MoveForce;
    private float DefaultMoveForce;

    [SerializeField] public Animator PlayerAnimator;
    [SerializeField] private SpriteRenderer PlayerSprite;

    [SerializeField] private Texture2D CustomCursor;

    private float defaultGravity;

    public static PlayerController Main;
    private Camera cam;
    
    [SerializeField] private PlayerBottomDetector BottomDetector;

    public PlayerState State = PlayerState.Regular;
    private LightBeam CurrentBeam = null;

    public Rigidbody2D rb;
    void Start()
    {
        DefaultMoveForce = MoveForce;
        cam = Camera.main;
        Main = this;
        rb = this.GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
        ChangeToRegular();
    }

    public bool CanJump = true;
    public bool lightningBoost = false;

    private bool JumpBuffer = false;
    private int JumpBufferFrame = 1;

    private Vector2 direction = Vector2.zero;
    private Vector2 prevDirection = Vector2.zero;

    private float CurrentBeamLerp = 0;
    private int FrameDelay = 10;
    private float BeamDuration = 0;

    private WindFlow CurrentWind;
    private int WindFrameDelay;
    private int CurrentWindSegmentIndex;
    private float CurrentWindSegmentLerp;
    private bool CurrentWindGoToEnd;


    private bool DoRepulse = false;
    public void Repulse(float dir) { StartCoroutine(RepulseCoroutine(dir)); }
    private IEnumerator RepulseCoroutine(float dir)
    {
        DoRepulse = true;
        rb.velocity = new Vector2(dir, rb.velocity.y);
        yield return new WaitForSeconds(0.1f);
        DoRepulse = false;
    }
    void FixedUpdate()
    {
        // rb.AddForce(direction * MoveForce);
        if(!DoRepulse && State == PlayerState.Regular) rb.velocity = new Vector2(direction.x * MoveForce, rb.velocity.y);
    }

    public void IgnoreInputDirection(bool isLeftDirection, bool startIgnoring)
    {
        if(isLeftDirection) ignoreLeft = startIgnoring;
        else                ignoreRight = startIgnoring;
    }

    bool ignoreLeft = false;
    bool ignoreRight = false;

    private bool GetKeyDownJump() => Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space);
    private bool GetKeyUpJump() => Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.Space);

    private IEnumerator BrieflyIncreaseSpeed()
    {
        MoveForce = DefaultMoveForce * 1.3f;
        yield return new WaitForSeconds(0.4f);
        MoveForce = DefaultMoveForce;
    }

    void Update()
    {
        if(State != PlayerState.Regular) { if(direction != Vector2.zero) PlayerAnimator.Play("PlayerIdle"); direction = Vector2.zero; }
        if(State == PlayerState.Regular)
        {
            direction = Vector2.right * Input.GetAxisRaw("Horizontal");
            if(direction.x > 0 && ignoreRight) direction = Vector2.zero;
            if(direction.x < 0 && ignoreLeft) direction = Vector2.zero;

            if(direction.x > 0) transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            else if(direction.x < 0) transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);

            if(prevDirection == Vector2.zero && direction != Vector2.zero) PlayerAnimator.Play("PlayerWalk");
            else if(prevDirection != Vector2.zero && direction == Vector2.zero) PlayerAnimator.Play("PlayerIdle");
            prevDirection = direction;

            // transform.Translate(direction * Time.deltaTime * MoveForce);

            if(GetKeyDownJump()) JumpBuffer = true;
            if(GetKeyUpJump()) JumpBuffer = false;

            if((JumpBuffer || GetKeyDownJump()) && CanJump)
            {
                if(JumpBuffer && JumpBufferFrame > 0) { JumpBufferFrame--; return; }
                JumpBufferFrame = 1;
                var lightningJumpForce = JumpForce * 1.3f;

                var force = lightningBoost ? lightningJumpForce : JumpForce;
                if(lightningBoost) 
                {
                    if(LightningBoost.WhichToLightUp != null) LightningBoost.WhichToLightUp.Lightup(HexColor.Get(0xffff00ff), 0.15f, 2);
                    StartCoroutine(BrieflyIncreaseSpeed());
                }
                CanJump = false;
                lightningBoost = false;
                JumpBuffer = false;
                rb.velocity = Vector2.up * force;
            }
        }
        else if(State == PlayerState.Beam)
        {
            rb.velocity = Vector2.zero;
            if(FrameDelay > 0) { FrameDelay--; return; }
            BeamDuration += Time.deltaTime;

            var mdx = 0; //Input.GetAxisRaw("Mouse X");
            var mdy = Input.GetAxisRaw("Mouse Y");
            var delta = mdx + mdy;


            CurrentBeamLerp += delta * 0.07f;

            float thresh = 0.4f;
            float extraJumpTime = 0.18f;
            if(CurrentBeamLerp < 0 - thresh) 
            {
                if(LockCameraForSomeTimeCoroutine != null) { StopCoroutine(LockCameraForSomeTimeCoroutine); LockCameraForSomeTimeCoroutine = null; }
                StartCoroutine(LockCameraForSomeTime());

                ChangeToRegular();
                transform.position = CurrentBeam.BottomPosition;
                var force = BeamDuration < extraJumpTime ? 35 : 15;
                if(rb.velocity.y < 0) rb.velocity = Vector3.zero;
                rb.AddForce(new Vector3(Random.Range(-5, 5.0f), force, 0), ForceMode2D.Impulse);
                StartCoroutine(QuickToggleGround());
                CanJump = false;
                if(BeamDuration >= extraJumpTime) StartCoroutine(TempReduceGravity());
                else StartCoroutine(SlowTimeTemp());
                return;
            }
            else if(CurrentBeamLerp > 1 + thresh)
            {
                if(LockCameraForSomeTimeCoroutine != null) { StopCoroutine(LockCameraForSomeTimeCoroutine); LockCameraForSomeTimeCoroutine = null; }
                StartCoroutine(LockCameraForSomeTime());

                ChangeToRegular();
                transform.position = CurrentBeam.TopPosition;
                var force = BeamDuration < extraJumpTime ? 35 : 15;
                if(rb.velocity.y < 0) rb.velocity = Vector3.zero;
                rb.AddForce(new Vector3(Random.Range(-5, 5.0f), force, 0), ForceMode2D.Impulse);
                StartCoroutine(QuickToggleGround());
                CanJump = false;
                if(BeamDuration >= extraJumpTime) StartCoroutine(TempReduceGravity());
                else StartCoroutine(SlowTimeTemp());
                return;
            }

            var cl = Mathf.Clamp01(CurrentBeamLerp);
            CurrentBeam.SetPlayerLerp(1 - Mathf.Clamp01(CurrentBeamLerp), Mathf.Abs(delta * 0.07f));
        }
        else if(State == PlayerState.Wind)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            if(WindFrameDelay > 0) WindFrameDelay--;

            float windSpeed = CurrentWind.Speed;
            CurrentWindSegmentLerp += Time.deltaTime * windSpeed;
            if(CurrentWindSegmentLerp > 1)
            {
                CurrentWindSegmentLerp = 0;
                CurrentWindSegmentIndex += CurrentWindGoToEnd ? 1 : -1;

                if(CurrentWindSegmentIndex < 0 || CurrentWindSegmentIndex >= CurrentWind.PartsForward.Count)
                {
                    rb.velocity = Vector3.zero;
                    ChangeToRegular();
                    return;
                }
            }

            var currentSegment = CurrentWind.PartsForward[CurrentWindSegmentIndex];
            var segmentToMouse = cam.ScreenToWorldPoint(Input.mousePosition) - currentSegment.transform.position;
            var dotProduct = Vector2.Dot(segmentToMouse.v2(), currentSegment.up.v2());
            var mouseUp = dotProduct > 0;

            var playerUpDirection = mouseUp ? currentSegment.up : -currentSegment.up;
            transform.up = playerUpDirection;

            var sign = CurrentWindGoToEnd ? 1 : -1;
            transform.position =  
                Vector3.Lerp(
                    currentSegment.position - sign * currentSegment.right * WindFlow.EndOffset, 
                    currentSegment.position + sign * currentSegment.right * WindFlow.EndOffset, 
                    CurrentWindSegmentLerp);
            transform.position = transform.position + playerUpDirection;

            if(Input.GetMouseButtonDown(0) && WindFrameDelay <= 0)
            {
                rb.velocity = Vector3.zero;
                ChangeToRegular();

                var isGlobalUp = Vector2.Dot(playerUpDirection, Vector3.up) > 0;

                // TODO: this doesnt exactly work, gotta fix
                var forward = ((CurrentWindGoToEnd && isGlobalUp) || (CurrentWindGoToEnd && !isGlobalUp)) ? Vector3.right : Vector3.left;
                StartCoroutine(ExitWindCoroutine(isGlobalUp ? Vector2.up : Vector2.down, forward));
            }
        }
        else if(State == PlayerState.Locked)
        {
            direction = Vector3.zero;
        }
    }

    private IEnumerator ExitWindCoroutine(Vector2 up, Vector2 forward)
    {
        var oldGravityScale = defaultGravity;
        // pretty much locks player input
        if(up.y < 0) rb.gravityScale = -rb.gravityScale;
        DoRepulse = true;
        rb.AddForce(up * 28, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f);
        rb.gravityScale = 0;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(forward * 20, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        rb.gravityScale = oldGravityScale;
        DoRepulse = false;
    }

    private IEnumerator TempReduceGravity()
    {
        CanJump = false;
        var oldGravity = defaultGravity;
        yield return new WaitForSeconds(0.2f);

        float timer = 0;
        float maxTime = 0.5f;
        while(timer < maxTime && !CanJump)
        {
            timer += Time.deltaTime;
            yield return null;

            rb.gravityScale = oldGravity * 0.1f;
        }
        rb.gravityScale = oldGravity;
    }

    private IEnumerator QuickToggleGround()
    {
        BottomDetector.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        BottomDetector.gameObject.SetActive(true);
    }

    private Coroutine LockCameraForSomeTimeCoroutine;
    private IEnumerator LockCameraForSomeTime()
    {
        yield return null;
        CameraFollow.Main.StrictFix = true;
        yield return new WaitForSeconds(1);
        CameraFollow.Main.StrictFix = false;
    }

    void ChangeToRegular()
    {
        this.GetComponent<BoxCollider2D>().enabled = true;
        JumpBuffer = false;
        ignoreLeft = false;
        ignoreRight = false;
        transform.rotation = Quaternion.identity;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.SetCursor(CustomCursor, Vector2.zero, CursorMode.Auto);
        PlayerSprite.enabled = true;
        PlayerAnimator.Play("PlayerIdle");
        if(PlayerVanishCoroutine != null) { StopCoroutine(PlayerVanishCoroutine); PlayerVanishCoroutine = null; }

        if(CurrentBeam != null) 
        {
            CurrentBeam.StartIdle();
        }

        State = PlayerState.Regular;
        CameraFollow.Main.StrictFix = false;
        CameraFollow.Main.Target = () => transform.position;
        CanJump = false;
    }

    void ChangeToBeam(LightBeam beam, float lerp)
    {
        PlayerAnimator.Play("PlayerIdle");
        this.GetComponent<BoxCollider2D>().enabled = false;
        CurrentBeam = beam;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        State = PlayerState.Beam;
        CurrentBeamLerp = lerp;
        CurrentBeam.SetPlayerLerp(1 - lerp, 1f);
        FrameDelay = 3;
        BeamDuration = 0;
        PlayerVanishCoroutine = StartCoroutine(PlayerVanish());
        CameraFollow.Main.StrictFix = true;
        CameraFollow.Main.Target = () => Vector3.Lerp(CurrentBeam.BottomPosition, CurrentBeam.TopPosition, CurrentBeamLerp);
        CanJump = false;
    }

    private IEnumerator SlowTimeTemp()
    {
        var slowedTimeScale = 0.7f;
        Time.timeScale = slowedTimeScale;
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1f;
    }

    void ChangeToWind(WindFlow wind, bool isEnd)
    {
        PlayerAnimator.Play("PlayerIdle");
        WindFrameDelay = 6;
        CurrentWind = wind;
        CurrentWindSegmentIndex = isEnd ? wind.PartsForward.Count - 1 : 0;
        CurrentWindSegmentLerp = 0;
        CurrentWindGoToEnd = !isEnd;

        State = PlayerState.Wind;
    }

    private Coroutine PlayerVanishCoroutine;
    private IEnumerator PlayerVanish()
    {
        PlayerAnimator.Play("PlayerVanish");
        yield return new WaitForSeconds(0.85f);
        PlayerSprite.enabled = false;
    }

    public void ClickWind(WindFlow wind)
    {
        if(State != PlayerState.Regular) return;
        var req = 3.3f;
        var sd = (wind.StartPoint - transform.position).magnitude;
        var ed = (wind.EndPoint - transform.position).magnitude;
        Debug.Log("click");

        if(sd > req && ed > req) 
        {
            return;
        }

        var isEnd = ed < req && ed < sd;
        if(!isEnd && sd > req) 
        {
            return;
        }

        ChangeToWind(wind, isEnd);
    }

    public void ClickBeam(LightBeam beam)
    {
        if(State != PlayerState.Regular) return;
        var reqDistance = 3.2f;
        var td = (beam.TopPosition - transform.position).magnitude;
        var bd = (beam.BottomPosition - transform.position).magnitude;

        if(td > reqDistance && bd > reqDistance) 
        {
            return;
        }

        var isTop = td < reqDistance && transform.position.y > (beam.TopPosition.y - 0.5f) && td < bd;
        if(!isTop && bd > reqDistance) 
        {
            return;
        }

        ChangeToBeam(beam, isTop ? 1 : 0);
    }
}
