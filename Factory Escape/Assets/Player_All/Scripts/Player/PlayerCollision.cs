using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCollision : MonoBehaviour
{ 
    [HideInInspector]
    public int starSayac = 0; //Oyundajki kapıların açılması için de kullanılmaktadır.
    private int starTotal = 0;
    private bool singleGameObjectTrigger = false;

    [HideInInspector]
    public GameObject animationGameObject;
    public GameObject  fightButtonGameObject;

    private PlayerAnimations _playerAnimations;

    private CharacterController characterController;
    private GameObject enemyTriggerGameObject;
    private void Start() {
        starTotal = GameObject.FindGameObjectsWithTag("Star").Length;
        characterController = GetComponent<CharacterController>();
        _playerAnimations = animationGameObject.GetComponent<PlayerAnimations>();

        fightButtonGameObject.SetActive(false);
    }

    private void Update()
    {
        if (enemyTriggerGameObject != null && !enemyTriggerGameObject.activeInHierarchy)
        {
            _playerAnimations.PlayerFightIdle(false);
            fightButtonGameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Star")) {
            other.gameObject.SetActive(false);
            starSayac++; //Oyunu geçmek için istenilen sayının üstüne çıkmalı.
            Debug.Log(starSayac);
        }
        else if(other.gameObject.CompareTag("Tuzak") && !singleGameObjectTrigger) {
            singleGameObjectTrigger = true; //Player'in aynı anda iki tuzak objesiyle etkileşmesini önler.
            gameObject.GetComponent<Features_Script>().health -= other.GetComponent<Features_Script>().damage;
            
            if(gameObject.GetComponent<Features_Script>().health > 0) {
                characterController.enabled = false;
                gameObject.transform.position = new Vector3(0,1,0);
                characterController.enabled = true;
            
                StartCoroutine(Ie_SingleGameObjectTrigger()); //Player'in aynı anda iki tuzak objesiyle etkileşmesini önler. Değerin tekrar false olması için zaman gerekli.
            }   
            
            _playerAnimations.PlayerFightIdle(false);
            fightButtonGameObject.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("enemy"))
        {
            enemyTriggerGameObject = other.gameObject;
            _playerAnimations.PlayerFightIdle(true);
            fightButtonGameObject.SetActive(true);
        }
    }
    
    private void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("enemy")) {
            _playerAnimations.PlayerFightIdle(false);
            fightButtonGameObject.SetActive(false);
        }
    }

    private IEnumerator Ie_SingleGameObjectTrigger(){
        yield return new WaitForSeconds(0.5f);
        singleGameObjectTrigger = false;
    }
}


