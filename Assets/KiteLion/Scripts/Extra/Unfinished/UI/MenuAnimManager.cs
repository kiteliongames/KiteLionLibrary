using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Threading;
using UnityEngine.UI;

/// <summary>
/// Threading + Anim Hockey = Great Dependencies!
/// Thread watches button or page change, and reacts accordingly.
/// -- Eliot Carney-Seim
/// </summary>
public class MenuAnimManager : MonoBehaviour {

    /// <summary>
    /// On Page Change, the "anim hockey" is thus:
    ///  1. Tell animator to rotate out -> Animator finishes the animation.
    ///  2. Animator tells us, via "UpdateButtonTextHelper" script to update the menu button text.
    ///  3. The "UpdateText" function below updates the text" -> Tells animator to finish.
    ///  4. Animator returns to the screen, with updated menu text.
    ///  
    /// On Button change . . . .
    /// </summary>
    public class StateWatcher
    {

        private MenuManager.PageName currentPage;
        private int currentBtn;
        private MenuManager.PageName prevPage;
        private bool isTracking = true;
        private MenuManager menu;
        private Animator menuAnim;
        private MenuAnimManager parent;

        public StateWatcher(MenuManager menu, Animator menuAnim, MenuAnimManager parent)
        {
            this.menu = menu;
            this.menuAnim = menuAnim;
            this.parent = parent;
            currentPage = MenuManager.PageName.MainMenu;
            prevPage = MenuManager.PageName.None;
        }

        public void TrackPage()
        {
            while (isTracking)
            {
                if(menu.CurrentPageName != currentPage)
                {
                    if (menu.CurrentPageName == prevPage)
                    {
                        currentPage = prevPage;
                        prevPage = MenuManager.PageName.None;
                        parent.doAnimBack = true;
                    }
                    else
                    {
                        prevPage = currentPage;
                        currentPage = menu.CurrentPageName;
                        parent.doAnimForward = true;
                    }
                }
            }
        }

        public void TrackButton()
        {
            while (isTracking)
            {
                int nextBtn = menu.CurrentPage.CurrentBtn;
                if (nextBtn != currentBtn)
                {
                    currentBtn = nextBtn;
                    parent.doNextButtonAnim = true;
                }
            }
        }

        public void stopTracking()
        {
            isTracking = false;
        }
    }

    #region Unity Classes Instancing
    public MenuManager menuMan;
    public Animator MainMenuAnim;
    private EventSystem MainMenuEvSys;
    #endregion

    #region Local Classes Instancing
    private Thread ThreadPageWatch;
    private Thread ThreadButtonWatch;
    private StateWatcher Watch;
    #endregion

    #region In-game Menu Objs
    public GameObject[] MenuHolders;
    private Text[] menuTexts;
    private Button[] menuButtons;
    public Text TitleText;
    #endregion

    #region Private Variable for Anim Checks 
    private bool doAnimForward;
    private bool doAnimBack;
    private bool doNextButtonAnim;
    private bool doButtonTextUpdate;
    #endregion

    public void Start()
    {

        MainMenuEvSys = GetComponentInParent<EventSystem>();

        Watch = new StateWatcher(menuMan, MainMenuAnim, this);

        ThreadPageWatch = new Thread(new ThreadStart(Watch.TrackPage));
        ThreadButtonWatch = new Thread(new ThreadStart(Watch.TrackButton));

        ThreadPageWatch.Start();
        ThreadButtonWatch.Start();

        //BELOW IMPLIES THAT <<MAX_BUTTONS>> GAMEOBJECTS ALWAYS EXIST
        // Yet are simply not always visible.
        menuTexts = new Text[MenuManager.MAX_BUTTONS];
        for(int i = 0; i < MenuManager.MAX_BUTTONS; i++)
        {
            menuTexts[i] = MenuHolders[i].GetComponentInChildren<Text>();
        }

        //SAME HERE but for the button OBJs not the Text OBJs
        menuButtons = new Button[MenuManager.MAX_BUTTONS];
        for (int i = 0; i < MenuManager.MAX_BUTTONS; i++)
        {
            menuButtons[i] = MenuHolders[i].GetComponentInChildren<Button>();
        }

        DoButtonTextUpdate();
    }

    public void Update()
    {
        if (doButtonTextUpdate) //Update text on page change.
        {
            updateText();
            MainMenuAnim.SetTrigger("ButtonsUpdated");
            doButtonTextUpdate = false;
            Debug.Log("UpdateButton Received!");
        }
        if (doAnimBack) //Going to previous button page.
        {
            if (UFE.DEBUG_ON)
                Debug.Log("Activating Anim Back");
            MainMenuAnim.SetTrigger("BackSelect");
            doAnimBack = false;
        }
        if (doAnimForward) // Going to next button page.
        {
            MainMenuAnim.SetTrigger("ForwardSelect");
            doAnimForward = false;
        }
        if (doNextButtonAnim) //Going up or down current visible buttons.
        {
            MainMenuEvSys.SetSelectedGameObject(menuButtons[menuMan.CurrentPage.CurrentBtn].gameObject);
            if (UFE.DEBUG_ON)
                Debug.Log("New Button Set as Selected.");
            doNextButtonAnim = false;
        }
    }

    /// <summary>
    /// Mono should kill headless threads, but of course, just in case.
    /// </summary>
    public void OnDestroy()
    {
        Watch.stopTracking();
        ThreadPageWatch.Abort();
        ThreadPageWatch.Join();
        ThreadButtonWatch.Abort();
        ThreadButtonWatch.Join();
    }

    /// <summary>
    /// Mono should kill headless threads, but of course, just in case.
    /// </summary>
    public void OnApplicationQuit()
    {
        Watch.stopTracking();
        ThreadPageWatch.Abort();
        ThreadPageWatch.Join();
        ThreadButtonWatch.Abort();
        ThreadButtonWatch.Join();
    }

    /// <summary>
    /// Called by another method that's called by the Unity Animation "MenuTrans2"
    /// </summary>
    public void DoButtonTextUpdate()
    {
        doButtonTextUpdate = true;
    }
    
    /// <summary>
    /// Updates all Button text AND the Title text.
    /// </summary>
    private void updateText()
    {
        // instead of updating text, it should enable/disable a page's buttons.
        for (int i = 0; i < MenuManager.MAX_BUTTONS; i++) {
            if (i >= menuMan.CurrentPage.ButtonCount) {
                MenuHolders[i].gameObject.SetActive(false);
            } else {
                MenuHolders[i].gameObject.SetActive(true);
                menuTexts[i].text = menuMan.CurrentPage.PButtons[i].Name;
            }
        }

        string tempTitleKey = "Menu.Page.Name." + menuMan.CurrentPage.Name.ToString();
        string tempTitleValue = UFE.Localization.GetTextValue(tempTitleKey);
        TitleText.text = tempTitleValue;
    }

}
