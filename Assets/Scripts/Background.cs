using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public AnimationCurve scrollSpeed;

    // Update is called once per frame
    void Update()
    {
        if (gameObject.name == "Landscape" && transform.position.x <= -38.4)
        {
            gameObject.transform.position += new Vector3(38.4f, 0, 0); //dont even piss me off lad why tf is it 38.4 unity
        }
        else if (gameObject.name == "Bushes" && transform.position.x <= -52)
        {
            gameObject.transform.position += new Vector3(52, 0, 0); //dont even piss me off lad why tf is it 38.4 unity
        }
        else if (gameObject.name == "Smoke" && transform.position.y >= 15)
            gameObject.transform.position += new Vector3(0, -8.52f, 0);
        gameObject.transform.Translate(new Vector3(scrollSpeed.Evaluate(GameManager.Game.timer), 0, 0)*Time.deltaTime);
    }
}
