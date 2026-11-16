using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {

    [Header("References")]
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private PlayerCamera _camera;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private PlayerHitbox _hitbox;
    [SerializeField] private Sword _sword;

    [Header("Settings")]
    [SerializeField] private float _maxSlopeAngle = 45f;
    [SerializeField] private float _slopeDownRayLength = 0.5f;

    [Header("Combat Input Settings")]
    [Tooltip("Сколько секунд нужно держать ЛКМ, чтобы сработала тяжелая атака")]
    [SerializeField] private float _heavyAttackHoldTime = 0.3f;

    // Ссылки
    public PlayerStats Stats => _stats;
    public PlayerCamera CameraRef => _camera;
    public PlayerHitbox Hitbox => _hitbox;
    public Sword Sword => _sword;
    public CharacterController CharacterController { get; private set; }
    public PlayerAnimator Animator { get; private set; }

    // Ввод
    private PlayerControls _input;
    private Vector2 _moveInput;
    private bool _jumpPressedThisFrame;
    private bool _dodgePressedThisFrame;
    private bool _jumpReleased;

    // НОВОЕ: Флаги для кнопок
    private bool _interactPressedThisFrame;
    private bool _biyuAbilityPressedThisFrame;

    // Таймеры ввода атаки
    private float _lightAttackHoldTimer;
    private bool _isHoldingAttack;
    private bool _lightAttackReleased;
    private bool _heavyAttackTriggered;

    // Флаги блокировки
    public bool CanMove { get; set; } = true;
    public bool CanRotate { get; set; } = true;

    // Публичные свойства
    public Vector2 MoveInput => _moveInput;
    public bool HasMoveInput => _moveInput.sqrMagnitude > 0.01f;
    public bool IsGrounded { get; private set; }
    public bool HasSword => PlayerProgress.Instance != null && PlayerProgress.Instance.HasSword;

    // Проверки действий
    public bool WantsToJump => _jumpPressedThisFrame && _coyoteTimer > 0f;
    public bool WantsToDodge => _dodgePressedThisFrame && Time.time - _lastDodgeTime > _stats.DodgeCooldown && _stats.HasStamina(_stats.DodgeStaminaCost);
    public bool WantsToSprint => (Mouse.current != null && Mouse.current.rightButton.isPressed) && HasMoveInput && IsGrounded && _stats.HasStamina(1f);

    // Атаки
    public bool WantsLightAttack => HasSword && _lightAttackReleased && !IsAttacking;
    public bool WantsHeavyAttack => HasSword && _isHoldingAttack && _lightAttackHoldTimer >= _heavyAttackHoldTime && !_heavyAttackTriggered && !IsAttacking;

    // НОВОЕ: Свойства для взаимодействия и способности
    public bool WantsToInteract => _interactPressedThisFrame;
    public bool WantsToUseBiyuAbility => _biyuAbilityPressedThisFrame;

    // Состояние движения
    public Vector3 MoveDirection { get; set; }
    public float VerticalVelocity { get; private set; }
    public Vector3 DodgeDirection { get; private set; }
    public float LastFallVelocity { get; private set; }
    public bool IsAttacking { get; private set; }

    // Invulnerability
    private float _invulnerabilityTimer;
    public bool IsInvulnerable => _invulnerabilityTimer > 0f;

    // Таймеры
    private float _coyoteTimer;
    private float _coyoteLockTimer;
    private float _jumpBufferTimer;
    private float _lastDodgeTime = -999f;
    private bool _wasGroundedLastFrame;

    // Комбо
    private int _lightAttackComboIndex;
    private float _comboWindowTimer;
    public int LightAttackComboIndex => _lightAttackComboIndex;
    public bool IsInComboWindow => _comboWindowTimer > 0f;

    // Attack buffer
    private float _attackBufferTimer;
    public bool HasBufferedAttack => _attackBufferTimer > 0f;

    // FSM
    private PlayerStateMachine _stateMachine;
    private PlayerStateFactory _factory;

    private void Awake() {
        CharacterController = GetComponent<CharacterController>();
        Animator = GetComponent<PlayerAnimator>();

        _stats.CurrentStamina = _stats.MaxStamina;
        _stats.LastStaminaUseTime = -999f;

        _inputSetup();

        _stateMachine = new PlayerStateMachine();
        _factory = new PlayerStateFactory(this, _stateMachine);
        _stateMachine.Initialize(_factory.Idle);
    }

    private void _inputSetup() {
        _input = new PlayerControls();

        _input.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _input.Player.Move.canceled += ctx => _moveInput = Vector2.zero;

        _input.Player.Jump.performed += ctx => {
            _jumpPressedThisFrame = true;
            _jumpBufferTimer = _stats.JumpBufferTime;
        };
        _input.Player.Jump.canceled += ctx => _jumpReleased = true;

        _input.Player.Dodge.performed += ctx => _dodgePressedThisFrame = true;

        // АТАКА (ЛКМ)
        _input.Player.LightAttack.started += ctx => {
            _isHoldingAttack = true;
            _lightAttackHoldTimer = 0f;
            _lightAttackReleased = false;
            _heavyAttackTriggered = false;
            _attackBufferTimer = _stats.AttackBufferTime;
        };

        _input.Player.LightAttack.canceled += ctx => {
            _isHoldingAttack = false;
            if (_lightAttackHoldTimer < _heavyAttackHoldTime) {
                _lightAttackReleased = true;
            }
        };

        // НОВОЕ: Взаимодействие и способность
        _input.Player.Interact.performed += ctx => _interactPressedThisFrame = true;
        _input.Player.BiyuAttack.performed += ctx => _biyuAbilityPressedThisFrame = true;
    }

    private void OnEnable() => _input.Player.Enable();
    private void OnDisable() => _input.Player.Disable();

    private void Update() {
        if (_isHoldingAttack) _lightAttackHoldTimer += Time.deltaTime;

        UpdateGrounded();
        UpdateCoyoteTimer();
        UpdateJumpBuffer();
        UpdateAttackBuffer();
        UpdateInvulnerability();
        HandleGravity();

        _stateMachine.Update();

        _stats.RegenStamina(Time.deltaTime);
        UpdateAnimator();

        // Сброс флагов
        _lightAttackReleased = false;
        _jumpPressedThisFrame = false;
        _dodgePressedThisFrame = false;
        _jumpReleased = false;

        // НОВОЕ: Сброс кнопок
        _interactPressedThisFrame = false;
        _biyuAbilityPressedThisFrame = false;
    }

    public void MarkHeavyAttackTriggered() => _heavyAttackTriggered = true;

    private void UpdateGrounded() {
        bool sphereHit = Physics.CheckSphere(_groundCheck.position, _groundCheckRadius, _groundLayer, QueryTriggerInteraction.Ignore);
        if (sphereHit) {
            IsGrounded = true;
            _wasGroundedLastFrame = true;
            return;
        }

        if (_wasGroundedLastFrame && VerticalVelocity <= 0f) {
            if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, _slopeDownRayLength, _groundLayer, QueryTriggerInteraction.Ignore)) {
                if (Vector3.Angle(Vector3.up, hit.normal) <= _maxSlopeAngle) {
                    IsGrounded = true;
                    CharacterController.Move(Vector3.down * 0.5f * Time.deltaTime);
                    return;
                }
            }
        }
        IsGrounded = false;
        _wasGroundedLastFrame = false;
    }

    private void UpdateCoyoteTimer() {
        if (_coyoteLockTimer > 0f) { _coyoteLockTimer -= Time.deltaTime; _coyoteTimer = 0f; return; }
        if (IsGrounded) _coyoteTimer = _stats.CoyoteTime;
        else _coyoteTimer -= Time.deltaTime;
    }

    private void UpdateJumpBuffer() { if (_jumpBufferTimer > 0f) _jumpBufferTimer -= Time.deltaTime; }

    private void UpdateAttackBuffer() {
        if (_attackBufferTimer > 0f) _attackBufferTimer -= Time.deltaTime;
        if (_comboWindowTimer > 0f) _comboWindowTimer -= Time.deltaTime;
        else if (_lightAttackComboIndex > 0 && !IsAttacking) _lightAttackComboIndex = 0;
    }

    private void UpdateInvulnerability() { if (_invulnerabilityTimer > 0f) _invulnerabilityTimer -= Time.deltaTime; }

    public void ResetCoyoteTimer() { _coyoteTimer = 0f; _coyoteLockTimer = 0.1f; }
    public void ResetJumpBuffer() => _jumpBufferTimer = 0f;
    public void GrantInvulnerability(float duration) => _invulnerabilityTimer = Mathf.Max(_invulnerabilityTimer, duration);
    public void ClearInvulnerability() => _invulnerabilityTimer = 0f;

    private void HandleGravity() {
        if (IsGrounded && VerticalVelocity < 0f) VerticalVelocity = -2f;

        float multiplier = 1f;
        if (VerticalVelocity < 0f) multiplier = _stats.FallGravityMultiplier;
        else if (_jumpReleased) multiplier = _stats.LowJumpMultiplier;

        VerticalVelocity += _stats.Gravity * multiplier * Time.deltaTime;
        CharacterController.Move(Vector3.up * VerticalVelocity * Time.deltaTime);
    }

    public void ApplyMovement() {
        if (!CanMove || IsAttacking) {
            MoveDirection = Vector3.zero;
            return;
        }

        Vector3 inputDir = GetCameraRelativeMoveDirection();

        if (CanRotate && inputDir.sqrMagnitude > 0.01f) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputDir), _stats.RotationSpeed * Time.deltaTime);
        }

        float targetSpeed = WantsToSprint ? _stats.RunSpeed : _stats.WalkSpeed;
        float currentSpeed = MoveDirection.magnitude;
        float desiredSpeed = inputDir.magnitude * targetSpeed;
        float newSpeed = Mathf.MoveTowards(currentSpeed, desiredSpeed, _stats.Acceleration * Time.deltaTime);

        if (inputDir.sqrMagnitude > 0.01f) {
            MoveDirection = inputDir.normalized * newSpeed;
        } else {
            MoveDirection = Vector3.MoveTowards(MoveDirection, Vector3.zero, _stats.Acceleration * Time.deltaTime);
        }

        CharacterController.Move(MoveDirection * Time.deltaTime);
    }

    public void MoveCharacter(Vector3 delta) => CharacterController.Move(delta);
    public Vector3 GetCameraRelativeMoveDirection() => _camera.PlanarForward * _moveInput.y + _camera.PlanarRight * _moveInput.x;

    public void SetVerticalVelocity(float v) => VerticalVelocity = v;
    public void SetDodgeDirection(Vector3 dir) => DodgeDirection = dir;
    public void SetLastFallVelocity(float v) => LastFallVelocity = v;
    public void MarkDodgeTime() => _lastDodgeTime = Time.time;
    public void SetAttacking(bool value) => IsAttacking = value;
    public void ConsumeStamina(float amount) => _stats.ConsumeStamina(amount);
    public void StartComboWindow() => _comboWindowTimer = _stats.LightAttackComboWindow;
    public void AdvanceCombo() => _lightAttackComboIndex = (_lightAttackComboIndex + 1) % 2;
    public void ResetCombo() { _lightAttackComboIndex = 0; _comboWindowTimer = 0f; }
    public void ConsumeAttackBuffer() => _attackBufferTimer = 0f;

    public void ForceStopForCinematic() {
        _moveInput = Vector2.zero;
        MoveDirection = Vector3.zero;
        VerticalVelocity = 0f;

        if (Animator != null) {
            Animator.SetSpeed(0f);
            var rawAnim = GetComponent<Animator>();
            if (rawAnim != null) {
                rawAnim.SetFloat("Speed", 0f);
                rawAnim.SetBool("Grounded", true);
                rawAnim.SetFloat("VerticalVelocity", 0f);
            }
        }
    }

    private void UpdateAnimator() {
        float targetSpeed = WantsToSprint ? _stats.RunSpeed : _stats.WalkSpeed;
        float normalized = HasMoveInput ? targetSpeed / _stats.RunSpeed : 0f;

        Animator.SetSpeed(normalized);
        Animator.SetGrounded(IsGrounded);
        Animator.SetVerticalVelocity(VerticalVelocity);
    }

    private void OnDrawGizmosSelected() {
        if (_groundCheck == null) return;
        Gizmos.color = Application.isPlaying ? (IsGrounded ? Color.green : Color.red) : Color.yellow;
        Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
    }
}