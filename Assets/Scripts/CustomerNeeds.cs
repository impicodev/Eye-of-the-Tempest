using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerNeeds : MonoBehaviour
{
    public string need = ""; //usually an empty string; either "Food" or "Water" otherwise
    public int needDelay; //cooldown between requests def=10s
    public float angerThreshold; //amount of time before the customer gets angry def=30s
    public bool angry; //whether the customer is adding to the current anger bar or not

    public GameObject needBubble;
    public GameObject parentCar;
    public SpriteRenderer needIcon;
    public GameObject needBar;
    public List<Sprite> needSprites;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DetermineNeed());
        if(parentCar == null){
            parentCar = gameObject.transform.parent.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    //Ok. Every x seconds there is a possibility that the customer will want something.
    //Possibly this can later be updated to create more variation by creating a range around needDelay.
    IEnumerator DetermineNeed(){
        print("Running DetermineNeed");
        yield return new WaitForSeconds(needDelay);
        //Roll 0-5; 0=water, 1=food
        int randomRoll = Random.Range(0, 5);
        print("Result: "+randomRoll);
        switch(randomRoll){
            case(0):
                need = "Water";
                needBubble.SetActive(true);
                needIcon.sprite = needSprites[0];
                StartCoroutine(LosePatience());
                break;
            case(1):
                need = "Food";
                needBubble.SetActive(true);
                needIcon.sprite = needSprites[1];
                StartCoroutine(LosePatience());
                break;
            default:
                StartCoroutine(DetermineNeed());
                break;
        }
    }

    IEnumerator LosePatience(){
        for(int i=0; i<angerThreshold; i++){
            yield return new WaitForSeconds(1);
            needBar.transform.localScale += new Vector3(1/angerThreshold, 0, 0);
        }
        angry = true;
    }

    public void BecomeSatisfied(){
        angry = false;
        needBar.transform.localScale = new Vector3(0, 0, 0);
        needBubble.SetActive(false);

        StopCoroutine(LosePatience());
        StartCoroutine(DetermineNeed());
    }
}