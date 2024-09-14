using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceInfo : MonoBehaviour
{
    public string type;
    public List<Sprite> sprites; //for when we have actual sprites!
    public SpriteRenderer sr;
    public AudioData takeSFX;
    // Start is called before the first frame update
    void Start()
    {
        switch(type){
            case("Water"):
                sr.sprite = sprites[0];
                break;
            case("Food"):
                sr.sprite = sprites[1];
                break;
            case("Coal"):
                sr.sprite = sprites[2];
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
        if (other.TryGetComponent(out PlayerController player))
        {
            if (player.canCarryMore())
                player.ResourceCollision(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //print("Resource trigger exited");
        if (other.TryGetComponent(out PlayerController player))
        {
            player.ResetPrompt();
        }
    }
}
