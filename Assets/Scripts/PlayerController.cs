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
    public int carryCapacity = 1;
    public bool readyToGive = false;
    public bool readyToTake = false;
    public GameObject target;
    public GameObject itemPrefab;
    public Transform itemContainer;
    public TMP_Text prompt, promptB;

    Queue<string> itemNames = new Queue<string>();
    Queue<GameObject> items = new Queue<GameObject>();
    Rigidbody2D rb;
    BoxCollider2D collider;
    float spacePressed = -100;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 vel = rb.velocity;
        vel.x = horizontal * speed;

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow))
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

        if(readyToGive && Input.GetKeyDown(KeyCode.E) && target.GetComponent<CustomerNeeds>().need == GetHeldItem() && items.Count > 0){
            GiveItem(target);
        }

        if(readyToTake && canCarryMore() && Input.GetKeyDown(KeyCode.E)){
            TakeItem(target);
        }

        if (items.Count > 0 && Input.GetKeyDown(KeyCode.Q)){
            DropItem();
        }
        UpdatePrompt();
    }

    public bool canCarryMore()
    {
        return items.Count < carryCapacity;
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
        if (itemNames.Count == 0)
            return "";
        return itemNames.Peek();
    }
    public bool GetReadyToGive()
    {
        return readyToGive;
    }
    public bool GetReadyToTake(){
        return readyToTake;
    }

    private void UpdatePrompt()
    {
        prompt.text = promptB.text = "";
        if (readyToGive && target.GetComponent<CustomerNeeds>().need == GetHeldItem() && items.Count > 0)
            prompt.text = "E to give";
        if (readyToTake && canCarryMore())
            prompt.text = "E to take";
        if (items.Count > 0)
            promptB.text = "Q to drop";
    }

    public void CustomerCollision(GameObject customer){
        readyToGive = true;
        target = customer;
    }

    public void ResourceCollision(GameObject resource){
        readyToTake = true;
        target = resource;
    }

    public void ResetPrompt(){
        readyToTake = false;
        readyToGive = false;
        target = null;
    }

    public void GiveItem(GameObject customer){
        customer.GetComponent<CustomerNeeds>().BecomeSatisfied();
        readyToGive = false;
        target = null;

        DropItem();
    }

    public void TakeItem(GameObject item){
        string name = item.GetComponent<ResourceInfo>().GetResourceName();
        GameObject obj = Instantiate(itemPrefab, itemContainer);
        Sprite sprite = GameManager.Game.getSprite(name);
        obj.GetComponent<SpriteRenderer>().sprite = sprite;
        obj.GetComponent<RectTransform>().sizeDelta = sprite.bounds.size;
        itemNames.Enqueue(name);
        items.Enqueue(obj);
    }

    public void DropItem(){
        Destroy(items.Dequeue());
        itemNames.Dequeue();
    }
}
