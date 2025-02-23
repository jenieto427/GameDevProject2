using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionManager : MonoBehaviour
{
    private UIManager uiManager;
    private PlayerDataManager playerDataManager;
    private NPCWizardAnimationController animationController;
    private NPCAlienTallAnimationController alienAnimationController;


    private void Awake()
    {
        playerDataManager = GameObject.Find("PlayerDataManager").GetComponent<PlayerDataManager>();
        GameObject uiManagerObject = GameObject.Find("UIManager");
        uiManager = uiManagerObject.GetComponent<UIManager>();

        GameObject npcObject = GameObject.Find("Wizard");
        GameObject npcAlienObject = GameObject.Find("Alien_Tall");
        if (SceneManager.GetActiveScene().name == "Village")
        {
            animationController = npcObject.GetComponent<NPCWizardAnimationController>();
            alienAnimationController = npcAlienObject.GetComponent<NPCAlienTallAnimationController>();
        }

    }
    public void enterExitStudyDungeon()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        // Switch between 'StudyDungeon' or 'Village'
        SceneManager.LoadScene(currentSceneName == "StudyDungeon" ? "Village" : "StudyDungeon");
    }
    public void HandleVendorInteraction()
    {
        int cost = 1; // Cost in plant materials for one stimulant
        if (playerDataManager.plantMaterial >= cost)
        {
            playerDataManager.DecreasePlantMaterial(cost); // Reduce samples/money
            playerDataManager.IncrementStimulant(); // Increment stimulant count 
            // Show success messages
            uiManager.UpdateNotificationQueue("Drug addicts make for the best ... scholars?");
            uiManager.UpdateNotificationQueue("You recieved a stimulant");
            animationController.TriggerYes();
        }
        else
        {
            // Show error messages of failed purchase
            uiManager.UpdateNotificationQueue("Not enough samples");
            animationController.TriggerNo();
        }
    }
    public void HandleToolVendorInteraction()
    {
        int cost = 5; // Cost in plant materials for one stimulant
        if (playerDataManager.plantMaterial >= cost)
        {
            playerDataManager.DecreasePlantMaterial(cost); // Reduce samples/money
            playerDataManager.decreaseMinigameEnergyCost(0.5f); // Increment stimulant count 
            // Show success messages
            uiManager.UpdateNotificationQueue("Yeah some tools is gonna save everyone");
            uiManager.UpdateNotificationQueue("Average brain wave cost is now: " + playerDataManager.minigameEnergyCost.ToString("F2"));
            alienAnimationController.TriggerYes();
        }
        else
        {
            // Show error messages of failed purchase
            uiManager.UpdateNotificationQueue("Not enough samples");
            alienAnimationController.TriggerNo();
        }
    }
    public void research()
    {
        int cost = 25; // Cost in plant materials for to upgrade study methods
        if (playerDataManager.plantMaterial >= cost)
        {
            playerDataManager.DecreasePlantMaterial(cost); // Reduce samples/money
            playerDataManager.increaseMinigameSampleReward(2); // Increment stimulant count 
            // Show success messages
            uiManager.UpdateNotificationQueue("Congrats you researched a little...");
            uiManager.UpdateNotificationQueue("Average sample retrieval is now: " + playerDataManager.minigameSampleReward.ToString());
        }
        else
        {
            // Show error messages of failed purchase
            uiManager.UpdateNotificationQueue("Not enough samples");
        }
    }
}

