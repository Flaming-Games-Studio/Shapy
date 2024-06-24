using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlueprintVisualPrefab : MonoBehaviour
{
    public Image blueprintImage;
    public TextMeshProUGUI blueprintName;
    public Button blueprintButton;
    public BlueprintData blueprintData;

    public CraftingManager CM;

    private void OnDestroy()
    {
        CM.UnsubscribeBlueprintOnClickEvents(this);
    }
}
