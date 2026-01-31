using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 _moveInput;
    private bool _jumpPressed;
    private bool _jumpHeld;
    private bool _isGrounded;

    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private BoxCollider2D _groundCollider;
    [SerializeField] LayerMask _platformLayerMask;

    [Header("Mask Switch Parameters")]
    [SerializeField] private float _maskSwitchCooldown = 0.25f;
    private float _lastSwitchTime;

    [Header("Player Move Parameters")]
    [SerializeField] private float _movingSpeed = 5;
    [SerializeField] private float _jumpForce = 12f;
    [SerializeField] private float _jumpAirHandlingForce = 33f;
    [SerializeField] private float _xMinVelocity = -20;
    [SerializeField] private float _xMaxVelocity = 20;
    [SerializeField] private float _yMinVelocity = -13;
    [SerializeField] private float _yMaxVelocity = 40;

    [Header("Horizontal Movement Feel")]
    [SerializeField] private float _acceleration = 60f;
    [SerializeField] private float _deceleration = 80f;

    [Header("Footsteps")]
    [SerializeField] private float _stepInterval = 0.4f;
    private float _stepTimer;
    private bool _isJumpTriggered = false;
    private bool _isJumpgHanndlingTriggered = false;

    [Header("Gravity Parameters")]
    [SerializeField] private float _initGravityScale = 5.35f;
    [SerializeField] private float _gravityScaleOnFall = 3.5f;

    [Header("Jump Buffering Parameters")]
    [SerializeField] private float _jumpBufferTime = 0.14f;
    private float _currentJumpBufferTime = 0;

    [Header("Coyote Jump Parameters")]
    [SerializeField] private float _coyoteTime = 0.13f;
    [SerializeField] private float _currentCoyoteTime = 0.0f;
    private bool _hasJump = false;
    private bool _hasStartedCoyoteTimer = false;
    private bool _canJumpHigher = true;
    private bool _canDoubleJump;
    private bool _hasDoubleJumped;

    [Header("Dash Parameters")]
    [SerializeField] private float _dashForce = 20f;
    [SerializeField] private float _dashDuration = 0.15f;
    [SerializeField] private float _dashCooldown = 0.5f;
    private bool _canDash;
    private bool _isDashing;
    private float _lastDashTime;

    [Header("Push Block Parameters")]
    [SerializeField] private float _pushForce = 5f;
    [SerializeField] private float _pushCheckDistance = 0.1f;
    [SerializeField] private LayerMask _pushableLayerMask;
    private bool _canPush;
    private PushableBlock _currentPushedBlock;

    private InputSystem_Actions _input;

    private void InitInput()
    {
        _input = new();
        _input.Player.Jump.started += OnPerformJumpStarted;
        _input.Player.Jump.canceled += OnPerformJumpCanceled;
        _input.Player.Move.performed += OnPerformMove;
        _input.Player.Move.canceled += OnPerformMoveCanceled;
        _input.Player.SwitchMask.performed += OnSwitchMask;
        _input.Player.Dash.started += OnDash;

        _input.Enable();
    }

    private void OnDestroy()
    {
        _input.Player.Jump.started -= OnPerformJumpStarted;
        _input.Player.Jump.canceled -= OnPerformJumpCanceled;
        _input.Player.Move.performed -= OnPerformMove;
        _input.Player.Move.canceled -= OnPerformMoveCanceled;
        _input.Player.SwitchMask.performed -= OnSwitchMask;
        _input.Player.Dash.started -= OnDash;

        _input.Player.Disable();
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _rigidbody.gravityScale = _initGravityScale;
    }

    private void Awake()
    {
        InitInput();
    }

    private void FixedUpdate()
    {
        CheckGround();

        if (_isJumpTriggered)
        {
            Jump();
            _isJumpTriggered = false;
        }

        if (_isJumpgHanndlingTriggered)
        {
            _rigidbody.AddForce(transform.up * _jumpAirHandlingForce, ForceMode2D.Force);
        }

        float targetSpeed = _moveInput.x * _movingSpeed;

        float accel = Mathf.Abs(_moveInput.x) > 0.01f ? _acceleration : _deceleration;

        float newX = Mathf.MoveTowards(_rigidbody.linearVelocity.x, targetSpeed, accel * Time.fixedDeltaTime);

        _rigidbody.linearVelocity = new Vector2(newX, _rigidbody.linearVelocity.y);

        ClampVelocity();

        if (_isDashing)
        {
            ClampVelocity();
            return;
        }

        HandlePush();
    }

    private void Update()
    {
        if (PlayerManager.Instance.IsPlayerDead)
            return;

        HandleFootsteps();

        //_animator.SetFloat("Speed", Mathf.Abs(_moveInput.x));
        //_animator.SetBool("IsGrounded", _isGrounded);

        HandleJumpBuffering();
        HandleCoyoteJump();
        HandleJump();
        HandleGravityChanges();
        HandleFlip();
    }

    private void HandleFootsteps()
    {
        bool isRunning = _isGrounded && Mathf.Abs(_rigidbody.linearVelocity.x) > 0.1f;

        if (!isRunning)
        {
            _stepTimer = 0f;
            return;
        }

        _stepTimer -= Time.deltaTime;

        if (_stepTimer <= 0f)
        {
            //SoundManager.Instance.PlaySound("character_clothes");

            _stepTimer = _stepInterval;
        }
    }

    private void OnPerformMove(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnPerformMoveCanceled(InputAction.CallbackContext ctx)
    {
        _moveInput = Vector2.zero;
    }

    private void OnSwitchMask(InputAction.CallbackContext ctx)
    {
        if (Time.time < _lastSwitchTime + _maskSwitchCooldown)
            return;

        float value = ctx.ReadValue<float>();
        if (Mathf.Abs(value) < 0.1f)
            return;

        _lastSwitchTime = Time.time;
        int direction = value > 0 ? 1 : -1;
        MaskManager.Instance.SwitchMask(direction);
    }

    private void HandleFlip()
    {
        if (Mathf.Abs(_moveInput.x) < 0.01f)
            return;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(_moveInput.x);
        transform.localScale = scale;
    }

    private void CheckGround()
    {
        float extraHeightTest = 0.1f;
        RaycastHit2D raycastHitGround = Physics2D.BoxCast(_groundCollider.bounds.center, _groundCollider.bounds.size - new Vector3(0.1f, 0f, 0f), 0f, Vector2.down, extraHeightTest, _platformLayerMask);
        Color raycastGroundColor = Color.blue;
        Debug.DrawRay(_groundCollider.bounds.center + new Vector3(_groundCollider.bounds.extents.x, 0), Vector2.down * (_groundCollider.bounds.extents.y + extraHeightTest), raycastGroundColor);
        Debug.DrawRay(_groundCollider.bounds.center - new Vector3(_groundCollider.bounds.extents.x, 0), Vector2.down * (_groundCollider.bounds.extents.y + extraHeightTest), raycastGroundColor);
        Debug.DrawRay(_groundCollider.bounds.center - new Vector3(_groundCollider.bounds.extents.x, _groundCollider.bounds.extents.y + extraHeightTest), Vector2.right * (_groundCollider.bounds.extents.x) * 2, raycastGroundColor);

        _isGrounded = raycastHitGround.collider != null;

        if (_isGrounded)
        {
            _canJumpHigher = true;
            _hasDoubleJumped = false;
        }
    }

    #region Rigibody modification related methods
    private void ClampVelocity()
    {
        Vector2 velocity = _rigidbody.linearVelocity;

        velocity.x = Mathf.Clamp(velocity.x, _xMinVelocity, _xMaxVelocity);
        velocity.y = Mathf.Clamp(velocity.y, _yMinVelocity, _yMaxVelocity);

        _rigidbody.linearVelocity = velocity;
    }

    private void HandleGravityChanges()
    {
        // Increase gravity when the player fall, otherwise will set its normal gravity
        if (!_isGrounded && _rigidbody.linearVelocity.y <= 0 && _rigidbody.gravityScale != _gravityScaleOnFall)
        {
            _rigidbody.gravityScale = _gravityScaleOnFall;
        }
        else if (!_isGrounded && _rigidbody.linearVelocity.y > 0 && _rigidbody.gravityScale != _initGravityScale)
        {
            _rigidbody.gravityScale = _initGravityScale;
        }
    }
    #endregion

    #region Dash Methods
    public void EnableDash(bool value)
    {
        _canDash = value;
    }

    private void OnDash(InputAction.CallbackContext ctx)
    {
        if (!_canDash)
            return;

        if (_isDashing)
            return;

        if (Time.time < _lastDashTime + _dashCooldown)
            return;

        StartCoroutine(DashRoutine());
    }
    private System.Collections.IEnumerator DashRoutine()
    {
        _isDashing = true;
        _lastDashTime = Time.time;

        float originalGravity = _rigidbody.gravityScale;
        _rigidbody.gravityScale = 0f;

        Vector2 dashDirection = _moveInput.x != 0
            ? new Vector2(Mathf.Sign(_moveInput.x), 0)
            : new Vector2(transform.localScale.x, 0);

        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.AddForce(dashDirection * _dashForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(_dashDuration);

        _rigidbody.gravityScale = originalGravity;
        _isDashing = false;
    }
    #endregion

    #region Push Blocks Methods
    public void EnablePush(bool value)
    {
        _canPush = value;
    }
    private void HandlePush()
    {
        if (!_canPush || Mathf.Abs(_moveInput.x) < 0.1f)
        {
            ReleasePush();
            return;
        }

        Vector2 direction = new Vector2(Mathf.Sign(_moveInput.x), 0);
        Vector2 origin = (Vector2)transform.position;

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            direction,
            _groundCollider.bounds.extents.x + _pushCheckDistance,
            _pushableLayerMask
        );

        if (hit.collider != null)
        {
            PushableBlock block = hit.collider.GetComponent<PushableBlock>();
            if (block != null)
            {
                _currentPushedBlock = block;
                block.Push(direction, _pushForce);
                return;
            }
        }

        ReleasePush();
    }
    private void ReleasePush()
    {
        if (_currentPushedBlock != null)
        {
            _currentPushedBlock.Stop();
            _currentPushedBlock = null;
        }
    }

    #endregion

    #region Jump Methods
    private void OnPerformJumpStarted(InputAction.CallbackContext ctx)
    {
        _jumpPressed = true;
        _jumpHeld = true;
    }
    private void OnPerformJumpCanceled(InputAction.CallbackContext ctx)
    {
        _jumpHeld = false;
        _canJumpHigher = false;
    }
    private void Jump()
    {
        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0);
        _rigidbody.AddForce(transform.up * _jumpForce, ForceMode2D.Impulse);
        _isGrounded = false;
        _hasJump = true;
        _currentCoyoteTime = 0;

        //SoundManager.Instance.PlaySound("character_jump");
    }
    private void HandleJump()
    {
        if (_isGrounded && _hasJump)
        {
            _hasJump = false;
            //SoundManager.Instance.PlaySound("JumpImpact");
        }

        // Normal jump
        if (_jumpPressed && _isGrounded)
        {
            _isJumpTriggered = true;
            _isGrounded = false;
            _hasJump = true;
        }
        // Double jump
        else if (_jumpPressed && !_isGrounded && _hasJump && _canDoubleJump && !_hasDoubleJumped)
        {
            _hasDoubleJumped = true;
            _isJumpTriggered = true;
        }
        // Air handling
        else if (_jumpHeld && !_isGrounded && _hasJump && _rigidbody.linearVelocity.y > 0 && _canJumpHigher)
        {
            _isJumpgHanndlingTriggered = true;
        }
        else
        {
            _isJumpgHanndlingTriggered = false;
        }

        _jumpPressed = false;
    }
    private void HandleJumpBuffering()
    {
        _currentJumpBufferTime -= Time.deltaTime;

        if (_jumpPressed && !_isGrounded)
        {
            _currentJumpBufferTime = _jumpBufferTime;
        }

        if (_currentJumpBufferTime > 0 && _isGrounded)
        {
            _isJumpTriggered = true;
            _isGrounded = false;
            _hasJump = true;
            _currentJumpBufferTime = 0;
        }
    }
    private void HandleCoyoteJump()
    {
        // Start corote timer when player leave the ground
        if (!_isGrounded && !_hasStartedCoyoteTimer && _rigidbody.linearVelocity.y < 0)
        {
            _currentCoyoteTime = _coyoteTime;
            _hasStartedCoyoteTimer = true;
        }

        // Check jump on coyote timer on
        if (_jumpPressed && _currentCoyoteTime > 0 && !_hasJump)
        {
            _isJumpTriggered = true;
            _isGrounded = false;
            _hasJump = true;
            _currentCoyoteTime = 0;
        }

        // Reset coyotetimer on ground
        if (_isGrounded)
        {
            _hasStartedCoyoteTimer = false;
            _currentCoyoteTime = 0;
        }

        else if (_hasStartedCoyoteTimer)
        {
            _currentCoyoteTime -= Time.deltaTime;
        }
    }
    public void EnableDoubleJump(bool value)
    {
        _canDoubleJump = value;
        _hasDoubleJumped = false;
    }
    public void ModifyJumpForce(float value)
    {
        _jumpForce = value;
    }
    #endregion
}