using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    public GameObject storm;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player"){
            //Disappear object when player collides. Do nothing else
            storm.GetComponent<Storm>().DestroyLightning(gameObject);
        }
    }

}