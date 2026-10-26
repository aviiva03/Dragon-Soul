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

    private Animator Anim {
        get {
            if (_anim == null) {
                _anim = GetComponent<Animator>();
                if (_anim == null) _anim = GetComponentInChildren<Animator>();
            }
            return _anim;
        }
    }

    public void SetSpeed(float normalized) {
        if (Anim == null) return;
        Anim.SetFloat(SpeedHash, normalized, 0.1f, Time.deltaTime);
    }

    public void SetGrounded(bool g) {
        if (Anim == null) return;
        Anim.SetBool(GroundedHash, g);
    }

    public void SetVerticalVelocity(float v) {
        if (Anim == null) return;
        Anim.SetFloat(VertVelHash, v);
    }

    public void PlayJump() {
        if (Anim == null) return;
        Anim.SetTrigger(JumpHash);
    }

    public void PlayDodge() {
        if (Anim == null) return;
        Anim.SetTrigger(DodgeHash);
    }

    public void PlayLightAttack(int comboIndex) {
        if (Anim == null) return;
        if (comboIndex == 0) Anim.SetTrigger(LightAttack1Hash);
        else Anim.SetTrigger(LightAttack2Hash);
    }

    public void PlayHeavyAttack() {
        if (Anim == null) return;
        Anim.SetTrigger(HeavyAttackHash);
    }
}