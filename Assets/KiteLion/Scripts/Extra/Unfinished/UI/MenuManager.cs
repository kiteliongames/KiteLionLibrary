using UnityEngine;
using SmartLocalization;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// All menuing is done here. A completely GUI independent UI nav.
/// Dependenct on CInput only. The MenuAnimManager looks at this to determine the action it takes.
/// Now includes audio.
///  -- Eliot Carney-Seim
/// </summary>
public class MenuManager : MainMenuScreen
{

    public const int MAX_BUTTONS = 4;
    public const bool DEBUG_ON = true;

    #region public instance fields
    public AudioClip onLoadSound;
    public AudioClip music;
    public AudioClip selectSound;
    public AudioClip cancelSound;
    public AudioClip moveCursorSound;
    public bool stopPreviousSoundEffectsOnLoad = false;
    public float delayBeforePlayingMusic = 0.1f;
    #endregion

    public enum PageName{
        None,
        MainMenu,
        Offline,
        Options
    };

    /// <summary>
    /// A Page is a single menu of buttons.
    /// </summary>
    public class Page
    {
        private MenuManager parent;
        private PageName name;
        private pButton[] pButtons;
        private Page prevPage;
        private int buttonCount; //Number of buttons

        private int currentBtn; // 0 -> MAX_BUTTONS

        public Page(MenuManager parent, PageName name, pButton[] buttons)
        {
            if(buttons.Length > MAX_BUTTONS)
            {
                Debug.LogError("Max Buttons allowed per Menu Page is " + MAX_BUTTONS + "!");
                Application.Quit();
                return;
            }
            this.parent = parent;
            this.name = name;
            this.pButtons = buttons;
            buttonCount = buttons.Length;
            prevPage = null;
        }

        public class pButton
        {
            private Page nextPage;
            private string name;
            public delegate void OnPress();
            public OnPress toNewScene;

            public pButton(Page nextPage)
            {
                this.nextPage = nextPage;
                this.name = UFE.Localization.GetTextValue("Menu.Page.Name." + nextPage.Name.ToString());
                this.toNewScene = null;
            }

            public pButton(string name, OnPress newScene)
            {
                this.nextPage = null;
                this.name = name;
                this.toNewScene = newScene;
            }

            /// <summary>
            /// Used if we're changing a page, but for some reason some extra function
            /// needs to be done as well.
            /// </summary>
            /// <param name="nextPage">Page to go to.</param>
            /// <param name="newScene">Function loading new "scene", following OnPress signature.</param>
            public pButton(Page nextPage, OnPress newScene)
            {
                this.nextPage = nextPage;
                this.name = nextPage.Name.ToString();
                this.toNewScene = newScene;
            }

            public Page NextPage
            {
                get
                {
                    return nextPage;
                }
            }

            public string Name
            {
                get
                {
                    return name;
                }
            }
        }

        public pButton[] PButtons
        {
            get
            {
                return pButtons;
            }
        }

        public PageName Name
        {
            get
            {

                return name;
            }
        }

        /// <summary>
        /// Moves to target selection. Used for mouse support.
        /// Always does thing, so no need for boolean to verify.
        /// Tests for valid targetButton are done by _SelectTarget helper.
        /// 
        /// Called by: In-Editor EventSystem on FrontLayer Canvas Buttons
        /// </summary>
        /// <param name="targetButton"></param>
        public void SelectTarget(int targetButton)
        {
            currentBtn = targetButton;
        }

        /// <summary>
        /// Moves menu selection up (backend only, no animation call) if possible.
        /// </summary>
        /// <returns>Returns true if successfully moved up, false if not.</returns
        public bool SelectUp()
        {
            int tempSelect = currentBtn;
            currentBtn = currentBtn < pButtons.Length - 1 ? currentBtn+1 : currentBtn;
            if (DEBUG_ON)
                Debug.Log("New Select Up is: " + currentBtn);
            return (tempSelect != currentBtn);
        }

        /// <summary>
        /// Moves menu selection down if possible.
        /// </summary>
        /// <returns>Returns true if successfully moved down, false if not.</returns>
        public bool SelectDown()
        {
            int tempSelect = currentBtn;
            currentBtn = currentBtn > 0? currentBtn-1 : currentBtn;
            if (DEBUG_ON)
                Debug.Log("New Select Down is: " + currentBtn);
            return (tempSelect != currentBtn);
        }

        public Page OnSelect()
        {
            if (DEBUG_ON)
                Debug.Log("Button Selected: " + pButtons[currentBtn].Name);

            if (pButtons[currentBtn].toNewScene != null)
            {
                pButtons[currentBtn].toNewScene();
                parent.MenuDeployCtrl.UnDeploy();
            }
            if (pButtons[currentBtn].NextPage != null)
            {
                if (DEBUG_ON)
                    Debug.Log("New Page is: " + pButtons[currentBtn].NextPage.Name);
                return pButtons[currentBtn].NextPage;
            }
            return null;
        }

        public Page PrevPage
        {
            get
            {
                return prevPage;
            }

            set
            {
                prevPage = value;
            }
        }

        public int CurrentBtn
        {
            get
            {
                return currentBtn;
            }
        }

        public int ButtonCount
        {
            get
            {
                return buttonCount;
            }
        }
    }

    #region Page Class Initializing
    private Page MainMenu;
    private Page Online;
    private Page Offline;
    private Page Options;
    #endregion

    #region Menu Nav Live Storing Data
    private Page currentPage;
    private PageName currentPageName;
    private bool canMove; //True if the joystick is reset back to 0.
    private int selected;
    #endregion

    #region Button Input Detection Cieling / Floor
    public float UpWeight;
    public float DownWeight;
    #endregion

    #region External Class References
    public BamnMainMenuScreen MenuActions;
    public MenuDeploy MenuDeployCtrl;
    #endregion

    // Use this for initialization
    void Start()
    {
        #region Menu Construction
        Options = new Page(this, PageName.Options, new Page.pButton[]
        {
            new Page.pButton(UFE.Localization.GetTextValue("Menu.Options.Button.Controls"), null),
            new Page.pButton(UFE.Localization.GetTextValue("Menu.Options.Button.Audio"), null),
            new Page.pButton(UFE.Localization.GetTextValue("Menu.Options.Button.Video"), null)
        });

        Page.pButton.OnPress toTraining = new Page.pButton.OnPress(GoToTrainingModeScreen);
        Page.pButton.OnPress toVersus = new Page.pButton.OnPress(GoToVersusModeScreen);
        Page.pButton.OnPress toStory = new Page.pButton.OnPress(GoToStoryModeScreen);
        Offline = new Page(this, PageName.Offline, new Page.pButton[]
        {
            new Page.pButton(UFE.Localization.GetTextValue("Menu.Offline.Button.Story"), toStory),
            new Page.pButton(UFE.Localization.GetTextValue("Menu.Offline.Button.Versus"), toVersus),
            new Page.pButton(UFE.Localization.GetTextValue("Menu.Offline.Button.Training"), toTraining)
        });

        Page.pButton.OnPress QuitButton = new Page.pButton.OnPress(Quit);
        MainMenu = new Page(this, PageName.MainMenu, new Page.pButton[]
        {
            new Page.pButton("Single Player", new Page.pButton.OnPress(MenuActions.GoToPlayerVsCpuScreen)), //NULL MUST BE SET HERE // TO Player vs AI
            new Page.pButton("Multi Player", new Page.pButton.OnPress(MenuActions.GoToPlayerVsPlayerScreen)), // To Player vs Player
            new Page.pButton("Training", new Page.pButton.OnPress(MenuActions.GoToTrainingModeScreen)), // Options aren't done so just going to Training.
            new Page.pButton(UFE.Localization.GetTextValue("Menu.Main.Button.Exit"), Quit)
        });
        #endregion

        currentPage = MainMenu;
        currentPageName = MainMenu.Name;
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        #region UI Input
        if (canMove)
        {
            if (cInput.GetAxis("P1KeyboardVertical") > UpWeight)
            {
                //Goes "down" the list, visually downward on buttons.
                if (currentPage.SelectDown()) {
                    UFE.PlaySound(moveCursorSound);
                    if (DEBUG_ON)
                        Debug.Log("Moved Down a Button.");
                }

                canMove = false;
            }
            if (cInput.GetAxis("P1KeyboardVertical") < DownWeight)
            {
                if (currentPage.SelectUp()) {
                    UFE.PlaySound(moveCursorSound);
                    if (DEBUG_ON)
                        Debug.Log("Moved Up a Button.");
                }

                canMove = false;
            }
        }

        if (!canMove)
            if (cInput.GetAxis("P1KeyboardVertical") < UpWeight && cInput.GetAxis("P1KeyboardVertical") > DownWeight)
                canMove = true;

        if (cInput.GetButtonDown("P1Button1")) // CONFIRM OR SUBMIT BUTTON
        {
            if (DEBUG_ON)
                Debug.Log("Confirm Button pressed!");

            _NextPage();
        }
        if (cInput.GetButtonDown("P1Button2")) // BACK OR CANCEL BUTTON
        {
            if (DEBUG_ON)
                Debug.Log("Back Button pressed!");

            if (currentPage.PrevPage != null)
            {
                UFE.PlaySound(cancelSound);
                if (DEBUG_ON)
                    Debug.Log("Going back a Page to: " + currentPage.PrevPage.Name);
                currentPage = currentPage.PrevPage;
                currentPageName = currentPage.Name;
            }

        }
        #endregion
    }

    #region public helper methods
    /// <summary>
    /// Moves to target selection. Used for mouse support.
    /// Always does thing, so no need for boolean to verify.
    /// </summary>
    /// <param name="targetButton">A valid number between 0 and @MAX_BUTTONS - 1</param>
    public void _SelectTarget(int targetButton)
    {
        if (targetButton > MAX_BUTTONS - 1 || targetButton < 0) {
            CBUG.Error("TARGET BUTTON EXCEEDS MAXIMUM BUTTONS OF: " + MAX_BUTTONS);
            return;
        }
        currentPage.SelectTarget(targetButton);
        UFE.PlaySound(moveCursorSound);
    }

    public void _NextPage()
    {
        UFE.PlaySound(selectSound);
        Page nextPage = currentPage.OnSelect();
        if (nextPage != null) {
            nextPage.PrevPage = currentPage;
            currentPage = nextPage;
            currentPageName = currentPage.Name;
            if (DEBUG_ON)
                Debug.Log("Previous page has been set to: " + currentPage.PrevPage.Name);
        }
    }
    #endregion

    #region public override methods
    public override void DoFixedUpdate()
    {
        base.DoFixedUpdate();
        this.DefaultNavigationSystem(this.moveCursorSound, this.selectSound, this.cancelSound);
    }
    public override void OnShow()
    {
        base.OnShow();
        this.HighlightOption(this.FindFirstSelectable());
        
        if (this.music != null) {
            UFE.DelayLocalAction(delegate () { UFE.PlayMusic(this.music); }, this.delayBeforePlayingMusic);
        }

        if (this.stopPreviousSoundEffectsOnLoad) {
            UFE.StopSounds();
        }

        if (this.onLoadSound != null) {
            UFE.DelayLocalAction(delegate () { UFE.PlaySound(this.onLoadSound); }, this.delayBeforePlayingMusic);
        }
    }
    #endregion

    #region Getters
    public Page CurrentPage
    {
        get
        {
            return currentPage;
        }
    }

    public PageName CurrentPageName
    {
        get
        {
            return currentPageName;
        }
    }
    #endregion
}
