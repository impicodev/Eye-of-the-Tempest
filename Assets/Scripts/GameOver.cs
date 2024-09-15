using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOver : MonoBehaviour
{
    public TMPro_Text difText;
    public TMPro_Text hsText;

    // Start is called before the first frame update
    void Start()
    {
        string difficulty = PlayerPrefs.GetString("Difficulty");
        difText.text = "Difficulty: "+difficulty;
        hsText.text = "Orders Completed: "+PlayerPrefs.GetString(difficulty);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
