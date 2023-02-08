using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Door_Star_Open : MonoBehaviour
{
    [SerializeField]
    [Header("Door Open Star Count")]
    private int levelOpenDoorStarCount = 0;
    private int starTotal = 0;
    private bool _kontrol = true;

    [Header("Other")]
    public BoxCollider kapi_kapanma_collider;
    
    [SerializeField]
    private Animator animator;
    public GameObject doorUpGameObject;

    private void Start() {
        animator = GetComponent<Animator>();
        starTotal = GameObject.FindGameObjectsWithTag("Star").Length;
        Debug.Log(levelOpenDoorStarCount);
    }

    private void LateUpdate() {
        try {
            if(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCollision>().starSayac == levelOpenDoorStarCount && _kontrol == true) {
                _kontrol = false;
                Debug.Log("Kapı Açıldı.");
                animator.Play("Open", 0, 0.0f);
                kapi_kapanma_collider.enabled = true;
            }
        }
        catch {
            //Debug.Log("Player'a Ulaşılamıyor."); //GameOver
        }
         
          
    }


    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player") {
            doorUpGameObject.GetComponent<Collider>().enabled = true;
            animator.Play("Close", 0, 0.0f);
        }
    }
}
