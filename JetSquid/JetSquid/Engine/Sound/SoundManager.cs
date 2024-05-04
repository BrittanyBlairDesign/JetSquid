


using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public class SoundManager
{
    private int _soundTrackIndex = -1;
    private List<SoundEffectInstance> _soundTracks = new List<SoundEffectInstance>();
    private Dictionary<Type, SoundEffect> _soundBank = new Dictionary<Type, SoundEffect>();
    public void SetSoundTrack(List<SoundEffectInstance> tracks)
    {
        _soundTracks = tracks;
        if(_soundTracks.Count == 1)
        {
            _soundTracks[0].IsLooped = true; 
        }
        _soundTrackIndex = _soundTracks.Count - 1;
    }

    public void PlaySoundTrack()
    {
        var nbTracks = _soundTracks.Count;
        var currentTrack = _soundTracks[_soundTrackIndex];
        if (nbTracks <= 0)
        {
            return;
        }

        
        var nextTrack = _soundTracks[(_soundTrackIndex + 1) % nbTracks];

        if(currentTrack.State == SoundState.Stopped)
        {
            if (nextTrack != null)
            {
                nextTrack.Play();

                if(_soundTracks.Count == 1)
                {
                    nextTrack.IsLooped = true;
                }
                else
                {
                    _soundTrackIndex++;
                }

            }
            
            if(_soundTrackIndex >= _soundTracks.Count)
            {
                _soundTrackIndex = 0;

            }
        }
    }

    public void OnNotify(BaseGameStateEvent gameEvent)
    {
        if (_soundBank.ContainsKey(gameEvent.GetType()))
        {
            var sound = _soundBank[gameEvent.GetType()];

            sound.Play();
        }
    }

    public void RegisterSound(BaseGameStateEvent gameStateEvent, SoundEffect sound)
    {
        _soundBank.Add(gameStateEvent.GetType(), sound);
    }
}

