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
    public int available = 1;
    public float pattyCookTime = 5;
    public Slider pattyTimer;
    public AudioData pattyTimerSFX, pattyFinishSFX;
    public GameObject burgerPrefab;
    public Transform burgerContainer;
    public float timer = -1;
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
            case ("Patty"):
                sr.sprite = sprites[3];
                break;
        }
    }

    private void Update()
    {
        if (timer >= 0)
        {
            timer += Time.deltaTime;
            pattyTimer.value = timer / pattyCookTime;
            if (timer >= pattyCookTime)
            {
                timer = -1;
                pattyTimer.value = 0;
                ++available;
                Instantiate(burgerPrefab, burgerContainer);
                AudioManager.PlayOneShotAudio(pattyFinishSFX);
            }
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
