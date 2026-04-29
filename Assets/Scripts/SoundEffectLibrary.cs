using UnityEngine;
using System.Collections.Generic;

public class SoundEffectLibrary : MonoBehaviour
{
    [SerializeField] List<SoundEffectGroup> soundEffectGroups;
    private Dictionary<string, List<AudioClip>> soundEffects;

    [System.Serializable]
    public struct SoundEffectGroup
    {
        public string name;
        public List<AudioClip> clips;
    }

    public AudioClip GetRandomClip(string groupName)
    {
        if (soundEffects.TryGetValue(groupName, out List<AudioClip> clips) && clips.Count > 0)
        {
            int randomIndex = Random.Range(0, clips.Count);
            return clips[randomIndex];
        } 
        return null;
    } 
    private void Awake()
    {
        soundEffects = new Dictionary<string, List<AudioClip>>();
        foreach (SoundEffectGroup group in soundEffectGroups)
        {
            soundEffects[group.name] = group.clips;
        }
    }
}


