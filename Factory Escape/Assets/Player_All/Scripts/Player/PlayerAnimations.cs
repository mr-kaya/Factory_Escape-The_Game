using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Animator _animator;
    private static readonly int FightLayer = Animator.StringToHash("FightLayer");
    private static readonly int BaseLayer = Animator.StringToHash("BaseLayer");
    private static readonly int StandToRun = Animator.StringToHash("StandToRun");
    private static readonly int RunToJump = Animator.StringToHash("RunToJump");
    private static readonly int ReclessBool = Animator.StringToHash("ReclessBool");
    private static readonly int UpRaycast = Animator.StringToHash("UpRaycast");
    private static readonly int DownRaycast = Animator.StringToHash("DownRaycast");
    private static readonly int OnTheGround = Animator.StringToHash("OnTheGround");

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void Move(float move) {
       _animator.SetFloat(StandToRun, Mathf.Abs(move)); 
    }

    public void Jump(bool jump) {
        _animator.SetBool(RunToJump, jump);
    }

    public void Fall(bool fall)
    {
        _animator.SetBool(OnTheGround, fall);
    }

    public void Recless(bool recless) {
        _animator.SetBool(ReclessBool, recless);
    }

    public void Climbing(/*bool climbing*/ bool upRaycast, bool downRaycast) {
        //_animator.SetBool("JumpToClimbing", climbing);
        _animator.SetBool(UpRaycast,upRaycast);
        _animator.SetBool(DownRaycast,downRaycast);
    }

    public void ClimbingExit(/*bool climbingExit*/ bool upRaycast, bool downRaycast) {
        //_animator.SetBool("ClimbToEnd", climbingExit);
        _animator.SetBool(UpRaycast,upRaycast);
        _animator.SetBool(DownRaycast,downRaycast);
    }

    public void PlayerFightIdle(bool triggerEnemy) {
        if (triggerEnemy)
        {
            _animator.SetTrigger(FightLayer);
        }
        else
        {
            _animator.SetTrigger(BaseLayer);
        }
        _animator.SetBool("FightBool", triggerEnemy);
    }
}
