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

        // content file names
        private static readonly string GAME_MUSIC_FILENAME = "greentime";
        private static readonly string TRAVEL_SOUND_FILENAME = "timeTravel";

        // the in game music
        private static Song gameMusic;

        // game sound effects
        private static int soundCount = 1;
        private static SoundEffect[] sounds;

        // flag marking if game music is playing
        private static bool gameMusicPlaying = false;

        /// <summary>
        /// Loads all music files
        /// </summary>
        /// <param name="content"></param>
        public static void LoadAllSounds(ContentManager content)
        {
            // music
            gameMusic = content.Load<Song>(GAME_MUSIC_FILENAME);

            // sound effects
            sounds = new SoundEffect[soundCount];
            sounds[SOUND_TIMETRAVEL] = content.Load<SoundEffect>(TRAVEL_SOUND_FILENAME);
        }

        /// <summary>
        /// Plays in game music
        /// </summary>
        public static void PlayGameMusic()
        {
            // play game music if it is not already playing
            if (!gameMusicPlaying)
            {
                MediaPlayer.Stop();
                MediaPlayer.Volume = 0.0f;  // mute sound until it fades in
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(gameMusic);
                gameMusicPlaying = true;
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
            sounds[soundIndex].Play();
        }
    }
}
