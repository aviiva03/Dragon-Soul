using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int GroundedHash = Animator.StringToHash("Grounded");
    private static readonly int VertVelHash = Animator.StringToHash("VerticalVelocity");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int DodgeHash = Animator.StringToHash("Dodge");
    private static readonly int LightAttack1Hash = Animator.StringToHash("LightAttack1");
    private static readonly int LightAttack2Hash = Animator.StringToHash("LightAttack2");
    private static readonly int HeavyAttackHash = Animator.StringToHash("HeavyAttack");

    private Animator _anim;

    private void Awake() => _anim = GetComponent<Animator>();

    public void SetSpeed(float normalized) =>
        _anim.SetFloat(SpeedHash, normalized, 0.1f, Time.deltaTime);

    public void SetGrounded(bool g) => _anim.SetBool(GroundedHash, g);
    public void SetVerticalVelocity(float v) => _anim.SetFloat(VertVelHash, v);

    public void PlayJump() => _anim.SetTrigger(JumpHash);
    public void PlayDodge() => _anim.SetTrigger(DodgeHash);

    public void PlayLightAttack(int comboIndex) {
        if (comboIndex == 0) _anim.SetTrigger(LightAttack1Hash);
        else _anim.SetTrigger(LightAttack2Hash);
    }

    public void PlayHeavyAttack() => _anim.SetTrigger(HeavyAttackHash);
}