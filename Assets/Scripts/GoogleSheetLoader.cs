using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using static System.Net.WebRequestMethods;
using System.Collections;



public class GoogleSheetLoader : MonoBehaviour
{
    string sheetData;
    public TMP_Text pageText;
    string sheetURL = "https://docs.google.com/spreadsheets/d/16OBIuRv1VhBSzz7-RiYpzeV0VYRmw6NgI-mW-9eJyyk/export?format=tsv&range=A2:B7";

    IEnumerator Start()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(sheetURL))
        {
            yield return www.SendWebRequest();

            if (www.isDone)
                sheetData = www.downloadHandler.text;
        }
        DisplayText();
    }

    void DisplayText()
    {
        string[] rows = sheetData.Split('\n');
        string[] columns = rows[0].Split('\t');

        pageText.text = columns[0] + "\n" + columns[1];
    }
}
