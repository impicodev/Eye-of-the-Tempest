using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiter : MonoBehaviour
{
    //currently unfinished
    private Transform tr;
    public int direction; //-1 or 1: -1=left-facing, 1=right-facing
    public bool stopped;

    void Start()
    {
        if(tr == null){
            tr = gameObject.GetComponent<Transform>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!stopped){
            tr.Translate(new Vector2(direction, 0));
        }
    }

    IEnumerator TreatCustomer(){
        yield return new WaitForSeconds(3);
        stopped = false;
    }

    IEnumerator WaitAtWall(){
        yield return new WaitForSeconds(1);
        stopped = false;
        direction *= -1;

    }

    public void TurnAround(){
        stopped = true;
        StartCoroutine(WaitAtWall());
    }

    public void EncounterCustomer(){
        StartCoroutine(TreatCustomer());
    }
}
