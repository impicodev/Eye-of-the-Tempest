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
        hsText.text = "Orders Filled: "+ PlayerPrefs.GetInt("Score") + "\n";
        if (PlayerPrefs.GetInt("Score") == PlayerPrefs.GetInt("Best"))
            hsText.text += "New Record!";
        else
            hsText.text += "Personal Best: " + PlayerPrefs.GetInt("Best");
    }
}
