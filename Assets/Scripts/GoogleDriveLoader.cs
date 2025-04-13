using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

// ����̺꿡�� ���� ��������
public class GoogleDriveEpubReader : MonoBehaviour
{
    private string fileId = "1YJJOgHhtc5_dQErguahwTBZdWN1sgvIR";
    private string localPath;

    // Start() �޼���� �ٿ�ε� ����
    void Start()
    {
        localPath = Path.Combine(Application.persistentDataPath, "TestBook.epub");
        StartCoroutine(DownloadEpub());
    }

    // ���� ����̺꿡�� EPUB �ٿ�ε�
    IEnumerator DownloadEpub()
    {
        // ���� ����̺� url
        string url = $"https://drive.google.com/uc?export=download&id={fileId}";
        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            File.WriteAllBytes(localPath, www.downloadHandler.data);
            Debug.Log("EPUB �ٿ�ε� �Ϸ�!");
            // �ٿ�ε� �Ϸ� �� EPUB �Ľ��ϱ�
            LoadEpub(localPath);
        }
        else
        {
            Debug.LogError("EPUB �ٿ�ε� ����: " + www.error);
        }
    }

    // EPUB ������ �ε��ϰ� �Ľ��ϴ� �ڵ�
    void LoadEpub(string path)
    {
        // ���⼭ EPUB ������ VersOne.Epub ���̺귯���� �Ľ�
    }
}
