using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class Item : MonoBehaviour
{
   public int ID;
   public string Name;

   //function for pickup
   public virtual void Pickup()
   {
      Debug.Log("Picked up: " + Name);
      
      Sprite itemIcon=GetComponent<Image>().sprite;
      if (ItemPickupUIController.Instance != null)
      {
         ItemPickupUIController.Instance.ShowItemPickup(Name, itemIcon);
      }
   }
}
