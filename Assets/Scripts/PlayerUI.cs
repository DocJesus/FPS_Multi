using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform thrusterFuelFill;

    [SerializeField]
    private RectTransform HealthFill;

    private PlayerController playerController;
    private Player player;
    private WeaponManager weapon;

    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private GameObject scoreBoard;

    [SerializeField]
    private Text ammoText;

    private void Update()
    {
        SetFuelAmont(playerController.GetThrusterFuelAmont());
        SetHealthAmount(player.GetHealthPCT());
        SetAmmoAmount(weapon.GetCurrentWeapon().bullet);
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //lance le menu pause
            TogglePauseMenu();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleScoreBoard(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            ToggleScoreBoard(false);
        }
    }

    private void Start()
    {
        PauseMenu.isOn = false;
        pauseMenu.SetActive(false);
    }

    public void SetPlayer(Player _player)
    {
        player = _player;
        playerController = _player.GetComponent<PlayerController>();
        weapon = _player.GetComponent<WeaponManager>(); 
    }

    void SetFuelAmont(float amont)
    {
        thrusterFuelFill.localScale = new Vector3(1, amont, 1);
    }

    void SetHealthAmount(float amount)
    {
        HealthFill.localScale = new Vector3(1, amount, 1);
    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.isOn = pauseMenu.activeSelf;
    }

    public void ToggleScoreBoard(bool active)
    {
        scoreBoard.SetActive(active);
    }

    void SetAmmoAmount(int _ammo)
    {
        ammoText.text = _ammo.ToString();
    }
}
