using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class SaveData
{
   public Vector3 playerPosition;
   public string mapBoundary;//The boundary name for map
   public List<InventorySaveData> inventoryData; //List to hold inventory data for each item
}
