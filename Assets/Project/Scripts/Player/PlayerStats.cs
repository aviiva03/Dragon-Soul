using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Player/Stats")]
public class PlayerStats : ScriptableObject {

    [Header("Movement")]
    public float WalkSpeed = 4f;
    public float RunSpeed = 8f;
    public float RotationSpeed = 15f;
    public float Acceleration = 12f;

    [Header("Jump & Gravity")]
    public float JumpHeight = 1.5f;
    public float Gravity = -25f;

    [Header("Stamina")]
    public float MaxStamina = 100f;
    public float StaminaDrainPerSec = 15f;
    public float StaminaRegenPerSec = 20f;
    public float RegenDelay = 1f;

    [Header("Dodge")]
    public float DodgeSpeed = 12f;
    public float DodgeDuration = 0.3f;
    public float DodgeCooldown = 0.8f;
    public float DodgeStaminaCost = 25f;

    [Header("Dodge i-Frames")]
    [Tooltip("Сколько секунд с начала рывка игрок неуязвим")]
    public float DodgeInvincibilityDuration = 0.2f;

    [Tooltip("Задержка перед началом i-frames (0 = сразу)")]
    public float DodgeInvincibilityStartDelay = 0f;

    [Header("Game Feel")]
    [Tooltip("Сколько секунд после схода с платформы ещё можно прыгнуть")]
    public float CoyoteTime = 0.15f;
    [Tooltip("Сколько секунд запоминается нажатие прыжка до приземления")]
    public float JumpBufferTime = 0.15f;
    [Tooltip("Множитель гравитации при падении")]
    public float FallGravityMultiplier = 2f;
    [Tooltip("Множитель гравитации при отпускании прыжка")]
    public float LowJumpMultiplier = 3f;

    [Header("Light Attack")]
    public float LightAttack1Duration = 0.4f;
    public float LightAttack2Duration = 0.5f;
    [Tooltip("Окно комбо после удара")]
    public float LightAttackComboWindow = 0.6f;
    public float LightAttackDamage1 = 10f;
    public float LightAttackDamage2 = 15f;

    [Header("Light Attack Movement")]
    public float LightAttackStepDistance = 0.5f;
    public float LightAttackStepDuration = 0.15f;

    [Header("Heavy Attack")]
    public float HeavyAttackWindup = 0.4f;
    public float HeavyAttackImpact = 0.2f;
    public float HeavyAttackRecovery = 0.4f;
    public float HeavyAttackDamage = 30f;
    public float HeavyAttackStaminaCost = 25f;

    [Header("Heavy Attack Movement")]
    public float HeavyAttackStepDistance = 0.8f;
    public float HeavyAttackStepDuration = 0.2f;
    [Tooltip("Замедление во время замаха (0 = стоп, 1 = полная)")]
    public float HeavyAttackWindupMoveMultiplier = 0.2f;

    [Header("Attack Buffer")]
    public float AttackBufferTime = 0.2f;

    [Header("Hitbox")]
    [Tooltip("Радиус хитбокса лёгкой атаки")]
    public float LightAttackHitboxRadius = 0.7f;

    [Tooltip("Радиус хитбокса тяжёлой атаки")]
    public float HeavyAttackHitboxRadius = 1.0f;

    // ---- Runtime ----
    [System.NonSerialized] public float CurrentStamina;
    [System.NonSerialized] public float LastStaminaUseTime;

    public bool HasStamina(float amount) => CurrentStamina >= amount;

    public void ConsumeStamina(float amount) {
        CurrentStamina = Mathf.Max(0f, CurrentStamina - amount);
        LastStaminaUseTime = Time.time;
    }

    public void RegenStamina(float deltaTime) {
        if (Time.time - LastStaminaUseTime < RegenDelay) return;
        CurrentStamina = Mathf.Min(MaxStamina,
            CurrentStamina + StaminaRegenPerSec * deltaTime);
    }

    private void OnValidate() {
        if (DodgeDuration <= 0f) DodgeDuration = 0.3f;
        if (DodgeSpeed <= 0f) DodgeSpeed = 12f;
        if (DodgeCooldown <= 0f) DodgeCooldown = 0.8f;
        if (DodgeInvincibilityDuration <= 0f) DodgeInvincibilityDuration = 0.2f;
        if (DodgeInvincibilityStartDelay < 0f) DodgeInvincibilityStartDelay = 0f;
        if (CoyoteTime <= 0f) CoyoteTime = 0.15f;
        if (JumpBufferTime <= 0f) JumpBufferTime = 0.15f;
        if (FallGravityMultiplier <= 0f) FallGravityMultiplier = 2f;
        if (LowJumpMultiplier <= 0f) LowJumpMultiplier = 3f;
        if (MaxStamina <= 0f) MaxStamina = 100f;
        if (WalkSpeed <= 0f) WalkSpeed = 4f;
        if (RunSpeed <= 0f) RunSpeed = 8f;
        if (Gravity >= 0f) Gravity = -25f;
        if (JumpHeight <= 0f) JumpHeight = 1.5f;
        if (LightAttack1Duration <= 0f) LightAttack1Duration = 0.4f;
        if (LightAttack2Duration <= 0f) LightAttack2Duration = 0.5f;
        if (LightAttackComboWindow <= 0f) LightAttackComboWindow = 0.6f;
        if (AttackBufferTime <= 0f) AttackBufferTime = 0.2f;
        if (HeavyAttackWindup <= 0f) HeavyAttackWindup = 0.4f;
        if (HeavyAttackImpact <= 0f) HeavyAttackImpact = 0.2f;
        if (HeavyAttackRecovery <= 0f) HeavyAttackRecovery = 0.4f;
        if (LightAttackStepDuration <= 0f) LightAttackStepDuration = 0.15f;
        if (HeavyAttackStepDuration <= 0f) HeavyAttackStepDuration = 0.2f;
        if (LightAttackHitboxRadius <= 0f) LightAttackHitboxRadius = 0.7f;
        if (HeavyAttackHitboxRadius <= 0f) HeavyAttackHitboxRadius = 1.0f;
    }
}