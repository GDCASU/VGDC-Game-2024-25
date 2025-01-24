using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Level Music Data", menuName = "Audio/Level Music Data")]
public class LevelMusicData : ScriptableObject
{
    [Serializable]
    public class MusicTrack
    {
        public string trackName;
    }

    public List<MusicTrack> musicTracks = new List<MusicTrack>();
}
