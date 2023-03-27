using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIController : MonoBehaviour
{
    [SerializeField] GameObject Start_Screen;
    [SerializeField] GameObject Win_Screen;
    [SerializeField] GameObject Lose_Screen;
    [SerializeField] Image StaminaBar;
    public void Start_The_Game()
    {
        Start_Screen.SetActive(false);
        MainController.instance.NowStatus = GameStatus.Gaming;
    }
    private void Update()
    {
        if (MainController.instance.NowStatus == GameStatus.Win)
        {
            Win_Screen.SetActive(true);
        }
        if (MainController.instance.NowStatus == GameStatus.Lose)
        {
            Lose_Screen.SetActive(true);
        }
        StaminaBar.fillAmount = MainController.instance.GetFillPercentage();
    }
}
