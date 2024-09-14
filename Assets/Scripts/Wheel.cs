using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public Background bg;
    public float multiplier = 8;

    private void Start()
    {
        transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
    }

    private void Update()
    {
        transform.eulerAngles += new Vector3(0, 0, multiplier * bg.scrollSpeed.Evaluate(GameManager.Game.timer) * Time.deltaTime);
    }
}
