using UnityEngine;
using UnityEngine.EventSystems;

public class UIClickSound : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string soundName = "uiClick";

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundEffectManager.Play(soundName);
    }
}
