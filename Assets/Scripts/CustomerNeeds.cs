using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerNeeds : MonoBehaviour
{
    public string need = ""; //usually an empty string; either "Food" or "Water" otherwise
    public float happinessLoss = 10; //happiness lost after timer ends (max happiness = 100)
    public AnimationCurve patienceCurve;

    public GameObject needBubble;
    public GameObject parentCar;
    public SpriteRenderer needIcon;
    public Slider needBar;
    public GameObject indicatorPrefab;
    public SpriteRenderer sprite;
    public AudioData satisfiedSFX;

    private GameObject indicator;
    private float timer;
    private float patience;

    // Start is called before the first frame update
    void Start()
    {
        if(parentCar == null){
            parentCar = gameObject.transform.parent.parent.gameObject;
        }
    }

    void Update()
    {
        if (need != "")
        {
            timer += Time.deltaTime;
            needBar.normalizedValue = 1 - timer / patience;
            if (timer >= patience)
            {
                Debug.Log("Customer at " + transform.position + " was angered >:(");
                GameManager.Game.customerAngered(happinessLoss);
                BecomeSatisfied();
            }
        }

        needBubble.GetComponent<SpriteRenderer>().color = new Color(1, 1 - timer / patience, 1 - timer / patience);
        if (sprite.isVisible)
        {
            if (indicator != null)
                Destroy(indicator);
        }
        else if (need != "")
        {
            if (indicator == null)
            {
                bool isLeft = transform.position.x < Camera.main.transform.position.x;
                indicator = Instantiate(indicatorPrefab, isLeft ? GameManager.Game.leftContainer : GameManager.Game.rightContainer);
                indicator.transform.localScale = new Vector3(isLeft ? 1 : -1, 1, 1);
                indicator.transform.GetChild(0).GetComponent<Image>().sprite = GameManager.Game.getSprite(need);
            }
            else
            {
                float distance = Vector2.Distance(transform.position, GameManager.Game.player.transform.position);
                float scale = Mathf.Max(0.4f, 1 - (distance - 13) / 50);
                indicator.GetComponent<RectTransform>().sizeDelta = Vector2.one * 140 * scale;
                indicator.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = Vector2.one * 80 * scale;
                indicator.GetComponent<Image>().color = new Color(1, 1 - timer / patience, 1 - timer / patience);
            }
        }
    }

    public string GetNeed(){
        return need;
    }

    //Ok. Every x seconds there is a possibility that the customer will want something.
    //Possibly this can later be updated to create more variation by creating a range around needDelay.
    public void DetermineNeed(){
        //Roll 0-1; 0=water, 1=food
        int randomRoll = Random.Range(0, 2);
        switch (randomRoll){
            case(0):
                need = "Water";
                break;
            case(1):
                need = "Food";
                break;
        }
        timer = 0;
        patience = patienceCurve.Evaluate(GameManager.Game.timer);
        needBubble.SetActive(true);
        needIcon.sprite = GameManager.Game.getSprite(need);
    }

    public void BecomeSatisfied(){
        AudioManager.PlayOneShotAudio(satisfiedSFX);
        needBubble.SetActive(false);
        need = "";
    }

    private void OnTriggerEnter2D(Collider2D other){
        //print("Trigger entered");
        //The only thing that should be colliding is the player, but let's have a check anyways.
        if (other.TryGetComponent(out PlayerController player)){
            player.CustomerCollision(gameObject);
        }
        //oops lightning bolt time
        if(other.tag == "Lightning"){
            GameManager.Game.customerAngered(5);
            print("Ouch");
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
