using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MenuScript : MonoBehaviour {

    
    public Button optionsMenuBtn;
    public Canvas optionsMenu;
    //public Canvas menuBtns;

    public Button newGameMenuBtn;
    public Button settingsMenuBtn;
    public Canvas settingsMenu;

    public Button creditsMenuBtn;


    public Button quitGameMenuBtn;
    public Canvas quitMenu;

    public Button YesQuitBtn;
    public Button CancelBtn;

    public Button ResumeBtn;

    // Use this for initialization
    void Start ()
    {
        optionsMenu = optionsMenu.GetComponent<Canvas>();
        optionsMenuBtn = optionsMenuBtn.GetComponent<Button>();

        //menuBtns = menuBtns.GetComponent<Canvas>();

        newGameMenuBtn = newGameMenuBtn.GetComponent<Button>();
        quitGameMenuBtn = quitGameMenuBtn.GetComponent<Button>();
        
        ResumeBtn = ResumeBtn.GetComponent<Button>();

        quitMenu = quitMenu.GetComponent<Canvas>();
        YesQuitBtn = YesQuitBtn.GetComponent<Button>();
        CancelBtn = CancelBtn.GetComponent<Button>();

        quitMenu.enabled = false;
        optionsMenu.enabled = false;
        //menuBtns.enabled = false;
        settingsMenu.enabled = false;
    }

    public void QuitPress()
    {
        quitMenu.enabled = true;
        newGameMenuBtn.enabled = false;
        quitGameMenuBtn.enabled = false;
    }


    public void CancelPress()
    {
        quitMenu.enabled = false;
        newGameMenuBtn.enabled = true;
        quitGameMenuBtn.enabled = true;
        settingsMenu.enabled = false;
    }

    public void NewGameMenuPress()
    {
        SceneManager.LoadScene("Main");
    }

    public void QuitGamePress()
    {
        Application.Quit();
    }

    public void OptionsBtnPress()
    {
        optionsMenu.enabled = true;
        //menuBtns.enabled = true;

    }

    public void ResumeGamePress()
    {
        optionsMenu.enabled = false;
        settingsMenu.enabled = false;
        quitMenu.enabled = false;

    }

    public void musicToggle(bool Off)
    {
        if(Off)
            SoundManager.instance.musicSource.Stop();
        else
            SoundManager.instance.musicSource.Play();
    }

    public void soundToggle(bool Off)
    {
        if (Off)
        {
            AudioListener.pause = true;
            AudioListener.volume = 0;
        }
        else
        {
            AudioListener.pause = false;
            AudioListener.volume = 1;
        }
    }


}
