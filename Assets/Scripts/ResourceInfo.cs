using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceInfo : MonoBehaviour
{
    public string type;
    // public List<Sprite> sprites; //for when we have actual sprites!
    public List<Color> colors; //temp, for capsule sprites
    public SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        switch(type){
            case("Water"):
                // sr.sprite = sprites[0];
                sr.color = colors[0];
                break;
            case("Food"):
                // sr.sprite = sprites[1];
                sr.color = colors[1];
                break;
            case("Coal"):
                // sr.sprite = sprites[2];
                sr.color = colors[2];
                break;
        }
    }

    public string GetResourceName(){
        return type;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //print("Resource trigger entered");
        //The only thing that should be colliding is the player, but let's have a check anyways.
        if (other.gameObject.tag == "Player")
        {
            PlayerController pc = other.gameObject.GetComponent<PlayerController>();
            if (pc.GetHeldItem() == "")
            {
                pc.ResourceCollision(gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //print("Resource trigger exited");
        if (other.gameObject.tag == "Player")
        {
            PlayerController pc = other.gameObject.GetComponent<PlayerController>();
            if (pc.GetReadyToTake())
            {
                pc.ResetPrompt();
            }
        }
    }
}
