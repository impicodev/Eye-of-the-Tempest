using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(new Vector3(0, -10, 0)*Time.deltaTime);
        if(gameObject.transform.position.y <= -11){
            Destroy(gameObject); //I think this hasn't been working but it's ok
        }
    }
}