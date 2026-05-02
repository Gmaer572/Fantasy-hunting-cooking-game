using UnityEngine;
using System.Collections.Generic;

public class MusicLibrary : MonoBehaviour
{
    [System.Serializable]
    public struct MusicTrack
    {
        public string name;
        public AudioClip clip;
    }

    [SerializeField] List<MusicTrack> tracks;
    Dictionary<string, AudioClip> trackMap;

    void Awake()
    {
        trackMap = new Dictionary<string, AudioClip>();
        foreach (MusicTrack track in tracks)
            trackMap[track.name] = track.clip;
    }

    public AudioClip GetClip(string trackName)
    {
        trackMap.TryGetValue(trackName, out AudioClip clip);
        return clip;
    }
}
