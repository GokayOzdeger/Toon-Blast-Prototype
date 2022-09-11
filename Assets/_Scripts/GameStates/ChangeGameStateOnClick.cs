using UnityEngine;
using UnityEngine.UI;

public class ChangeGameStateOnClick : MonoBehaviour
{
    [SerializeField] private GameState gameState;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(ChangeGameState);
    }

    private void ChangeGameState()
    {
        GameManager.Instance.ChangeGameState(gameState);
    }
}