//Copyright 2022, Infima Games. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InfimaGames.LowPolyShooterPack.Interface
{
    /// <summary>
    /// Quality Settings Menu.
    /// </summary>
    public class MenuQualitySettings : Element
    {
        #region FIELDS SERIALIZED

        public static MenuQualitySettings Instance;

        [Title(label: "Settings")]

        [Tooltip("Canvas to play animations on.")]
        [SerializeField]
        private GameObject animatedCanvas;

        [Tooltip("Animation played when showing this menu.")]
        [SerializeField]
        private AnimationClip animationShow;

        [Tooltip("Animation played when hiding this menu.")]
        [SerializeField]
        private AnimationClip animationHide;

        #endregion
        
        #region FIELDS
        
        /// <summary>
        /// Animation Component.
        /// </summary>
        private Animation animationComponent;
        /// <summary>
        /// If true, it means that this menu is enabled and showing properly.
        /// </summary>
        private bool menuIsEnabled;


        #endregion

        #region UNITY

        protected override void Awake()
        {
            base.Awake();
            Instance = this;
        }

        private void Start()
        {
            //Hide pause menu on start.
            animatedCanvas.GetComponent<CanvasGroup>().alpha = 0;
            //Get canvas animation component.
            animationComponent = animatedCanvas.GetComponent<Animation>();
        }

        protected override void Tick()
        {
            //Switch. Fades in or out the menu based on the cursor's state.
            bool cursorLocked = characterBehaviour.IsCursorLocked();
            switch (cursorLocked)
            {
                //Hide.
                case true when menuIsEnabled:
                    Hide();
                    break;
                //Show.
                case false when !menuIsEnabled:
                    Show();
                    break;
            }
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Shows the menu by playing an animation.
        /// </summary>
        private void Show()
        {
            //Enabled.
            menuIsEnabled = true;

            //Play Clip.
            animationComponent.clip = animationShow;
            animationComponent.Play();
            //PauseGame();
            //Enable depth of field effect.
        }
        /*
        public void PauseGame()
        {
            StartCoroutine(PauseGameCoroutine());
        }

        IEnumerator PauseGameCoroutine()
        {
            yield return new WaitForSeconds(0.5f);
            Time.timeScale = 0;
        }*/

        /// <summary>
        /// Hides the menu by playing an animation.
        /// </summary>
        private void Hide()
        {
            //Disabled.
            menuIsEnabled = false;

            //Play Clip.
            animationComponent.clip = animationHide;
            animationComponent.Play();
            Time.timeScale = 1;

            //Disable depth of field effect.
        }

        /// <summary>
        /// Sets whether the post processing is enabled, or disabled.
        /// </summary>
        private void SetPostProcessingState(bool value = true)
        {
        }

        /// <summary>
        /// Sets the graphic quality to very low.
        /// </summary>
        public void SetQualityVeryLow()
        {
            //Set Quality.
            QualitySettings.SetQualityLevel(0);
            //Disable Post Processing.
            SetPostProcessingState(false);
        }
        /// <summary>
        /// Sets the graphic quality to low.
        /// </summary>
        public void SetQualityLow()
        {
            //Set Quality.
            QualitySettings.SetQualityLevel(1);
            //Disable Post Processing.
            SetPostProcessingState(false);
        }

        /// <summary>
        /// Sets the graphic quality to medium.
        /// </summary>
        public void SetQualityMedium()
        {
            //Set Quality.
            QualitySettings.SetQualityLevel(2);
            //Enable Post Processing.
            SetPostProcessingState();
        }
        /// <summary>
        /// Sets the graphic quality to high.
        /// </summary>
        public void SetQualityHigh()
        {
            //Set Quality.
            QualitySettings.SetQualityLevel(3);
            //Enable Post Processing.
            SetPostProcessingState();
        }

        /// <summary>
        /// Sets the graphic quality to very high.
        /// </summary>
        public void SetQualityVeryHigh()
        {
            //Set Quality.
            QualitySettings.SetQualityLevel(4);
            //Enable Post Processing.
            SetPostProcessingState();
        }
        /// <summary>
        /// Sets the graphic quality to ultra.
        /// </summary>
        public void SetQualityUltra()
        {
            //Set Quality.
            QualitySettings.SetQualityLevel(5);
            //Enable Post Processing.
            SetPostProcessingState();
        }

        public void Restart()
        {
            //Path.
            string sceneToLoad = SceneManager.GetActiveScene().path;
            
            #if UNITY_EDITOR
            //Load the scene.
            UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(sceneToLoad, new LoadSceneParameters(LoadSceneMode.Single));
            #else
            //Load the scene.
            SceneManager.LoadSceneAsync(sceneToLoad, new LoadSceneParameters(LoadSceneMode.Single));
            #endif
        }
        public void Quit()
        {
            //Quit.
            Application.Quit();
        }

        #endregion
    }
}