using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerNeeds : MonoBehaviour
{
    public string need = ""; //usually an empty string; either "Food" or "Water" otherwise
    public float happinessLoss = 10; //happiness lost after timer ends (max happiness = 100)
    public int needDelay; //cooldown between requests default=10s
    public float angerThreshold; //amount of time before the customer gets angry default=30s

    public GameObject needBubble;
    public GameObject parentCar;
    public SpriteRenderer needIcon;
    public GameObject needBar;
    public List<Sprite> needSprites;
    public GameObject leftIndicatorPrefab, rightIndicatorPrefab;

    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DetermineNeed());
        if(parentCar == null){
            parentCar = gameObject.transform.parent.parent.gameObject;
        }
    }

    void Update()
    {
        if (need != "")
        {
            timer += Time.deltaTime;
            needBar.transform.localScale = new Vector3(1 - timer / angerThreshold, needBar.transform.localScale.y, 1);
            if (timer >= angerThreshold)
            {
                Debug.Log("Customer at " + transform.position + " was angered >:(");
                GameManager.Game.customerAngered(happinessLoss);
                need = "";
                StartCoroutine(DetermineNeed());
            }
        }
    }

    public string GetNeed(){
        return need;
    }

    //Ok. Every x seconds there is a possibility that the customer will want something.
    //Possibly this can later be updated to create more variation by creating a range around needDelay.
    IEnumerator DetermineNeed(){
        yield return new WaitForSeconds(needDelay);
        //Roll 0-5; 0=water, 1=food
        int randomRoll = Random.Range(0, 5);
        switch(randomRoll){
            case(0):
                need = "Water";
                timer = 0;
                needBubble.SetActive(true);
                needIcon.sprite = needSprites[0];
                break;
            case(1):
                need = "Food";
                timer = 0;
                needBubble.SetActive(true);
                needIcon.sprite = needSprites[1];
                break;
            default:
                StartCoroutine(DetermineNeed());
                break;
        }
    }

    public void BecomeSatisfied(){
        needBar.transform.localScale = new Vector3(1f, 0.05f, 0);
        needBubble.SetActive(false);

        StartCoroutine(DetermineNeed());
    }

    private void OnTriggerEnter2D(Collider2D other){
        //print("Trigger entered");
        //The only thing that should be colliding is the player, but let's have a check anyways.
        if (other.gameObject.tag == "Player"){
            PlayerController pc = other.gameObject.GetComponent<PlayerController>();
            if(pc.GetHeldItem() != "" && pc.GetHeldItem() == need){
                pc.CustomerCollision(gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        //print("Trigger exited");
        if (other.gameObject.tag == "Player"){
            PlayerController pc = other.gameObject.GetComponent<PlayerController>();
            if(pc.GetReadyToGive()){
                pc.ResetPrompt();
            }
        }
    }
}
