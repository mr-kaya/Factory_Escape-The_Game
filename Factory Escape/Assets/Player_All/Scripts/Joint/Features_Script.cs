using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Features_Script : MonoBehaviour
{
    [Header("Features")]
    [SerializeField] [Range(0, 2000)]
    public int health = 75;
    [SerializeField] [Range(0, 5000)]
    public int damage = 50;

    [Header("Only Player")]
    public GameObject playerControllerPanel;

    [HideInInspector]
    public int[] damageFightArray = {10, 15, 25, 40,    50, 75, 90, 110,    120, 150, 220, 400};    
    public string[] fightNameArray = {"FightComboPunch", "FightSinglePunch", "kickVer2", "kickVer1"};
    
    [Header("Only Enemy")]
    public BoxCollider enemyCollisionCollider;
    
    private Animator animator;
    private bool singlePlayAnimation;

    void Start() {
        animator = gameObject.GetComponentInChildren<Animator>();
    
        singlePlayAnimation = true;
    }

    void Update() {
        if(health <= 0) {
            switch(gameObject.tag) {
                case "Player":
                    playerControllerPanel.SetActive(false);
                    break;
                case "enemy":
                    if(singlePlayAnimation) {
                        singlePlayAnimation = false;


                        enemyCollisionCollider.enabled = false;

                        animator.Play("Death", 0, 0.0F);
                        animator.speed = 1.5F;
                    }
                    break;
            }

            StartCoroutine(DeletePrefab());
        } 
    }

    IEnumerator DeletePrefab() {
        yield return new WaitForSeconds(2.75f);
        gameObject.SetActive(false);
    }

}
