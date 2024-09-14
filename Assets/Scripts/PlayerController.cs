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
    public Transform sprite;

    Queue<string> itemNames = new Queue<string>();
    Queue<GameObject> items = new Queue<GameObject>();
    Rigidbody2D rb;
    BoxCollider2D collider;
    float spacePressed = -100;
    Animator animator;
    float lastGive = -10;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        animator = sprite.GetComponent<Animator>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 vel = rb.velocity;
        vel.x = horizontal * speed;
        animator.SetBool("Running", Input.GetAxisRaw("Horizontal") != 0);
        animator.SetBool("Happy", Time.time - lastGive <= 1.5f);
        sprite.localScale = new Vector3(vel.x < 0 ? -1 : 1, 1, 1);

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow))
            spacePressed = Time.time;

        Vector2 pos = rb.position;
        pos.y = collider.bounds.min.y - 0.05f;
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down);
        bool grounded = hit.collider != null && !hit.collider.isTrigger && pos.y - hit.point.y < 0.05f;
        animator.SetBool("Grounded", grounded);

        if (Time.time - spacePressed <= jumpBuffer && grounded) {
            spacePressed = -100;
            vel.y = jumpStrength;
        }
        rb.velocity = vel;

        if(readyToGive && Input.GetKeyDown(KeyCode.E) && target.GetComponent<CustomerNeeds>().need == GetHeldItem() && items.Count > 0){
            GiveItem(target);
        }

        if (readyToTake && Input.GetKeyDown(KeyCode.E) && GetHeldItem() == "Patty" && target.GetComponent<ResourceInfo>().type == "Food" && target.GetComponent<ResourceInfo>().timer == -1)
        {
            AudioManager.PlayOneShotAudio(target.GetComponent<ResourceInfo>().pattyTimerSFX);
            target.GetComponent<ResourceInfo>().timer = 0;
            DropItem();
        }
        else if(readyToTake && canCarryMore() && Input.GetKeyDown(KeyCode.E) && target.GetComponent<ResourceInfo>().available > 0)
        {
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
        if (readyToTake && GetHeldItem() == "Patty" && target.GetComponent<ResourceInfo>().type == "Food" && target.GetComponent<ResourceInfo>().timer == -1)
            prompt.text = "E to cook patty";
        else if (readyToTake && canCarryMore() && target.GetComponent<ResourceInfo>().available > 0)
            prompt.text = "E to take";
        if (readyToGive && target.GetComponent<CustomerNeeds>().need == GetHeldItem() && items.Count > 0)
            prompt.text = "E to give";
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
        if (customer.GetComponent<CustomerNeeds>().isCustomer)
            GameManager.Game.orderCompleted();
        lastGive = Time.time;
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
        if (target.GetComponent<ResourceInfo>().type == "Food")
            target.GetComponent<ResourceInfo>().available--;

        if (target.GetComponent<ResourceInfo>().burgerContainer)
            Destroy(target.GetComponent<ResourceInfo>().burgerContainer.GetChild(0).gameObject);
        AudioManager.PlayOneShotAudio(target.GetComponent<ResourceInfo>().takeSFX);
    }

    public void DropItem(){
        Destroy(items.Dequeue());
        itemNames.Dequeue();
    }
}
