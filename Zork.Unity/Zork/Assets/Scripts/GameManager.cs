using UnityEngine;
using Zork;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        TextAsset gameJsonAsset = Resources.Load<TextAsset>(ZorkGameFileAssetName);
        Game.Load(gameJsonAsset.text, OutputService, InputService);
    }
    void Start()
    {
        
    }

    void Update()
    {

        
    }

    [SerializeField]
    private string ZorkGameFileAssetName = "Zork";
    [SerializeField]
    private UnityOutputService OutputService;
    [SerializeField]
    private UnityInputService InputService;
}
