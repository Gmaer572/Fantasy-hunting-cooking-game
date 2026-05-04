using UnityEngine;

// Place this component in each room scene and set the track name to
// whatever key you registered in MusicLibrary on the MusicManager GameObject.
public class RoomMusic : MonoBehaviour
{
    [SerializeField] string trackName;

    void Start()
    {
        MusicManager.Play(trackName);
    }
}
