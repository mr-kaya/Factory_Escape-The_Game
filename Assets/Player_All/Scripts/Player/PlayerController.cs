using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector2 move; //Hareket Hızı
    
    [SerializeField]
    [Header("Debug Menu")]
    private bool keyboardController = false; 

    [SerializeField]
    private bool _doubleJump = false;
    [Space(20)]
    public GameObject joystickGameObject;
    private Vector3 playerVelocity;
    private bool groundedPlayer = true;
    private bool _realGroundedPlayer = false;
    private bool _fakeGroundedPlayer = false;
    private bool resetJump = false;
    private float gravityValue = -9.81f;

    [SerializeField]
    private float playerSpeed = 5.0f;
    [SerializeField]
    private float jumpHeight = 1.25f;

    // Player Animations
    private Animator animator;

    [HideInInspector]
    public GameObject animationGameObject;
    private PlayerAnimations _playerAnimations;
    private bool jumpAnimation = false;
    private bool reclessAnimation = false;
    private float reclessAnimationTimer = 0.0f;
    
    private bool hitInfoBool = false;
    private bool hitInfoExitBool = false;
    private RaycastHit hitInfo;
    private RaycastHit hitClimbInfo;
    private RaycastHit hitClimbInfoExit;
    private bool climbingSystemCore = false;

    //Fight System
    private int characterDamage = 0; 
    private int[] damageFightArray;
    private string[] fightNameArray;

    void Start() 
    {
        animator = gameObject.GetComponentInChildren<Animator>();
        _playerAnimations = animationGameObject.GetComponent<PlayerAnimations>();
        controller = gameObject.GetComponent<CharacterController>();
        
        //Player Default Collider
        controller.center = new Vector3(0,-0.33f,0);
        controller.height = 1.82f;

        controller.radius = 0.25f; 
        controller.minMoveDistance = 0.0f;
        transform.rotation = Quaternion.Euler(0,-90,0); // Default : Quaternion.Euler(0,-90,0);

        //Fight System
        characterDamage = GetComponent<Features_Script>().damage;
        damageFightArray = GetComponent<Features_Script>().damageFightArray;
        fightNameArray = GetComponent<Features_Script>().fightNameArray;
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Abs(gameObject.transform.position.z) > 0.33F) {
            controller.enabled = false;
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            Debug.Log("Karakter Düzeltildi!");
            controller.enabled = true;
        }

        if(gameObject.GetComponent<Features_Script>().health > 0) {    
            Move();
        }
        else {
            Dead();
        }
        Jump(false);
        Climb();
    }

    private bool deathLoopKarar = true;
    void Dead() {
        if(deathLoopKarar) {
            deathLoopKarar = false;

            animator.Play("Death", 0, 0.0f);
        }
    }

    void Move() {
        //Player 2 Dimension Controller
        if(!keyboardController) {
            move = new Vector2(-joystickGameObject.GetComponent<Joystick>().Horizontal, 0.0f);
        }else {
            move = new Vector2(-Input.GetAxis("Horizontal"), 0.0f); //move = new Vector2(-Input.GetAxis("Horizontal"), 0.0f);
        }
        controller.Move(move * Time.deltaTime * playerSpeed);

        if(move != Vector2.zero) {
            gameObject.transform.forward = move;
        }

        //Climbing Move   
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Climbing") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) {
            controller.Move(Vector3.up * Time.deltaTime * playerSpeed * 0.5f);
        }
        //Climbing Move End

        //Move Animation
        _playerAnimations.Move(move.x);
        //Recless Animation
        if(move.x == 0) {
            reclessAnimationTimer += Time.deltaTime;
            if(reclessAnimationTimer >= 10.0f) {
                reclessAnimation = true;
            }
        }
        else {
            reclessAnimationTimer = 0.0f;
            reclessAnimation = false;
        }
        _playerAnimations.Recless(reclessAnimation);
    }

    public void Jump(bool MobileJump) {
        if(groundedPlayer && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
        }
    
        //Jump BEGİN
        Physics.Raycast(transform.position, Vector3.down, out hitInfo, 1.4F);
        
        if((_fakeGroundedPlayer && Input.GetButtonDown("Jump")) || (_fakeGroundedPlayer && MobileJump)) {
            if(hitClimbInfo.collider != null) //Bu if ifadesi tırmanma animasyonu daha fazla görünsün diye tasarlanmıştır.
                hitInfoBool = true; //Jump Animation
            else
                jumpAnimation = true;

            if(_realGroundedPlayer)
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);//Zıplamadaki hareket hızı yani gerçek yerden zıplama ise girer.
            else
                playerVelocity.y += Mathf.Sqrt(jumpHeight/2 * -3.0f * gravityValue);//Tırmanırkenki hareket hızı yani gerçek yerden zıplama değilse girer.

            _fakeGroundedPlayer = _realGroundedPlayer = false;
            resetJump = true;
            
            //Mobile Jump Button
            MobileJump = false;

            if(!_doubleJump) 
                StartCoroutine(ResetJumpRoutine());
        }
        else if(hitInfo.collider != null) {
            if(resetJump == false || _doubleJump) {
                _fakeGroundedPlayer = _realGroundedPlayer = true;
                jumpAnimation = false; //Jump Animation
            }
        }
        //Jump Animation
        _playerAnimations.Jump(jumpAnimation);
        //Jump END

        groundedPlayer = controller.isGrounded;
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    IEnumerator ResetJumpRoutine() {
        yield return new WaitForSeconds(0.5f);
        resetJump = false;
    }
    
    void Climb() {
        Physics.Raycast(new Vector3(transform.position.x, transform.position.y-0.35f, transform.position.z), transform.forward, out hitClimbInfo, 0.5F);
        Physics.Raycast(new Vector3(transform.position.x, transform.position.y+0.5f, transform.position.z), transform.forward, out hitClimbInfoExit, 0.5f);
        
        //Climbing Animation
        if(hitClimbInfo.collider != null && hitClimbInfo.collider.tag == "floor")
            hitInfoBool = true;
        else
            hitInfoBool = false;
        if(hitClimbInfoExit.collider != null && hitClimbInfoExit.collider.tag == "floor")
            hitInfoExitBool = true;
        else
            hitInfoExitBool = false;

        _playerAnimations.ClimbingExit(hitInfoExitBool, hitInfoBool);
        _playerAnimations.Climbing(hitInfoExitBool, hitInfoBool);
        //

        if(hitClimbInfo.collider == null) {//climbingSystemCore ifadesi ile aynı duvardan tırmanmak yerine karşı duvara zıplamayı sağladım.
            climbingSystemCore = true;
        } 
        
        if(hitClimbInfo.collider != null && hitClimbInfoExit.collider != null && !_fakeGroundedPlayer && climbingSystemCore) {
            _fakeGroundedPlayer = true;//Tırmanırken space tuşuna bastığımızda if işleminden geçsin diye.
            climbingSystemCore = false;
        }
        else if(!hitInfoExitBool && hitInfoBool && gameObject.GetComponent<Features_Script>().health > 0) {
            controller.enabled = false;
            transform.position += (animationGameObject.transform.forward*.88f + animationGameObject.transform.up) * .66f;
            //transform.position = new Vector3(transform.position.x + 0.6f, transform.position.y+.75f, transform.position.z); //Önceki çözüm //Old solution
            controller.enabled = true;
        }
    }

    public bool foreachKarar = false;
    public void Fight() {
        ///*Player - Enemy hareket ve Fight
        
        
         
        reclessAnimationTimer = 0.0F;
        reclessAnimation = false;

        foreachKarar = false;

        foreach (var item in fightNameArray)
        {
            if(animator.GetCurrentAnimatorStateInfo(0).IsName(item)) {
                foreachKarar = true;
            }
        }

        if(!foreachKarar && move.x == 0) {
            int randomAnimation = Random.Range(0, fightNameArray.Length);
            animator.Play(fightNameArray[randomAnimation], 0, 0.0f);
        
            switch(randomAnimation) {
                case 0: Physics.Raycast(new Vector3(transform.position.x, transform.position.y-0.35f, transform.position.z), transform.forward, out hitClimbInfo, 0.75F); break;
                case 1: Physics.Raycast(new Vector3(transform.position.x, transform.position.y-0.35f, transform.position.z), transform.forward, out hitClimbInfo, 0.8F); break;
                case 2: Physics.Raycast(new Vector3(transform.position.x, transform.position.y-0.35f, transform.position.z), transform.forward, out hitClimbInfo, 1.3F); break;
                case 3: Physics.Raycast(new Vector3(transform.position.x, transform.position.y-0.35f, transform.position.z), transform.forward, out hitClimbInfo, 2F); break;
                case 4: break;
                case 5: break;
                case 6: break;
                case 7: break;
                default: break;
            }

            StartCoroutine(ie_FightAnimation(hitClimbInfo, fightNameArray[randomAnimation], randomAnimation));
        }
        ///*Player - Enemy hareket ve Fight
    }

    IEnumerator ie_FightAnimation(RaycastHit hitEnemy, string fightName, int randomAnimation) {
        yield return new WaitForSeconds(.5F);
        //Debug.Log(hitEnemy.collider.tag); //Deneme, Raycast collider hangi objeye değiyor.
        if(animator.GetCurrentAnimatorStateInfo(0).IsName(fightName) && hitEnemy.collider != null && hitEnemy.collider.CompareTag("enemy_main")) {    
            hitEnemy.collider.GetComponentInParent<Features_Script>().health -= characterDamage + damageFightArray[randomAnimation];
            
            Debug.Log("Damage : "+ (characterDamage+damageFightArray[randomAnimation]) ); 
        }
    }
}