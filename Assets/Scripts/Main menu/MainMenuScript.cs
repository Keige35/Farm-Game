using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    
    public void NewGameButton()
    {
        Debug.Log("new");
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Lvl_1");
    }
    public void LoadGameButton()
    {
        SceneManager.LoadScene("Lvl_1");
    }
}
