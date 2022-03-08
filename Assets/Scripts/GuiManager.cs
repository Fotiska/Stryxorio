using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour
{
    private GameManage gameManage;
    [SerializeField] private Text fpsText;
    [SerializeField] private Text enemyCountText;
    [SerializeField] private Text baseHealthText;
    [SerializeField] private Text blockHealthText;

    private void Awake()
    {
        gameManage = GameManage.instance; //Get a Script GameManage
        StartCoroutine(DebugMenu()); //Start Coroutine
    }

    private IEnumerator DebugMenu()
    {
        while (true)
        {
            yield return new WaitForSeconds(1); //Wait 1 Sec
            
            fpsText.text = "FPS: " + Mathf.RoundToInt(1 / Time.deltaTime); //Change FPS Text
            enemyCountText.text = "EnemyCount: " + gameManage.enemyCount; //Change Enemy Count Text
            baseHealthText.text = "Base Health: " + gameManage.Base.health; //Change Base Health Text
            blockHealthText.text = "Block Health: ???"; //Change Block Health Text
            
            if (gameManage.block.Block == null) continue;
            Stats blockStats = gameManage.block.Block.GetComponent<Stats>();
            blockHealthText.text = "Block Health: " + blockStats.health; //Change Block Health Text
        }
    }
    
}
