using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GreenTime.Managers
{
    public static class SoundManager
    {
        // constants determining the sound index in the array
        public static readonly int SOUND_TIMETRAVEL = 0;
        public static readonly int SOUND_MENU_UP = 1;
        public static readonly int SOUND_MENU_DOWN = 2;
        public static readonly int SOUND_DROP = 3;

        // content file names
        private static readonly string GAME_MUSIC_FILENAME = "greentime";
        private static readonly string TRAVEL_SOUND_FILENAME = "timeTravel";
        private static readonly string MENU_UP_FILENAME = @"audio\scrollUp";
        private static readonly string MENU_DOWN_FILENAME = @"audio\scrollDown";
        private static readonly string DROP_FILENAME = @"audio\throwingGarbage";

        // the in game music
        private static Song gameMusic;

        // game sound effects
        private static int soundCount = 4;
        private static SoundEffect[] globalSounds;
        private static Dictionary<string, SoundEffect> levelSounds = new Dictionary<string,SoundEffect>();
        private static SoundEffectInstance ambientSound;
        
        // flag marking if game music is playing
        private static bool gameMusicPlaying = false;

        // ambients sound position to change volume levels
        private static int ambientSoundPosition;

        /// <summary>
        /// Loads all music files
        /// </summary>
        /// <param name="content"></param>
        public static void LoadAllSounds(ContentManager content)
        {
            SoundEffect.MasterVolume = 1.0f;

            // music
            gameMusic = content.Load<Song>(GAME_MUSIC_FILENAME);

            // sound effects
            globalSounds = new SoundEffect[soundCount];
            globalSounds[SOUND_TIMETRAVEL] = content.Load<SoundEffect>(TRAVEL_SOUND_FILENAME);
            globalSounds[SOUND_MENU_UP] = content.Load<SoundEffect>(MENU_UP_FILENAME);
            globalSounds[SOUND_MENU_DOWN] = content.Load<SoundEffect>(MENU_DOWN_FILENAME);
            globalSounds[SOUND_DROP] = content.Load<SoundEffect>(DROP_FILENAME);
        }

        /// <summary>
        /// Loads a sound specific in a level (needs to be unloaded)
        /// </summary>
        /// <param name="content"></param>
        /// <param name="soundName"></param>
        public static void LoadSound(ContentManager content, string soundName)
        {
            // cleanup sound if it was disposed
            if (levelSounds.ContainsKey(soundName) && levelSounds[soundName].IsDisposed)
                levelSounds.Remove(soundName);

            // add sound if not already there
            if ( !levelSounds.ContainsKey(soundName) )
                levelSounds.Add(soundName, content.Load<SoundEffect>(@"audio\" + soundName));
        }

        /// <summary>
        /// Loads an ambient soun specific in a level
        /// </summary>
        /// <param name="content"></param>
        /// <param name="soundName"></param>
        public static void LoadAmbientSound(ContentManager content, string soundName)
        {
            LoadSound(content, soundName);
            ambientSoundPosition = LevelManager.Instance.CurrentLevel.ambientSound.position;

            ambientSound = levelSounds[soundName].CreateInstance();
            ambientSound.IsLooped = LevelManager.Instance.CurrentLevel.ambientSound.looping;
        }

        public static void Unload()
        {
            // clear dictionary and ambient sound
            levelSounds.Clear();
            if ( ambientSound != null && !ambientSound.IsDisposed )
                ambientSound.Dispose();
        }

        /// <summary>
        /// Plays in game music
        /// </summary>
        public static void PlayGameMusic()
        {
            // play game music if it is not already playing
            if (SettingsManager.MusicEnabled && !gameMusicPlaying)
            {
                MediaPlayer.Stop();
                MediaPlayer.Volume = 0.0f;  // mute sound until it fades in
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(gameMusic);
                gameMusicPlaying = true;
            }
        }


        public static void Update( float playerX )
        {
            // update ambient sound panning and volume
            if (ambientSound != null && !ambientSound.IsDisposed)
            {
                ambientSound.Pan = MathHelper.Clamp((ambientSoundPosition - playerX) / 500.0f, -1.0f, 1.0f);
                ambientSound.Volume = MathHelper.Clamp(1.0f - Math.Abs((ambientSoundPosition - playerX) / 800.0f), 0.1f, 1.0f);
            }
        }

        /// <summary>
        /// Updates fade in and fade out of music
        /// </summary>
        /// <param name="transition"></param>
        public static void UpdateFade(float transition)
        {
            MediaPlayer.Volume = MathHelper.Clamp( 1.0f - transition, 0.3f, 1.0f ); // volume between 0.3 to 1.0
        }

        /// <summary>
        /// Plays a given sound effect
        /// </summary>
        /// <param name="soundIndex"></param>
        public static void PlaySound(int soundIndex)
        {
            if ( SettingsManager.SoundEnabled )
                globalSounds[soundIndex].Play();
        }

        /// <summary>
        /// Plays a level specific sound effect
        /// </summary>
        /// <param name="soundName"></param>
        public static void PlaySound(string soundName, bool looping)
        {
            if (SettingsManager.SoundEnabled)
            {
                if (looping)
                {
                    SoundEffectInstance i = levelSounds[soundName].CreateInstance();
                    i.IsLooped = true;
                    i.Play();
                }
                else
                {
                    levelSounds[soundName].Play();
                }
            }
        }

        public static void PlayAmbientSound()
        {
            if ( ambientSound != null && !ambientSound.IsDisposed )
                ambientSound.Play();
        }

        public static void ToggleMusic()
        {
            SettingsManager.MusicEnabled = !SettingsManager.MusicEnabled;

            // pause or resume music according to setting
            if (!SettingsManager.MusicEnabled)
                MediaPlayer.Pause();
            else
                MediaPlayer.Resume();
        }

        public static void ToggleSound()
        {
            SettingsManager.SoundEnabled = !SettingsManager.SoundEnabled;
        }
    }
}
