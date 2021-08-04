using System;
using ClassicUO.Configuration;
using ClassicUO.IO.Audio;

namespace ClassicUO.Game.UoMars
{
    public class UOMarsPacketHandler
    {
        public static void PlaySound(string file)
        {
            Profile currentProfile = ProfileManager.CurrentProfile;
            if(currentProfile == null)
            {
                return;
            }
                            
            float volume = currentProfile.SoundVolume / Constants.SOUND_DELTA;

            if (Client.Game.IsActive)
            {
                if (!currentProfile.ReproduceSoundsInBackground)
                {
                    volume = currentProfile.SoundVolume / Constants.SOUND_DELTA;
                }
            }
            else if (!currentProfile.ReproduceSoundsInBackground)
            {
                volume = 0;
            }

            if (volume < -1 || volume > 1f)
            {
                return;
            }
                            
            if (!currentProfile.EnableSound || !Client.Game.IsActive && !currentProfile.ReproduceSoundsInBackground)
            {
                volume = 0;
            }
                            
            try
            {
                // full path will be: \UoMarsClient/Music/Digital/UoMars/file.mp3
                //UOMusic music = new UOMusic(0, "UoMars/" + file, false);
                //music.Play(volume);
            }
            catch (Exception e)
            {}
        }
    }
}