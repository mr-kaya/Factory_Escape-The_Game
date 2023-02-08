using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Animator _animator;
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void Move(float move) {
       _animator.SetFloat("StandToRun", Mathf.Abs(move)); 
    }

    public void Jump(bool jump) {
        _animator.SetBool("RunToJump", jump);
    }

    public void Recless(bool recless) {
        _animator.SetBool("ReclessBool", recless);
    }

    public void Climbing(/*bool climbing*/ bool UpRaycast, bool DownRaycast) {
        //_animator.SetBool("JumpToClimbing", climbing);
        _animator.SetBool("UpRaycast",UpRaycast);
        _animator.SetBool("DownRaycast",DownRaycast);
    }

    public void ClimbingExit(/*bool climbingExit*/ bool UpRaycast, bool DownRaycast) {
        //_animator.SetBool("ClimbToEnd", climbingExit);
        _animator.SetBool("UpRaycast",UpRaycast);
        _animator.SetBool("DownRaycast",DownRaycast);
    }

    public void PlayerFightIdle(bool triggerEnemy) {
        _animator.SetBool("FightBool", triggerEnemy);
    }
}
