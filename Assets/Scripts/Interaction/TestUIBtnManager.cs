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
            ScenePowerManager.Power player = scenePowerManager.GetComponent<ScenePowerManager>().GetCurrentPower();
            switch (player)
            {
                case ScenePowerManager.Power.jellyVision:
                    jellyBtn.GetComponent<Image>().color = Color.coral;
                    break;
                case ScenePowerManager.Power.flyingPower:
                    flyBtn.GetComponent<Image>().color = Color.coral;
                    break;
                case ScenePowerManager.Power.sizeManipulationPower:
                    scaleBtn.GetComponent<Image>().color = Color.coral;
                    break;
                default:
                    break;
            }
            // Add onclick events
            jellyBtn.GetComponent<Button>().onClick.AddListener( () => {
                jellyBtn.GetComponent<Image>().color = Color.coral;
                flyBtn.GetComponent <Image>().color = Color.white;
                scaleBtn.GetComponent<Image>().color = Color.white;
                scenePowerManager.GetComponent<ScenePowerManager>().SetPlayerPower(ScenePowerManager.Power.jellyVision);
            });
            flyBtn.GetComponent<Button>().onClick.AddListener(() => {
                flyBtn.GetComponent<Image>().color = Color.coral;
                jellyBtn.GetComponent<Image>().color = Color.white;
                scaleBtn.GetComponent<Image>().color = Color.white;
                scenePowerManager.GetComponent<ScenePowerManager>().SetPlayerPower(ScenePowerManager.Power.flyingPower);
            });
            scaleBtn.GetComponent<Button>().onClick.AddListener(() => {
                scaleBtn.GetComponent<Image>().color = Color.coral;
                flyBtn.GetComponent<Image>().color = Color.white;
                jellyBtn.GetComponent<Image>().color = Color.white;
                scenePowerManager.GetComponent<ScenePowerManager>().SetPlayerPower(ScenePowerManager.Power.sizeManipulationPower);
            });
        }

    }
}
