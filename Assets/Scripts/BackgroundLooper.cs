using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    public Transform cameraTransform;
    public Transform[] backgrounds;
    public float spriteWidth;

    private void Update()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            Transform bg = backgrounds[i];

            if (cameraTransform.position.x - bg.position.x >= spriteWidth)
            {
                float rightMostX = GetRightMostBackgroundX();
                bg.position = new Vector3(rightMostX + spriteWidth, bg.position.y, bg.position.z);
            }
        }
    }

    float GetRightMostBackgroundX()
    {
        float maxX = backgrounds[0].position.x;

        for (int i = 1; i < backgrounds.Length; i++)
        {
            if (backgrounds[i].position.x > maxX)
            {
                maxX = backgrounds[i].position.x;
            }
        }

        return maxX;
    }
}