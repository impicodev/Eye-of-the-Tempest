using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOver : MonoBehaviour
{
    public TextMeshProUGUI difText;
    public TextMeshProUGUI hsText;

    // Start is called before the first frame update
    void Start()
    {
        string difficulty = PlayerPrefs.GetString("Difficulty");
        difText.text = "Difficulty: "+difficulty;
        hsText.text = "Orders Completed: "+ PlayerPrefs.GetInt(difficulty);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
