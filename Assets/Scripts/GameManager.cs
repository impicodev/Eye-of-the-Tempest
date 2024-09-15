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
    public AudioData purchaseSFX, angrySFX, orderSFX, fuelEmptySFX, fuelFullSFX;
    public AudioData voice1, voice2, voice3, voice4, voice5, voice6, voice7, voice8, voice9;
    public TMP_Text upgradeCounter, orderCounter, tutorialText;
    public GameObject upgradeUI, tutorial, openMouthRobot;
    public float fuelLoss = 1.5f;
    public float happinessLoss = 2;
    public float coalRefill = 50;

    public float timer;
    private int ordersCompleted = 0;
    public float happiness = 100;
    private float fuel = 100;
    private CustomerNeeds[] customers;
    private int queuedOrders = 0;
    int milestoneIdx = 0;
    bool mouseDown = false;

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
        fuelBar.gameObject.SetActive(false);

        StartCoroutine(showTutorial(new string[] {
            "Welcome abord the Loco-Motive!",
            "Use the ARROW keys to move around and SPACE to jump",
            ".4",
            "The passengers on this train are very needy! It's your job to keep them fed and happy",
            "Water is available at the water dispenser, and the burgers are in the fridge. You might need to cook them first, though...",
            "Customer orders will appear as bubbles above their heads and on the sides of the screen. Here comes your first one now!"
        }));
    }

    private void Start(){
        if (!PlayerPrefs.HasKey("Easy")){ //If easy isn't set, none of them are tbh. This should just be for first-time PlayerPrefs setup
            PlayerPrefs.SetInt("Easy", 0);
            PlayerPrefs.SetInt("Medium", 0);
            PlayerPrefs.SetInt("Hard", 0);
        }
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

    IEnumerator showTutorial(string[] messages)
    {
        player.lockControls = true;
        tutorial.SetActive(true);
        foreach (string message in messages) {
            if (message[0] == '.')
            {
                player.lockControls = false;
                tutorial.SetActive(false);
                yield return new WaitForSeconds(int.Parse(message.Substring(1)));
                player.lockControls = true;
                tutorial.SetActive(true);
                continue;
            }

            tutorialText.text = "";
            int cnt = 0;
            mouseDown = false;
            float lastSpeech = -100;
            foreach (char chr in message)
            {
                tutorialText.text += chr;
                yield return new WaitForSeconds(0.04f);
                if (mouseDown)
                {
                    tutorialText.text = message;
                    break;
                }

                AudioData clip = voice1;
                switch (Random.Range(1, 10))
                {
                    case (1):
                        clip = voice1;
                        break;
                    case (2):
                        clip = voice2;
                        break;
                    case (3):
                        clip = voice3;
                        break;
                    case (4):
                        clip = voice4;
                        break;
                    case (5):
                        clip = voice5;
                        break;
                    case (6):
                        clip = voice6;
                        break;
                    case (7):
                        clip = voice7;
                        break;
                    case (8):
                        clip = voice8;
                        break;
                    case (9):
                        clip = voice9;
                        break;
                }
                 if (Time.time - lastSpeech >= 0.6f)
                {
                    AudioManager.PlayOneShotAudio(clip, volumeMultiplier:1.5f);
                    lastSpeech = Time.time;
                }
                if (++cnt == 2)
                {
                    cnt = 0;
                    openMouthRobot.SetActive(!openMouthRobot.activeSelf);
                }
            }
            openMouthRobot.SetActive(true);
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space));
        }
        player.lockControls = false;
        tutorial.SetActive(false);
    }

    public void orderCompleted()
    {
        ++ordersCompleted;
        if (ordersCompleted == upgradeMilestones[milestoneIdx])
        {
            ++milestoneIdx;
            upgradeMilestones[milestoneIdx] += upgradeMilestones[milestoneIdx - 1];
            upgradeUI.SetActive(true);
            player.lockControls = true;
        }
        upgradeCounter.text = "Next Upgrade in " + (upgradeMilestones[milestoneIdx] - ordersCompleted) + " orders";
        orderCounter.text = "Orders Filled: " + ordersCompleted;
    }

    public void speedUpgrade()
    {
        player.lockControls = false;
        upgradeUI.SetActive(false);
        AudioManager.PlayOneShotAudio(purchaseSFX);
        player.speed += 3;

        if (milestoneIdx == 2)
        {
            StartCoroutine(showTutorial(new string[] {
                    "Oh no! The conductor has mysteriously disappeared.",
                    "Grab coal from the back of the train and bring it to the front to replenish fuel.",
                    "If you run out of fuel the passengers will be very unhappy... so stay vigilant"
                }));
            fuelBar.gameObject.SetActive(true);
        }
    }

    public void capacityUpgrade()
    {
        player.lockControls = false;
        upgradeUI.SetActive(false);
        AudioManager.PlayOneShotAudio(purchaseSFX);
        ++player.carryCapacity;

        if (milestoneIdx == 2)
        {
            StartCoroutine(showTutorial(new string[] {
                    "Oh no! The conductor has mysteriously disappeared.",
                    "Grab coal from the back of the train and bring it to the front to replenish fuel.",
                    "If you run out of fuel the passengers will be very unhappy... so stay vigilant"
                }));
            fuelBar.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            mouseDown = true;

        if (player.lockControls) return;
        if (milestoneIdx > 1)
        {
            if (fuel > 0 && fuel - fuelLoss * Time.deltaTime <= 0)
                AudioManager.PlayOneShotAudio(fuelEmptySFX);
            fuel = Mathf.Max(0, fuel - fuelLoss * Time.deltaTime);
        }
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
        if (fuel < 100 && fuel + coalRefill >= 100)
            AudioManager.PlayOneShotAudio(fuelFullSFX);
        fuel = Mathf.Min(100, fuel + coalRefill);
    }

    public void customerAngered(float happinessLoss)
    {
        AudioManager.PlayOneShotAudio(angrySFX);
        happiness -= happinessLoss;
        happinessBar.normalizedValue = happiness / 100;
        if (happiness <= 0)
            gameOver();
    }

    private void gameOver()
    {
        // SceneManager.LoadScene("Title");
        string difficulty = PlayerPrefs.GetString("Difficulty");
        if (PlayerPrefs.GetInt(difficulty) < ordersCompleted)
        {
            PlayerPrefs.SetInt(difficulty, ordersCompleted);
        }
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
