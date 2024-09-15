using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rainfall : MonoBehaviour
{
    // Update is called once per frame
    public int fallSpeed;
    void Update()
    {
        //Fall!
        gameObject.transform.Translate(new Vector3(-5, fallSpeed, 0) * Time.deltaTime);
        if(gameObject.transform.position.y <= -16){
            gameObject.transform.position = new Vector3(0, 16.5f, 0);
        }
    }
}
