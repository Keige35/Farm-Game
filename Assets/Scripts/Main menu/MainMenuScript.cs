using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private Sprite[] tutorialSprites;
    [SerializeField] private Image mainMenuImage;
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject loadButton;
    [SerializeField] private GameObject nextButton;
    private int spritesCount = 0;

    
    public void NewGameButton()
    {
        newGameButton.SetActive(false);
        loadButton.SetActive(false);
        nextButton.SetActive(true);
        ShowTutorial();
        
    }
    public void LoadGameButton()
    {
        SceneManager.LoadScene("Lvl_1");
    }

    public void ShowTutorial()
    {
        if (tutorialSprites != null && spritesCount < tutorialSprites.Length)
        { 
            mainMenuImage.sprite = tutorialSprites[spritesCount];
            spritesCount++;
        } 
        else
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene("Lvl_1");
        }

    }
}
