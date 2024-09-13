using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Game;
    public Slider happinessBar;
    public List<Sprite> itemSprites;
    public Transform leftContainer, rightContainer;
    public PlayerController player;
    public AnimationCurve difficultyCurve;

    [System.NonSerialized]
    public float timer;
    private float happiness = 100;
    private CustomerNeeds[] customers;
    private int queuedOrders = 0;

    private void Awake()
    {
        if (Game != null && Game != this)
        {
            Destroy(gameObject);
            return;
        }
        Game = this;
        customers = GameObject.FindObjectsOfType<CustomerNeeds>();
    }

    IEnumerator newOrder(float delay)
    {
        yield return new WaitForSeconds(delay);
        List<CustomerNeeds> inactive = new List<CustomerNeeds>();
        foreach (CustomerNeeds customer in customers)
            if (customer.need == "")
                inactive.Add(customer);

        --queuedOrders;
        if (inactive.Count > 0)
        {
            CustomerNeeds selected = inactive[Random.Range(0, inactive.Count)];
            selected.DetermineNeed();
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        int goal = (int)difficultyCurve.Evaluate(timer);
        int current = 0;
        foreach (CustomerNeeds customer in customers)
            if (customer.need != "")
                ++current;
        
        while (current + queuedOrders < goal)
        {
            ++queuedOrders;
            StartCoroutine(newOrder(Random.Range(0, Mathf.Max(2, 6 - (goal - current - queuedOrders)))));
        }
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
        SceneManager.LoadScene("Title");
    }

    public Sprite getSprite(string item)
    {
        switch (item)
        {
            case ("Water"):
                return itemSprites[0];
            case ("Food"):
                return itemSprites[1];
            case ("Coal"):
                return itemSprites[2];
        }
        return null;
    }
}
