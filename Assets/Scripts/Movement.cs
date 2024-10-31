
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]

class PlayerController : MonoBehaviour
{
    private TouchingDirections touchingDirections;
    private Rigidbody2D rigidBody { get; set; }
    private AnimationController animationController;
    private int doubleJumpMeter;
    [SerializeField]
    private Text CoinsText;
    [SerializeField]
    private Image hpBarUI;

    [Header("Stats")]

    [SerializeField]
    private float walkSpeed = 10f;

    [SerializeField]
    private float runSpeed = 15f;

    [SerializeField]
    private float jumpSpeed = 10f;

    [SerializeField]
    private float slideSpeed = 3f;

    [SerializeField]
    private float wallSlideLerp = 1.5f;

    [Header("Booleans")]

    [SerializeField]
    private bool canMove;

    [SerializeField]
    private bool wallSlide;

    [SerializeField]
    private bool sliding;

    [SerializeField]
    private bool wallJumped;

    public bool IsRunning { get; private set; }
    public bool IsMoving { get; private set; }

    public bool IsJump { get; private set; }
    private Vector2 moveInput = Vector2.zero;

    private int coins = 0;
    private int HP = 100;
    private int maxHP = 100;
    private void Awake()
    {
        touchingDirections = GetComponent<TouchingDirections>();
        rigidBody = GetComponent<Rigidbody2D>();
        animationController = GetComponent<AnimationController>();
   
    }

    private void Start()
    {
       UpdateCoinsUI();
       UpdateHPBarUI();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleWallSlide();
        HandleGroundedState();
    }
    private void Update()
    {
         if (touchingDirections.IsGround && doubleJumpMeter < 2 || touchingDirections.IsOnWall && doubleJumpMeter < 2)
         {
            doubleJumpMeter = 2;
         }
        /*if (!touchingDirections.IsGround)
        {
            IsJump = true;
        }
        else
        {
            IsJump = false;
        }*/
        UpdateHPBarUI();
       
       
    }
    private void HandleMovement()
    {
        if (!canMove) return;

        float currentSpeed = IsRunning ? runSpeed : walkSpeed;

        if (wallSlide && IsMovingTowardsWall())
        {
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        }
        else if (!wallJumped)
        {
            rigidBody.velocity = new Vector2(moveInput.x * currentSpeed, rigidBody.velocity.y);
        }
        else
        {
            rigidBody.velocity = Vector2.Lerp(rigidBody.velocity, new Vector2(moveInput.x * currentSpeed, rigidBody.velocity.y), wallSlideLerp * Time.fixedDeltaTime);
        }

        if (moveInput.x != 0 && !wallSlide)
        {
            animationController.Flip(moveInput.x);
        }
    }

    private void HandleWallSlide()
    {
        if (!touchingDirections.IsGround && touchingDirections.IsOnWall)
        {
            if (rigidBody.velocity.y > 0)
            {
                wallSlide = false;
                return;
            }

            if (IsMovingTowardsWall())
            {
                StartWallSlide();
            }
            else
            {
                StopWallSlide();
            }
        }
        else
        {
            StopWallSlide();
        }
    }

    private bool IsMovingTowardsWall()
    {
        return (moveInput.x >= 0 && touchingDirections.OnRightWall) || (moveInput.x <= 0 && touchingDirections.OnLeftWall);
    }

    private void StartWallSlide()
    {
        if (!wallSlide)
        {
            rigidBody.velocity = Vector2.zero;
            sliding = true;
        }

        wallSlide = true;
        rigidBody.velocity = new Vector2(0, -slideSpeed);
    }

    private void StopWallSlide()
    {
        wallSlide = false;
        sliding = false;
    }

    private void HandleGroundedState()
    {
        if (touchingDirections.IsGround)
        {
            wallSlide = false;
            sliding = false;
            wallJumped = false;
            IsJump = false;
        }
    }

    private void Jumping(Vector2 dir)
    {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
        rigidBody.AddForce(dir * jumpSpeed, ForceMode2D.Impulse);
    }

    private void WallJump()
    {
        StartCoroutine(DisableMovement(0.05f));

        Vector2 wallDir = touchingDirections.OnRightWall ? Vector2.left : Vector2.right;
        Jumping(Vector2.up + wallDir);

        wallJumped = true;
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        IsMoving = moveInput != Vector2.zero;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && doubleJumpMeter != 0)
        {
            if (touchingDirections.IsGround)
            {
                doubleJumpMeter --;
                Jumping(Vector2.up);
                IsJump = true;
            }
            else if (touchingDirections.IsOnWall)
            {
                WallJump();
            }
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGround && IsJump == false)
        {
            IsRunning = true;
            IsMoving = false;
        }
        else if (context.canceled)
        {
            IsRunning = false;
        }
    }
   
    public void AddCoins(int value = 1)
    {
        coins += value;
       UpdateCoinsUI();
    }
    
    private void UpdateCoinsUI()
    {
        CoinsText.text = $"Coins: {coins}";
    }

    public void UpdateHP(int value)
    {
        HP = Mathf.Clamp(HP + value, 0, maxHP);
        UpdateHPBarUI();
    }

    private void UpdateHPBarUI()
    {
        hpBarUI.fillAmount = Mathf.Clamp(HP / (float)maxHP, 0, 1f);
    }
}