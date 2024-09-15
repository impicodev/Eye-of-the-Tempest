using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    public GameObject storm;

    private void Start(){
        storm = gameObject.transform.parent.gameObject;
    }

    private void Update(){
        gameObject.transform.Translate(new Vector3(0, -10, 0) * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player"){
            //Disappear object when player collides. Do nothing else
            storm.GetComponent<Storm>().DestroyLightning(gameObject);
        }
    }

}