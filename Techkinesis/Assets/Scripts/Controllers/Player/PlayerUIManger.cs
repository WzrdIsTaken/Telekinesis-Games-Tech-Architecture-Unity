using UnityEngine;
using TMPro;

// Handles all player UI stuff!
// (well, apart from the crosshair.. why? because the crosshair is easier to find / adjust in SettingsManager and imo its colour is more of a 'setting') 

public class PlayerUIManger : MonoBehaviour
{
    [Tooltip("Demo UI Health Text")]  // This stuff would be a lot nicer, probably not even text! I envision no bars or anything, just a effect at the edge of the screen that would
    [SerializeField] TMP_Text demoHealthText;

    [Tooltip("Demo UI Energy Text")]  // indicate you hp and energy levels. Ideally something cool and 'shadery'.. However the text makes it really clear whats happening for the demo
    [SerializeField] TMP_Text demoEnergyText;

    HealthModule playerHealthModule;
    EnergyModule playerEnergyModule;

    // Receive the playerHealthModule from PlayerController, set the starting UI values and hook up the events
    public void Setup(HealthModule _playerHealthModule, EnergyModule _playerEnergyModule)
    {
        playerHealthModule = _playerHealthModule;
        playerEnergyModule = _playerEnergyModule;

        SetHealthUI();
        SetEnergyUI();

        playerHealthModule.UpdateHealthUI += SetHealthUI;
        playerEnergyModule.UpdateEnergyUI += SetEnergyUI;
    }

    public void SetHealthUI()
    {
        demoHealthText.text = "HP: " + playerHealthModule.GetHealth() + "/" + playerHealthModule.GetMaxHealth();
    }

    public void SetEnergyUI()
    {
        demoEnergyText.text = "Energy: " + playerEnergyModule.GetEnergy() + "/" + playerEnergyModule.GetMaxEnergy();
    }
}