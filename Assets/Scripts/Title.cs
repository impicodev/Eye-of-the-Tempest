using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Title : MonoBehaviour
{
    public TMP_Text txt;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("Score"))
        {
            PlayerPrefs.SetInt("Score", 0);
            PlayerPrefs.SetInt("Best", 0);
        }
        txt.text = "Best: " + PlayerPrefs.GetInt("Best");
    }
}
