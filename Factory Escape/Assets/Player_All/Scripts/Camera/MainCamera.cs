using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainCamera : MonoBehaviour
{
    public Vector3 offset;
    [SerializeField]
    private float smoothSpeed = 1f;
    private GameObject Player;
    
    private void Awake() {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z + offset.z);    
    }

    private void LateUpdate() {
        if(Player.activeInHierarchy) {
            Vector3 desiredPosition = Player.transform.position + offset;
            Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed*Time.deltaTime);
            transform.position = smoothPosition;
        }
        else {
            if(transform.position.z < offset.z * 2) {
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + offset.z * Time.deltaTime);
            }
            else {
                StartCoroutine(PlayerDead());
            }
        }
    }

    IEnumerator PlayerDead() {
        yield return new WaitForSeconds(0.75F);
        SceneManager.LoadScene("MainScene");
    }
}
