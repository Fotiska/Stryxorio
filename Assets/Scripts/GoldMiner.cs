using UnityEngine;

public class GoldMiner : MonoBehaviour
{

    private GameManage _gameManage;
    [SerializeField] private int earnGold;

    private void Awake()
    {
        _gameManage = FindObjectOfType<GameManage>();
    }

    public void MineGold()
    {    
        _gameManage.gold += earnGold;
        _gameManage.gold = Mathf.Clamp(_gameManage.gold, 0, _gameManage.maxGold);
    }
}
