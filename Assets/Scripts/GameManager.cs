using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Game;
    public Slider happinessBar;

    private float happiness = 100;

    private void Awake()
    {
        if (Game != null && Game != this)
        {
            Destroy(gameObject);
            return;
        }
        Game = this;
    }

    public void customerAngered(float happinessLoss)
    {
        happiness -= happinessLoss;
        happinessBar.normalizedValue = happiness / 100;
        if (happiness <= 0)
            gameOver();
    }

    private void gameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
