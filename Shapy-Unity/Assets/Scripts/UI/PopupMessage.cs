using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupMessage : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI message;
    private Image background;

    public void StartFade()
    {
        StartCoroutine(FadeAway());
    }
    private void OnEnable()
    {
        background = GetComponent<Image>();
    }

    IEnumerator FadeAway()
    {
        yield return new WaitForSeconds(2f);

        Color initialColor = background.color;

        float duration = 3f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(initialColor.a, 0, elapsedTime / duration);

            background.color = new Color(initialColor.r, initialColor.g, initialColor.b, newAlpha);
            title.color = new Color(title.color.r, title.color.g, title.color.b, newAlpha);
            message.color = new Color(title.color.r, title.color.g, title.color.b, newAlpha);

            yield return null;
        }

        background.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0);
        Destroy(gameObject);
    }
}
