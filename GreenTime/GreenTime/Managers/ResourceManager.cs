using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using GreenTimeGameData.Components;

namespace GreenTime.Managers
{
    public class ResourceManager
    {
        // content that is global to the whole game
        private ContentManager globalContent;
        // content that is loaded and unloaded for each screen
        private ContentManager localContent;

        // marks if menu resources are loaded
        private bool isGlobalContentLoaded;
        private bool isLocalContentLoaded;
        
        private bool isGameplayLoaded = false;
        private bool isMenuLoaded = false;

        #region Constants
        private readonly string GAMEPLAY_TEXTURE = "gameplayTexture";
        private readonly string MENU_TEXTURE = "menuTexture";
        #endregion

        private Dictionary<string, Rectangle> textureDictionary;

        public Texture2D GlobalTexture;
        public Texture2D LevelTexture;

        public SpriteFont MainFont;
        public SpriteFont ChatFont;
        
        public Effect DesaturationShader;
        public Effect SepiaShader;
        public Effect TimeTravelShader;

        #region Initialization
        public void InitializeContent(Game game)
        {
            globalContent = game.Content;
            localContent = new ContentManager(game.Services, "Content");
        }

        public void InitializeLevelManager()
        {
            LevelManager.Instance.Initialize(localContent);
        }
        #endregion

        #region Unloading
        /// <summary>
        /// Unloads the local content
        /// </summary>
        public void UnloadLocalContent()
        {
            if (isLocalContentLoaded)
            {
                localContent.Unload();
                SoundManager.UnloadLocal();
                isLocalContentLoaded = false;
            }
        }

        /// <summary>
        /// Unloads global content
        /// </summary>
        public void UnloadGlobalContent()
        {
            if (isGlobalContentLoaded)
            {
                globalContent.Unload();
                SoundManager.UnloadGlobal();
                isGlobalContentLoaded = false;
                isGameplayLoaded = false;
                isMenuLoaded = false;
            }
        }

        /// <summary>
        /// Unloads all content
        /// </summary>
        public void UnloadAllContent()
        {
            this.UnloadLocalContent();
            this.UnloadGlobalContent();
        }
        #endregion

        #region Loading
        public void LoadLevelTexture(string textureName)
        {
            LevelTexture = localContent.Load<Texture2D>(textureName);
            isLocalContentLoaded = true;
        }

        public void LoadGameplayTexture()
        {
            if (!isGameplayLoaded)
            {
                // if menu textures were lodaed, unload them first
                if (isMenuLoaded)
                    UnloadGlobalContent();

                // load global resources
                LoadGlobalResources(true);

                // load gameplay textures
                GlobalTexture = globalContent.Load<Texture2D>(GAMEPLAY_TEXTURE);
                isGameplayLoaded = true;
            }
        }

        /// <summary>
        /// Loads the menu texture and related resources for the menu system
        /// </summary>
        public void LoadMenuTexture()
        {
            if (!isMenuLoaded)
            {
                // if gameplay textures were loaded, unload them first
                if (isGameplayLoaded)
                    UnloadGlobalContent();

                // load global resources
                LoadGlobalResources(false);

                // load menu texture
                GlobalTexture = globalContent.Load<Texture2D>(MENU_TEXTURE);
                isMenuLoaded = true;
            }
        }

        public void LoadSound(string soundName, bool isAmbient = false)
        {
            if (isAmbient)
                SoundManager.LoadAmbientSound(localContent, soundName);
            else
                SoundManager.LoadSound(localContent, soundName);
            isLocalContentLoaded = true;
        }

        public void LoadSong(string songName)
        {
            SoundManager.LoadSong(globalContent, songName);
        }

        private void LoadGlobalResources( bool forGameplay)
        {
            // load texture list
            textureDictionary = globalContent.Load<Dictionary<string, Rectangle>>("textures");
            // load font
            MainFont = globalContent.Load<SpriteFont>("fonts\\menuFont");
            ChatFont = globalContent.Load<SpriteFont>("fonts\\chatfont");
            // load shaders
            DesaturationShader = globalContent.Load<Effect>("effects\\desaturate");
            SepiaShader = globalContent.Load<Effect>("effects\\sepia");
            TimeTravelShader = globalContent.Load<Effect>("effects\\timeTravel_shader");

            // load sounds
            if ( forGameplay )
                SoundManager.LoadGameplaySounds(globalContent);
            else
                SoundManager.LoadMenuSounds(globalContent);

            isGlobalContentLoaded = true;
        }
        #endregion

        /// <summary>
        /// Gets the area of the texture in the texture file
        /// </summary>
        /// <param name="textureName">The name of the texture</param>
        /// <returns></returns>
        public Rectangle this[string textureName]
        {
            get { return textureDictionary[textureName]; }
        }

        #region Singleton
        private static readonly ResourceManager instance = new ResourceManager();

        private ResourceManager()
        {
        }

        public static ResourceManager Instance { get { return instance; } }
        #endregion
    }
}
