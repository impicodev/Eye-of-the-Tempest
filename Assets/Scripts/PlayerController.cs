using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed = 10;
    public float jumpStrength = 10;
    public float jumpBuffer = 0.1f;

    public string heldItem = "";
    public bool readyToGive = false;
    public bool readyToTake = false;
    public GameObject target;
    public GameObject holder;
    public TMP_Text prompt;
    public List<Sprite> itemSprites;

    Rigidbody2D rb;
    BoxCollider2D collider;
    float spacePressed = -100;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        holder.SetActive(false);
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 vel = rb.velocity;
        vel.x = horizontal * speed;

        if (Input.GetKey(KeyCode.Space))
            spacePressed = Time.time;

        if (Time.time - spacePressed <= jumpBuffer) {

            Vector2 pos = rb.position;
            pos.y = collider.bounds.min.y - 0.05f;
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down);
            if (hit.collider != null && !hit.collider.isTrigger && pos.y - hit.point.y < 0.05f)
            {
                spacePressed = -100;
                vel.y = jumpStrength;
            }
        }
        rb.velocity = vel;

        if(readyToGive && Input.GetKey(KeyCode.E)){
            GiveItem(target);
        }

        if(readyToTake && Input.GetKey(KeyCode.E)){
            TakeItem(target);
        }

        if (heldItem != "" && Input.GetKey(KeyCode.Q)){
            DropItem();
        }
    }

    // private void OnTriggerEnter(Collider collider){
    //     print("Trigger entered");
    //     if (collider.gameObject.tag == "Customer"){
    //         //get the need of the customer
    //         string need = collider.gameObject.GetComponent<CustomerNeeds>().GetNeed();
    //         if(need == heldItem){
    //             //Show some prompt
    //             readyToGive = true;
    //             target = collider.gameObject;
    //             prompt.text = "E to give";
    //         }
    //     }
    //     if(collider.gameObject.tag == "Resource" && heldItem == ""){
    //         readyToTake = true;
    //         prompt.text = "E to pick up";
    //         target = collider.gameObject;
    //     }
    // }

    //Get functions
    public string GetHeldItem(){
        return heldItem;
    }
    public bool GetReadyToGive()
    {
        return readyToGive;
    }
    public bool GetReadyToTake(){
        return readyToTake;
    }

    public void CustomerCollision(GameObject customer){
        readyToGive = true;
        target = customer;
        prompt.text = "E to give";
    }

    public void ResourceCollision(GameObject resource){
        readyToTake = true;
        target = resource;
        prompt.text = "E to take";
    }

    public void ResetPrompt(){
        readyToTake = false;
        readyToGive = false;
        target = null;
        prompt.text = "";
    }

    public void GiveItem(GameObject customer){
        customer.GetComponent<CustomerNeeds>().BecomeSatisfied();
        heldItem = "";
        readyToGive = false;
        target = null;
        holder.SetActive(false);
        prompt.text = "";
    }

    public void TakeItem(GameObject item){
        heldItem = item.GetComponent<ResourceInfo>().GetResourceName();
        prompt.text = "Q to drop";
        target = null;
        readyToTake = false;
        holder.SetActive(true);
        SpriteRenderer sr = holder.GetComponent<SpriteRenderer>();
        switch(heldItem){
            case("Water"):
                sr.sprite = itemSprites[0];
                break;
            case("Food"):
                sr.sprite = itemSprites[1];
                break;
            case("Coal"):
                sr.sprite = itemSprites[2];
                break;
        }
    }

    public void DropItem(){
        heldItem = "";
        holder.SetActive(false);
        prompt.text = "";
    }
}
