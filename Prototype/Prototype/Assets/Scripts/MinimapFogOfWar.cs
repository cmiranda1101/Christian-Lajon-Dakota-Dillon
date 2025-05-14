using UnityEngine;

public class MinimapFogOfWar : MonoBehaviour
{
    Renderer fogOfWarRenderer; // The renderer of the fog of war object
    Texture2D fogTexture; // The texture used for the fog of war
    Vector3 playerPosition; // The position of the player
    Texture2D copy; // A copy of the fog texture
    [SerializeField] int radius; // The radius of the area to uncover around the player
    void Start()
    {
        fogOfWarRenderer = GetComponent<Renderer>();
        fogTexture = fogOfWarRenderer.material.mainTexture as Texture2D;
        copy = Instantiate(fogTexture);
        fogOfWarRenderer.material.mainTexture = copy;
    }

    
    void Update()
    {
        playerPosition = GameManager.instance.player.transform.position;
        calculateAndUncoverPixelPosition();
        
    }

    public void calculateAndUncoverPixelPosition()
    {
        // Get the player's position on the FogOfWar
        float horizontal = Mathf.InverseLerp(fogOfWarRenderer.bounds.min.x, fogOfWarRenderer.bounds.max.x, playerPosition.x);
        float vertical = Mathf.InverseLerp(fogOfWarRenderer.bounds.min.z, fogOfWarRenderer.bounds.max.z, playerPosition.z);
        // Convert to pixel coordinates
        int pixelX = Mathf.FloorToInt(horizontal * copy.width);
        int pixelY = Mathf.FloorToInt(vertical * copy.height);

        // Uncover the area around the player
        for (int x = -radius; x < radius; x++)
        {
            for (int y = -radius; y < radius; y++)
            {
                copy.SetPixel(pixelX + x, pixelY + y, new Color(0,0,0,0));
            }
        }
        copy.Apply();
    }
}
