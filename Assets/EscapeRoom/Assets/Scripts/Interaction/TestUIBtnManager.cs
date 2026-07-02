using UnityEngine;
using UnityEngine.UI;

public class TestUIBtnManager : MonoBehaviour
{

    [Header("Power Buttons")]
    public GameObject jellyBtn;
    public GameObject flyBtn;
    public GameObject scaleBtn;

    [Header("Scene Power Manager object")]
    public GameObject scenePowerManager;

    void Start()
    {
        if (scenePowerManager == null)
        {
            Debug.Log("Scene power manager object not found");
        }
        else {
            int player = scenePowerManager.GetComponent<ScenePowerManager>().playerNumber;
            if (player == 1)
            {
                jellyBtn.GetComponent<Image>().color = Color.coral;
            } else if (player == 2)
            {
                flyBtn.GetComponent<Image>().color = Color.coral;
            }
            else {
                scaleBtn.GetComponent<Image>().color = Color.coral;
            }
            // Add onclick events
            jellyBtn.GetComponent<Button>().onClick.AddListener( () => {
                jellyBtn.GetComponent<Image>().color = Color.coral;
                flyBtn.GetComponent <Image>().color = Color.white;
                scaleBtn.GetComponent<Image>().color = Color.white;
                scenePowerManager.GetComponent<ScenePowerManager>().TogglePowerTest(1);
            });
            flyBtn.GetComponent<Button>().onClick.AddListener(() => {
                flyBtn.GetComponent<Image>().color = Color.coral;
                jellyBtn.GetComponent<Image>().color = Color.white;
                scaleBtn.GetComponent<Image>().color = Color.white;
                scenePowerManager.GetComponent<ScenePowerManager>().TogglePowerTest(2);
            });
            scaleBtn.GetComponent<Button>().onClick.AddListener(() => {
                scaleBtn.GetComponent<Image>().color = Color.coral;
                flyBtn.GetComponent<Image>().color = Color.white;
                jellyBtn.GetComponent<Image>().color = Color.white;
                scenePowerManager.GetComponent<ScenePowerManager>().TogglePowerTest(3);
            });
        }

    }
}
