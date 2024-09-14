using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Game;
    public Slider happinessBar, fuelBar;
    public List<Sprite> itemSprites;
    public Transform leftContainer, rightContainer;
    public PlayerController player;
    public AnimationCurve difficultyCurve;
    public int[] upgradeMilestones;
    public AudioData purchaseSFX;
    public TMP_Text upgradeCounter, orderCounter;
    public GameObject upgradeUI;
    public float fuelLoss = 1.5f;
    public float happinessLoss = 2;
    public float coalRefill = 50;

    public float timer;
    private int ordersCompleted = 0;
    private float happiness = 100;
    private float fuel = 100;
    private CustomerNeeds[] customers;
    private int queuedOrders = 0;
    int milestoneIdx = 0;

    private void Awake()
    {
        if (Game != null && Game != this)
        {
            Destroy(gameObject);
            return;
        }
        Game = this;
        customers = GameObject.FindObjectsOfType<CustomerNeeds>();
        upgradeCounter.text = "Next Upgrade in " + upgradeMilestones[0] + " orders";
        orderCounter.text = "Orders Filled: 0";
    }

    IEnumerator newOrder(float delay)
    {
        yield return new WaitForSeconds(delay);
        List<CustomerNeeds> inactive = new List<CustomerNeeds>();
        foreach (CustomerNeeds customer in customers)
            if (customer.need == "" && customer.isCustomer)
                inactive.Add(customer);

        --queuedOrders;
        if (inactive.Count > 0)
        {
            CustomerNeeds selected = inactive[Random.Range(0, inactive.Count)];
            selected.DetermineNeed();
        }
    }

    public void orderCompleted()
    {
        ++ordersCompleted;
        if (ordersCompleted == upgradeMilestones[milestoneIdx])
        {
            ++milestoneIdx;
            upgradeMilestones[milestoneIdx] += upgradeMilestones[milestoneIdx - 1];
            upgradeUI.SetActive(true);
            Time.timeScale = 0;
        }
        upgradeCounter.text = "Next Upgrade in " + (upgradeMilestones[milestoneIdx] - ordersCompleted) + " orders";
        orderCounter.text = "Orders Filled: " + ordersCompleted;
    }

    public void speedUpgrade()
    {
        Time.timeScale = 1;
        upgradeUI.SetActive(false);
        AudioManager.PlayOneShotAudio(purchaseSFX);
        player.speed += 3;
    }

    public void capacityUpgrade()
    {
        Time.timeScale = 1;
        upgradeUI.SetActive(false);
        AudioManager.PlayOneShotAudio(purchaseSFX);
        ++player.carryCapacity;
    }

    private void Update()
    {
        fuel = Mathf.Max(0, fuel - fuelLoss * Time.deltaTime);
        fuelBar.normalizedValue = fuel / 100;
        if (fuel <= 0)
        {
            happiness -= happinessLoss * Time.deltaTime;
            happinessBar.normalizedValue = happiness / 100;
            if (happiness <= 0)
                gameOver();
        }

        timer += Time.deltaTime;
        int goal = (int)difficultyCurve.Evaluate(timer);
        int current = 0;
        foreach (CustomerNeeds customer in customers)
            if (customer.need != "" && customer.isCustomer)
                ++current;
        
        while (current + queuedOrders < goal)
        {
            ++queuedOrders;
            StartCoroutine(newOrder(Random.Range(0, Mathf.Max(2, 6 - (goal - current - queuedOrders)))));
        }
    }

    public void shovelCoal()
    {
        fuel = Mathf.Min(100, fuel + coalRefill);
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
            case ("Patty"):
                return itemSprites[3];
        }
        return null;
    }
}
