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
        // constants determining the sound index in the gameplay array
        public static readonly int SOUND_MENU_UP = 0;
        public static readonly int SOUND_MENU_DOWN = 1;
        public static readonly int SOUND_MENU_CONFIRM = 2;
        public static readonly int SOUND_MENU_CANCEL = 3;
        public static readonly int SOUND_TIMETRAVEL = 4;
        public static readonly int SOUND_DROP = 5;
        public static readonly int SOUND_PICK = 6;
        public static readonly int SOUND_FOOTSTEPS = 7;
        public static readonly int SOUND_POWERUP = 8;
        public static readonly int SOUND_POWERDOWN = 9;
        public static readonly int SOUND_SOLVED = 10;

        // content file names
        private static readonly string MENU_UP_FILENAME = @"audio\scrollUp";
        private static readonly string MENU_DOWN_FILENAME = @"audio\scrollDown";
        private static readonly string MENU_CONFIRM_FILENAME = @"audio\ConfirmationSound";
        private static readonly string MENU_CANCEL_FILENAME = @"audio\CancelSound";
        private static readonly string TRAVEL_SOUND_FILENAME = @"audio\timeTravel";
        private static readonly string DROP_FILENAME = @"audio\throwingGarbage";
        private static readonly string PICK_FILENAME = @"audio\pickup";
        private static readonly string STEPS_FILENAME = @"audio\footsteps";
        private static readonly string POWERUP_FILENAME = @"audio\PowerUp";
        private static readonly string POWERDOWN_FILENAME = @"audio\PowerDown";
        private static readonly string SOLVED_FILENAME = @"audio\PuzzleSuccess";
        private static readonly string SONG_GAME = @"audio\greentime";
        public static readonly string SONG_INTRO = @"audio\IntroSong";
        public static readonly string SONG_MENU = @"audio\MenuSong";

        // the in game music
        private static Song music;
        private static Song alternateMusic;

        // game sound effects
        private static int menuSoundCount = 4;
        private static int gameplaySoundCount = 11;

        private static SoundEffect[] globalSounds;
        private static Dictionary<string, SoundEffect> levelSounds = new Dictionary<string,SoundEffect>();
        private static SoundEffectInstance ambientSound;
        
        // flag marking if game music is playing
        private static bool gameMusicPlaying = false;

        // ambients sound position to change volume levels
        private static int ambientSoundPosition;

        // footsteps
        private static SoundEffectInstance footsteps;

        private static string songLoaded = null;

        /// <summary>
        /// Loads all music files
        /// </summary>
        /// <param name="content"></param>
        public static void LoadMenuSounds(ContentManager content)
        {

            SoundEffect.MasterVolume = 1.0f;

            // music
            music = content.Load<Song>(SONG_MENU);
            alternateMusic = content.Load<Song>(SONG_INTRO);

            // sound effects
            globalSounds = new SoundEffect[menuSoundCount];
            globalSounds[SOUND_MENU_UP] = content.Load<SoundEffect>(MENU_UP_FILENAME);
            globalSounds[SOUND_MENU_DOWN] = content.Load<SoundEffect>(MENU_DOWN_FILENAME);
            globalSounds[SOUND_MENU_CONFIRM] = content.Load<SoundEffect>(MENU_CONFIRM_FILENAME);
            globalSounds[SOUND_MENU_CANCEL] = content.Load<SoundEffect>(MENU_CANCEL_FILENAME);
        }

        public static void LoadGameplaySounds(ContentManager content)
        {

            SoundEffect.MasterVolume = 1.0f;

            // music
            music = content.Load<Song>(SONG_GAME);

            // sound effects
            globalSounds = new SoundEffect[gameplaySoundCount];
            globalSounds[SOUND_MENU_UP] = content.Load<SoundEffect>(MENU_UP_FILENAME);
            globalSounds[SOUND_MENU_DOWN] = content.Load<SoundEffect>(MENU_DOWN_FILENAME);
            globalSounds[SOUND_MENU_CONFIRM] = content.Load<SoundEffect>(MENU_CONFIRM_FILENAME);
            globalSounds[SOUND_MENU_CANCEL] = content.Load<SoundEffect>(MENU_CANCEL_FILENAME);
            globalSounds[SOUND_TIMETRAVEL] = content.Load<SoundEffect>(TRAVEL_SOUND_FILENAME);
            globalSounds[SOUND_DROP] = content.Load<SoundEffect>(DROP_FILENAME);
            globalSounds[SOUND_PICK] = content.Load<SoundEffect>(PICK_FILENAME);
            globalSounds[SOUND_POWERUP] = content.Load<SoundEffect>(POWERUP_FILENAME);
            globalSounds[SOUND_POWERDOWN] = content.Load<SoundEffect>(POWERDOWN_FILENAME);
            globalSounds[SOUND_SOLVED] = content.Load<SoundEffect>(SOLVED_FILENAME);

            // foot steps
            globalSounds[SOUND_FOOTSTEPS] = content.Load<SoundEffect>(STEPS_FILENAME);
            footsteps = globalSounds[SOUND_FOOTSTEPS].CreateInstance();
            footsteps.Volume = 0.7f;
            footsteps.IsLooped = true;
        }

        public static void LoadSong(ContentManager content, string songName)
        {
            if (songLoaded != songName)
            {
                songLoaded = songName;
                music = content.Load<Song>(songName);
            }
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

        public static void UnloadLocal()
        {
            // clear dictionary and ambient sound
            //gameMusicPlaying = false;
            levelSounds.Clear();
            if ( ambientSound != null && !ambientSound.IsDisposed )
                ambientSound.Dispose();
        }

        public static void UnloadGlobal()
        {
            gameMusicPlaying = false;
            songLoaded = null;
            globalSounds = null;
        }

        public static void PlayFootsteps()
        {
            if ( footsteps.State != SoundState.Playing )
                footsteps.Play();
        }

        public static void StopFootsteps()
        {
            footsteps.Stop(true);
        }

        /// <summary>
        /// Plays in game music
        /// </summary>
        public static void PlayMusic(bool alternate = false, bool loop = true, float startVolume = 0.0f)
        {
            // play game music if it is not already playing
            if (SettingsManager.MusicEnabled && ( !gameMusicPlaying || alternate))
            {
                MediaPlayer.Stop();
                MediaPlayer.Volume = startVolume;  // mute sound until it fades in
                MediaPlayer.IsRepeating = loop;
                if ( alternate )
                    MediaPlayer.Play(alternateMusic);
                else
                    MediaPlayer.Play(music);
                gameMusicPlaying = true;
            }
        }

        // for testing only
        //public static double TestPosition()
        //{
        //    return MediaPlayer.PlayPosition.TotalSeconds;
        //}

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
            MediaPlayer.Volume = MathHelper.Clamp( 0.5f - ( transition / 2.0f ), 0.3f, 0.5f ); // volume between 0.3 to 1.0
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
            if (ambientSound != null && !ambientSound.IsDisposed)
            {
                ambientSound.Volume = 0.0f;
                ambientSound.Play();
            }
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
