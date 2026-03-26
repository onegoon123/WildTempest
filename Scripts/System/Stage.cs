using UnityEngine;
using UnityEngine.Tilemaps;

public class Stage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var cdata = Wild.Enemy.Stage.StageMap[GameManager.Instance.currentStage];
        Color color = new Color(cdata.r, cdata.g, cdata.b);
        var tiles = GetComponentsInChildren<Tilemap>();
        foreach (var tile in tiles)
        {
            tile.color = color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
