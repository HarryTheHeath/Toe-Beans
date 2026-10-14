using System;
using UnityEngine;using UnityEngine.Rendering;
using UnityEngine.SceneManagement;




public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject buttonPanel;
    [SerializeField] private GameObject credits;
    [SerializeField] private String sceneName;

    private void Start()
    {
        hideCredits();
    }

    public void StartGame() 
    {
        SceneManager.LoadScene(sceneName);
    }
    
    public void QuitGame() 
    {
        Application.Quit();
    }

    public void showCredits()
    {
        buttonPanel.SetActive(false);
        credits.SetActive(true);
    }
    
    public void hideCredits()
    {
        buttonPanel.SetActive(true);
        credits.SetActive(false);
    }
    
}
