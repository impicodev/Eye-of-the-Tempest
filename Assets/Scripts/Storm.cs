using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storm : MonoBehaviour
{

    public int stormWaitTime;
    public int stormDuration;
    public bool storming = false;
    public GameObject lightning;

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
        StartCoroutine(StrikeLightning());
        //play some kind of rumble sfx to indicate that the storm is beginning
    }

    IEnumerator StrikeLightning()
    {
        for(int i=0; i<stormDuration; i++){
            yield return new WaitForSeconds(1);
            DropLightning();
        }
        EndStorm();
    }

    public void DropLightning(int x=0)
    {
        if(x==0){ //Normal lightning drop - needs to have X set
            x = Random.Range(-45, 45);
        }
        print("Dropping lightning at x="+x);
        GameObject newLightning = Instantiate(lightning, gameObject.transform);
        newLightning.transform.position = new Vector3(x, newLightning.transform.position.y, 0);
    }

    public void EndStorm(){
        storming = false;
        //Also like, run DropLightning one more time to kill a waiter. Implement later
    }

}