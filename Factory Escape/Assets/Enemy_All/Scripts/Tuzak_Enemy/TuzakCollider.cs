using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuzakCollider : MonoBehaviour
{
    // Player ile alakalı; can gitme fonksiyonunu Player'in kendi scripti olan "PlayerCollision.cs" içinde gerçekleştirdim.
    // Çünkü, iki Tuzak objesine aynı anda temas ettiğinde, canı 2x gidecek. Bunu engellemekte baya maliyetli olacaktır.
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("enemy_main")) {
            other.GetComponentInParent<Features_Script>().health -= gameObject.GetComponent<Features_Script>().damage;
        }
    }
}
