using System;
using System.Collections;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    private new Rigidbody rigidbody;
    private Animator animator;
    private Vector3 previous;
    private Vector3 playerVector3;
    private bool düşmanÖnündeDüşmanBool = false;

    private static readonly int StandToRun = Animator.StringToHash("StandToRun");
    private static readonly int FightBool = Animator.StringToHash("FightBool");

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        animator.speed = 0.75F;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        playerVector3 = transform.position;

        transform.eulerAngles = new Vector3(0, 90, 0);
    }

    private void Update() {
        Move_V1();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void Move_V1() {
        if(Mathf.Abs(transform.position.z) > 0.5F) {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, transform.position.y, 0);
        }
        
        if(Mathf.Abs(transform.position.x - playerVector3.x) > 1.2F && gameObject.GetComponent<Features_Script>().health > 0)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, new Vector3(playerVector3.x, transform.position.y, 0),
                2 * Time.deltaTime);
        }
        animator.SetFloat(StandToRun, Vector3.Distance(previous, transform.position) / Time.deltaTime);
        previous = gameObject.transform.position;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerVector3 = other.gameObject.transform.position;
            transform.LookAt(new Vector3(playerVector3.x, transform.position.y, transform.position.z));
            if (animator.GetFloat(StandToRun) == 0)
            {
                animator.SetBool(FightBool, true);
            }
            else
            {
                animator.SetBool(FightBool, false);
            }
        }
    }
}