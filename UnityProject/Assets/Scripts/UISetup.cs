using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISetup : MonoBehaviour
{
    [ContextMenu("Create Complete UI")]
    void CreateCompleteUI()
    {
        // Este script solo se usa UNA VEZ para crear toda la UI
        // Después se puede eliminar
        
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No se encontró Canvas en la escena!");
            return;
        }

        CreateMainMenu(canvas.transform);
        CreateHUD(canvas.transform);
        CreateCountdown(canvas.transform);
        CreateRoundComplete(canvas.transform);
        CreateVictory(canvas.transform);
        CreateGameOver(canvas.transform);
        
        Debug.Log("✅ UI completa creada!");
    }

    void CreateMainMenu(Transform parent)
    {
        GameObject panel = CreatePanel("MainMenuPanel", parent);
        panel.GetComponent<Image>().color = new Color(0, 0, 0, 0.8f);
        
        GameObject title = CreateText("Title", panel.transform);
        SetupText(title, "SMURF SHOOTER", 80, TextAlignmentOptions.Center);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 300);
        
        GameObject button = CreateButton("PlayButton", panel.transform, "JUGAR");
        button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
    }

    void CreateHUD(Transform parent)
    {
        GameObject hudPanel = new GameObject("HUDPanel");
        hudPanel.transform.SetParent(parent, false);
        RectTransform rect = hudPanel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        
        // Timer
        GameObject timer = CreateText("TimerText", hudPanel.transform);
        SetupText(timer, "00:00", 60, TextAlignmentOptions.Center);
        SetAnchor(timer, new Vector2(0.5f, 1f), new Vector2(0, -50));
        
        // Round
        GameObject round = CreateText("RoundText", hudPanel.transform);
        SetupText(round, "Ronda 1/3", 40, TextAlignmentOptions.Left);
        SetAnchor(round, new Vector2(0, 1f), new Vector2(150, -50));
        
        // Smurfs
        GameObject smurfs = CreateText("SmurfsText", hudPanel.transform);
        SetupText(smurfs, "Pitufos: 0/5", 40, TextAlignmentOptions.Right);
        SetAnchor(smurfs, new Vector2(1f, 1f), new Vector2(-150, -50));
        
        // Ammo
        GameObject ammo = CreateText("AmmoText", hudPanel.transform);
        SetupText(ammo, "Tomates: 15/15", 50, TextAlignmentOptions.Center);
        SetAnchor(ammo, new Vector2(0.5f, 0f), new Vector2(0, 100));
        
        hudPanel.SetActive(false);
    }

    void CreateCountdown(Transform parent)
    {
        GameObject panel = CreatePanel("CountdownPanel", parent);
        panel.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        
        GameObject text = CreateText("CountdownText", panel.transform);
        SetupText(text, "3", 200, TextAlignmentOptions.Center);
        text.GetComponent<TextMeshProUGUI>().color = Color.yellow;
        
        panel.SetActive(false);
    }

    void CreateRoundComplete(Transform parent)
    {
        GameObject panel = CreatePanel("RoundCompletePanel", parent);
        panel.GetComponent<Image>().color = new Color(0, 0, 0, 0.7f);
        
        GameObject text = CreateText("RoundCompleteText", panel.transform);
        SetupText(text, "¡Ronda Completada!", 60, TextAlignmentOptions.Center);
        
        panel.SetActive(false);
    }

    void CreateVictory(Transform parent)
    {
        GameObject panel = CreatePanel("VictoryPanel", parent);
        panel.GetComponent<Image>().color = new Color(0, 0.5f, 0, 0.9f);
        
        GameObject title = CreateText("VictoryTitle", panel.transform);
        SetupText(title, "¡VICTORIA!\n\n¡Has Completado el Juego!", 70, TextAlignmentOptions.Center);
        title.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 200);
        title.GetComponent<TextMeshProUGUI>().color = Color.yellow;
        
        GameObject playAgain = CreateButton("PlayAgainButton", panel.transform, "JUGAR DE NUEVO");
        playAgain.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -50);
        
        GameObject menu = CreateButton("MenuButton", panel.transform, "MENÚ PRINCIPAL");
        menu.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -200);
        
        panel.SetActive(false);
    }

    void CreateGameOver(Transform parent)
    {
        GameObject panel = CreatePanel("GameOverPanel", parent);
        panel.GetComponent<Image>().color = new Color(0.5f, 0, 0, 0.9f);
        
        GameObject text = CreateText("GameOverText", panel.transform);
        SetupText(text, "GAME OVER", 70, TextAlignmentOptions.Center);
        text.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 200);
        
        GameObject playAgain = CreateButton("PlayAgainButton", panel.transform, "INTENTAR DE NUEVO");
        playAgain.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -50);
        
        GameObject menu = CreateButton("MenuButton", panel.transform, "MENÚ PRINCIPAL");
        menu.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -200);
        
        panel.SetActive(false);
    }

    GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        
        Image img = panel.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.8f);
        
        return panel;
    }

    GameObject CreateText(string name, Transform parent)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(800, 200);
        
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = 50;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        
        return textObj;
    }

    GameObject CreateButton(string name, Transform parent, string text)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(500, 120);
        
        Image img = buttonObj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.6f, 1f);
        
        Button btn = buttonObj.AddComponent<Button>();
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 40;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        return buttonObj;
    }

    void SetupText(GameObject textObj, string text, float fontSize, TextAlignmentOptions alignment)
    {
        TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
    }

    void SetAnchor(GameObject obj, Vector2 anchor, Vector2 position)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.anchoredPosition = position;
    }
}