using System.Collections;
using UnityEngine;

namespace Player_All.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        private CharacterController controller;
        private Vector2 move; //Hareket Hızı
    
        [SerializeField]
        [Header("Debug Menu")]
        private bool keyboardController = false; 
        
        [SerializeField]
        private bool doubleJumpBool = false;
    
        private float doubleJumpDistance;
        [Space(20)]
        public GameObject joystickGameObject;
        private Vector3 playerVelocity;
        private bool groundedPlayer = true;
        private bool realGroundedPlayer = false;
        private bool fakeGroundedPlayer = false;
        private readonly float gravityValue = -9.81f;

        [SerializeField]
        private float playerSpeed = 5.0f;
        [SerializeField]
        private float jumpHeight = 1.25f;

        // Player Animations
        private Animator animator;

        [HideInInspector]
        public GameObject animationGameObject;
        private PlayerAnimations playerAnimations;
        private bool recklessAnimation = false;
        private float recklessAnimationTimer = 0.0f;
    
        private bool hitClimbInfoBool = false;
        private bool hitClimbInfoExitBool = false;
        private RaycastHit hitJumpInfo;
        private RaycastHit hitClimbInfo;
        private RaycastHit hitClimbInfoExit;
        private bool climbingSystemCore = false;

        //Fight System
        private int characterDamage = 0; 
        private int[] damageFightArray;
        private string[] fightNameArray;
        //private bool fightEnter;

        void Start() 
        {
            animator = gameObject.GetComponentInChildren<Animator>();
            playerAnimations = animationGameObject.GetComponent<PlayerAnimations>();
            controller = gameObject.GetComponent<CharacterController>();
        
            //Player Default Collider
            controller.center = new Vector3(0,-0.33f,0);
            controller.height = 1.82f;

            controller.radius = 0.25f; 
            controller.minMoveDistance = 0.0f;
            transform.rotation = Quaternion.Euler(0,-90,0); // Default : Quaternion.Euler(0,-90,0);

            //Double Jump System
            doubleJumpDistance = doubleJumpBool ? 1.4f : 1.35f;
        
            //Fight System
            characterDamage = GetComponent<Features_Script>().damage;
            damageFightArray = GetComponent<Features_Script>().damageFightArray;
            fightNameArray = GetComponent<Features_Script>().fightNameArray;
        }

        // Update is called once per frame
        void Update()
        {
            //Raycast System
            Physics.Raycast(transform.position, Vector3.down, out hitJumpInfo, doubleJumpDistance);
            Physics.Raycast(new Vector3(transform.position.x, transform.position.y-0.35f, transform.position.z), transform.forward, out hitClimbInfo, 0.5F);
            Physics.Raycast(new Vector3(transform.position.x, transform.position.y+0.5f, transform.position.z), transform.forward, out hitClimbInfoExit, 0.5f);
        
            if(Mathf.Abs(gameObject.transform.position.z) > 0.33F) {
                controller.enabled = false;
                transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                Debug.Log("Karakter Düzeltildi!");
                controller.enabled = true;
            }
        
            if (gameObject.GetComponent<Features_Script>().health > 0)
            {
                Move();
                Jump(false);
                Climb();
            }
            else
            {
                Dead();   
            }
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
            playerAnimations.Move(move.x);
            //Recless Animation
            if(move.x == 0) {
                recklessAnimationTimer += Time.deltaTime;
                if(recklessAnimationTimer >= 10.0f) {
                    recklessAnimation = true;
                }
            }
            else {
                recklessAnimationTimer = 0.0f;
                recklessAnimation = false;
            }
            playerAnimations.Reckless(recklessAnimation);
        }

        public void Jump(bool mobileJump) {
            if(groundedPlayer && playerVelocity.y < 0) {
                playerVelocity.y = 0f;
            }
    
            //Jump BEGİN
            if(((fakeGroundedPlayer && Input.GetButtonDown("Jump")) || (fakeGroundedPlayer && mobileJump)) && move.x != 0) {
                if(hitClimbInfo.collider != null) //Bu if ifadesi tırmanma animasyonu daha fazla görünsün diye tasarlanmıştır.
                    hitClimbInfoBool = true; //Jump Animation
                else
                {
                    playerAnimations.Jump(true);
                }

                if (realGroundedPlayer)
                    playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);//Zıplamadaki hareket hızı yani gerçek yerden zıplama ise girer.
                else
                    playerVelocity.y += Mathf.Sqrt(jumpHeight/2 * -3.0f * gravityValue);//Tırmanırkenki hareket hızı yani gerçek yerden zıplama değilse girer.
            
                fakeGroundedPlayer = realGroundedPlayer = false;
            
                //Mobile Jump Button
                mobileJump = false;
            }
            else if (hitJumpInfo.collider != null && hitJumpInfo.collider.CompareTag("floor"))
            {
                fakeGroundedPlayer = realGroundedPlayer = true;
                playerAnimations.Land();
                playerAnimations.Jump(false);
            }
        
            //Fly and Jump Fall ***
            if (hitJumpInfo.collider == null)
            {
                playerAnimations.Fall();
                playerAnimations.Jump(false);
            }
        
            groundedPlayer = controller.isGrounded;
            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }
    
        void Climb() {
            //Climbing Animation
            if(hitClimbInfo.collider != null && hitClimbInfo.collider.CompareTag("floor"))
                hitClimbInfoBool = true;
            else
                hitClimbInfoBool = false;
            if(hitClimbInfoExit.collider != null && hitClimbInfoExit.collider.CompareTag("floor"))
                hitClimbInfoExitBool = true;
            else
                hitClimbInfoExitBool = false;

            playerAnimations.ClimbingExit(hitClimbInfoExitBool, hitClimbInfoBool);
            playerAnimations.Climbing(hitClimbInfoExitBool, hitClimbInfoBool);
            //

            if(hitClimbInfo.collider == null) {//climbingSystemCore ifadesi ile aynı duvardan tırmanmak yerine karşı duvara zıplamayı sağladım.
                climbingSystemCore = true;
            } 
        
            if(hitClimbInfo.collider != null && hitClimbInfoExit.collider != null && !fakeGroundedPlayer && climbingSystemCore) {
                fakeGroundedPlayer = true;//Tırmanırken space tuşuna bastığımızda if işleminden geçsin diye.
                climbingSystemCore = false;
            }
            else if(!hitClimbInfoExitBool && hitClimbInfoBool && gameObject.GetComponent<Features_Script>().health > 0) {
                controller.enabled = false;
                transform.position += (animationGameObject.transform.forward*.88f + animationGameObject.transform.up) * .66f;
                //transform.position = new Vector3(transform.position.x + 0.6f, transform.position.y+.75f, transform.position.z); //Önceki çözüm //Old solution
                controller.enabled = true;
            }
        }

        public void Fight() {
            ///*Player - Enemy hareket ve Fight
            recklessAnimationTimer = 0.0F;
            recklessAnimation = false;
            animator.SetTrigger("Attack");

            /*if(!fightEnter && move.x == 0) {
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
        }*/
            ///*Player - Enemy hareket ve Fight
        }

        bool IsAnimationFinished()
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.normalizedTime >= 1f;
        }

        bool IsAnimationHalf()
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.normalizedTime >= 0.6f;
        }
    
        IEnumerator ie_FightAnimation(RaycastHit hitEnemy, string fightName, int randomAnimation) {
            yield return new WaitUntil(() => IsAnimationHalf());
            if (hitEnemy.collider != null)
            {
                Debug.Log(hitEnemy.collider.tag); //Deneme, Raycast collider hangi objeye değiyor.
                if(hitEnemy.collider.CompareTag("enemy_main"))
                {
                    Features_Script enemyScript = hitEnemy.collider.GetComponentInParent<Features_Script>();
                    if (enemyScript != null)
                    {
                        hitEnemy.collider.GetComponentInParent<Features_Script>().health -= characterDamage + damageFightArray[randomAnimation];
                        //fightEnter = false;   
                    }
                }
            }
        }
    }
}