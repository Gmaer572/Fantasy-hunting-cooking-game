using UnityEngine;
using System.IO;

public class SaveController : MonoBehaviour
{
   private string saveLocation;
   void Start()
   {
    //define save location, switch back to persistentDataPath before build.
      saveLocation = Path.Combine(Application.dataPath, "..", "Saves", "saveData.json");
      Directory.CreateDirectory(Path.GetDirectoryName(saveLocation));
      LoadGame();
   }
   public void SaveGame()
   {
      SaveData saveData = new SaveData
      {
       playerPosition=GameObject.FindGameObjectWithTag("Player").transform.position,
       //mapBoundary=FindObjectOfType<CinemachineConfiner>().m_BoundingShape2D.gameObject.name  
      };
      File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
   }
    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            SaveData saveData=JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playerPosition;
  
        }
        else
        {
            SaveGame();
        }
    }
   }

