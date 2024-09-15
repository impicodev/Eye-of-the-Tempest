using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Storm : MonoBehaviour
{

    public int stormWaitTime;
    public int stormDuration;
    public bool storming = false;
    public GameObject overlay;
    public GameObject lightning;
    public List<GameObject> lightningBolts;
    public Sprite stormBG;
    public Sprite normalBG;
    public List<SpriteRenderer> chunks;
    public List<GameObject> rainChunks;

    void Start()
    {
        StartCoroutine(StartStorm()); //Waits stormWaitTime seconds, then starts the storm.

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartStorm()
    {
        yield return new WaitForSeconds(stormWaitTime);
        storming = true;
        overlay.SetActive(true);
        StartCoroutine(StrikeLightning());
        //play some kind of rumble sfx to indicate that the storm is beginning
        //change chunk backgrounds
        foreach(SpriteRenderer chunk in chunks){
            chunk.sprite = stormBG;
        }
        foreach (GameObject rainChunk in rainChunks)
        {
            rainChunk.SetActive(true);
        }
    }

    IEnumerator StrikeLightning()
    {
        for(int i=0; i<stormDuration*2; i++){
            yield return new WaitForSeconds(0.5f);
            DropLightning();
        }
        EndStorm();
    }

    public void DropLightning(int x=0)
    {
        if(x==0){ //Normal lightning drop - needs to have X set
            x = Random.Range(-45, 45);
        }
        // print("Dropping lightning at x="+x);
        GameObject newLightning = Instantiate(lightning, gameObject.transform);
        newLightning.transform.position = new Vector3(x, newLightning.transform.position.y, 0);
        lightningBolts.Add(newLightning);
        StartCoroutine(Lightning(newLightning));
    }

    public void EndStorm(){
        storming = false;
        overlay.SetActive(false);
        //Also like, run DropLightning one more time to kill a waiter. Implement later. Or never I guess :thumbs_up:
        foreach(SpriteRenderer chunk in chunks)
        {
            chunk.sprite = normalBG;
        }
        foreach(GameObject rainChunk in rainChunks)
        {
            rainChunk.SetActive(false);
        }
    }

    IEnumerator Lightning(GameObject thisLightning){
        //Replaces the Lightning.cs script (because that was just messy and broken.) 
        //Translates the lightning down, then destroys it after a certain point.
        while(thisLightning != null){
            if (thisLightning.transform.position.y <= -11)
            {
                DestroyLightning(thisLightning);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void DestroyLightning(GameObject thisLightning){
        lightningBolts.Remove(thisLightning);
        Destroy(thisLightning);
    }

}