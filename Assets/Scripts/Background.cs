using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public int scrollSpeed;

    // Update is called once per frame
    void Update()
    {
        if(gameObject.name == "Landscape" && transform.position.x <= -38.4){
            gameObject.transform.position += new Vector3(38.4f, 0, 0); //dont even piss me off lad why tf is it 38.4 unity
        }
        else if (gameObject.name == "Bushes" && transform.position.x <= -52){
            gameObject.transform.position += new Vector3(52, 0, 0); //dont even piss me off lad why tf is it 38.4 unity
        }
        gameObject.transform.Translate(new Vector3(scrollSpeed, 0, 0)*Time.deltaTime);
    }
}
