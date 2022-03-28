using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlaceBlockScript : MonoBehaviour
{
    
    public int count;
    public string type;
    
    public void OnEnable()
    {
        StartCoroutine(check());
    }
    
    public IEnumerator check()
    {
        yield return new WaitForSeconds(0.5f);
        close();
        transform.Find("Closed").GetComponentInChildren<Text>().text = "Поставьте " + count + CONSTANTS.getItemByType(type).name;
        while (CONSTANTS.getTCount(type) < count)
        {
            yield return new WaitForSeconds(0.2f);
            print(CONSTANTS.getTCount(type));
        }
        open();
        Destroy(this);
    }
    
    public void open()
    {
        transform.Find("Opened").gameObject.SetActive(true);
        transform.Find("Closed").gameObject.SetActive(false);
        Destroy(transform.Find("Closed"));
        Destroy(this);
    }
    
    public void close()
    {
        transform.Find("Opened").gameObject.SetActive(false);
        transform.Find("Closed").gameObject.SetActive(true);
    }
}