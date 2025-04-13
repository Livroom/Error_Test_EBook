using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

// 드라이브에서 파일 가져오기
public class GoogleDriveEpubReader : MonoBehaviour
{
    private string fileId = "1YJJOgHhtc5_dQErguahwTBZdWN1sgvIR";
    private string localPath;

    // Start() 메서드로 다운로드 시작
    void Start()
    {
        localPath = Path.Combine(Application.persistentDataPath, "TestBook.epub");
        StartCoroutine(DownloadEpub());
    }

    // 구글 드라이브에서 EPUB 다운로드
    IEnumerator DownloadEpub()
    {
        // 구글 드라이브 url
        string url = $"https://drive.google.com/uc?export=download&id={fileId}";
        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            File.WriteAllBytes(localPath, www.downloadHandler.data);
            Debug.Log("EPUB 다운로드 완료!");
            // 다운로드 완료 후 EPUB 파싱하기
            LoadEpub(localPath);
        }
        else
        {
            Debug.LogError("EPUB 다운로드 실패: " + www.error);
        }
    }

    // EPUB 파일을 로드하고 파싱하는 코드
    void LoadEpub(string path)
    {
        // 여기서 EPUB 파일을 VersOne.Epub 라이브러리로 파싱
    }
}
