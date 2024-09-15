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
    public Storm storm;

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
        upgradeCounter.text = "Upgrade in:\n" + upgradeMilestones[0] + " orders";
        orderCounter.text = "Orders Filled:\n0";
        fuelBar.gameObject.SetActive(false);
    }

    private void Start(){
        StartCoroutine(showTutorial(new string[] {
            ".1",
            "Howdy there partner. You can use WASD  OR  ↑↓←→ to move, and press SPACE to jump.",
            ".4",
            "Now that you’ve got a handle on movin’ let’s get you workin’",
            "These passengers might be needy but this IS yer one an’ only purpose, so I don’t wanna hear no complainin’",
            "Tend to their food (burger icon) and beverage (water icon) needs before they lose their patience with ya.",
            "Make sure you cook the patty (patty icon) from the fridge on the stove. Ain’t no one gettin’ sick on my watch.",
            "Good luck, here comes your first order. Don't keep them waiting, or else!"
        }));
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
                 if (Time.time - lastSpeech >= 0.62f)
                {
                    Debug.Log("play");
                    AudioManager.PlayOneShotAudio(clip);
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
        if (ordersCompleted % 20 == 0)
        {
            StartCoroutine(storm.StartStorm());
        }

        if (ordersCompleted == 12)
        {
            fuelBar.gameObject.SetActive(true);
            StartCoroutine(showTutorial(new string[] {
                "Good smokin’ vamoose! Something’s happened to the conductor!",
                "Looks like you’re gonna have to load the coal from the back to the engine in front!",
                "Make sure to not run out of fuel, or else you risk sending the passengers into a frenzy"
            }));
        }

        if (ordersCompleted == upgradeMilestones[milestoneIdx])
        {
            ++milestoneIdx;
            upgradeMilestones[milestoneIdx] += upgradeMilestones[milestoneIdx - 1];
            upgradeUI.SetActive(true);
            player.lockControls = true;
        }
        upgradeCounter.text = "Upgrade in:\n" + (upgradeMilestones[milestoneIdx] - ordersCompleted) + " orders";
        orderCounter.text = "Orders Filled:\n" + ordersCompleted;
    }

    public void speedUpgrade()
    {
        player.lockControls = false;
        upgradeUI.SetActive(false);
        AudioManager.PlayOneShotAudio(purchaseSFX);
        player.speed += 3;
    }

    public void capacityUpgrade()
    {
        player.lockControls = false;
        upgradeUI.SetActive(false);
        AudioManager.PlayOneShotAudio(purchaseSFX);
        ++player.carryCapacity;

        if (player.carryCapacity == 2)
        {
            StartCoroutine(showTutorial(new string[] {
                "Seems like you’ve got a little upgrade and can carry more than one item now.",
                "But your systems are a little antiquated, so you can only deliver the item at the bottom of your stack.",
                "Make sure you’ve wrapped your shiny metal head around that, lest you gotta feed a burger to the floor."
            }));
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
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
        int goal = (int)(difficultyCurve.Evaluate(timer) * (storm.storming ? 1.5f : 1));
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
        PlayerPrefs.SetInt("Score", ordersCompleted);
        PlayerPrefs.SetInt("Best", Mathf.Max(PlayerPrefs.GetInt("Best"), ordersCompleted));
        SceneManager.LoadScene("GameOver");
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
