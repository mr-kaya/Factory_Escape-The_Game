using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    private Animator animator;
    private string[] fightArray = {"FightComboPunch", "FightSinglePunch", "kickVer1", "kickVer2"};
    private bool foreachKarar = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerStay(Collider other) {
        if(other.gameObject.CompareTag("enemy")) {
            foreachKarar = false;

            foreach (var item in fightArray)
            {
                if(animator.GetCurrentAnimatorStateInfo(0).IsName(item)) {
                    foreachKarar = true;
                }
            }
            
            if(foreachKarar) {
                Debug.Log("Animasyon Yürütülüyor.");
            }
        }
    }
}
