using UnityEngine;
using System.IO;

public class SaveController : MonoBehaviour
{
    private InventoryController inventoryController;
   private string saveLocation;
   void Start()
   {
    //define save location, switch back to persistentDataPath before build.
      saveLocation = Path.Combine(Application.dataPath, "..", "Saves", "saveData.json");
      inventoryController=FindObjectOfType<InventoryController>();
      Directory.CreateDirectory(Path.GetDirectoryName(saveLocation));
      LoadGame();
   }
   public void SaveGame()
   {
      GameObject player = GameObject.FindGameObjectWithTag("Player");
      if (player == null)
      {
          Debug.LogWarning("SaveGame: No GameObject tagged 'Player' found. Save aborted.");
          return;
      }
      SaveData saveData = new SaveData
      {
          playerPosition = player.transform.position,
          //mapBoundary=FindObjectOfType<CinemachineConfiner>().m_BoundingShape2D.gameObject.name
          inventoryData = inventoryController.GetInventoryItems()
      };
      File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
   }
    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            SaveData saveData=JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playerPosition;
            inventoryController.LoadInventoryItems(saveData.inventoryData);
        }
        else
        {
            SaveGame();
        }
    }
   }

