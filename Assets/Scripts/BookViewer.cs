using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using Unity.SharpZipLib.Zip;
using UnityEngine.Networking;
using HtmlAgilityPack;

// 이게 처음에 파싱한거 스트림으로 받아서
// 일단 epub이 압축인가 그래서 압축 풀고  가져온건데, 이건 무조건 streamingasset에 저장해서 불러와야 했던걸로 기억
public class BookViewer : MonoBehaviour
{
    public TextMeshProUGUI pageText;

    private List<string> pages = new List<string>();
    private int currentPageIndex = 0;

    void Start()
    {
        StartCoroutine(LoadEPUBFromStreamingAssets("TestBook.epub"));
    }

    IEnumerator LoadEPUBFromStreamingAssets(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);

        UnityWebRequest www = UnityWebRequest.Get(path);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("EPUB 파일 불러오기 실패: " + www.error);
        }
        else
        {
            byte[] epubData = www.downloadHandler.data;
            string extractPath = Path.Combine(Application.persistentDataPath, "ExtractedBook");

            using (MemoryStream ms = new MemoryStream(epubData))
            {
                UnzipEPUB(ms, extractPath);
            }

            // XHTML 파일 읽고 텍스트 추출하기
            string[] xhtmlFiles = Directory.GetFiles(extractPath, "*.xhtml", SearchOption.AllDirectories);
            foreach (string file in xhtmlFiles)
            {
                string html = File.ReadAllText(file);
                string plainText = ExtractTextFromHtml(html);
                SplitIntoPages(plainText, 800);
            }

            ShowPage();
        }
    }

    // EPUB 압축 해제
    void UnzipEPUB(Stream inputStream, string outputPath)
    {
        using (ZipInputStream zis = new ZipInputStream(inputStream))
        {
            ZipEntry entry;
            while ((entry = zis.GetNextEntry()) != null)
            {
                string fullPath = Path.Combine(outputPath, entry.Name);
                string directory = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                if (!entry.IsDirectory)
                {
                    using (FileStream fs = File.Create(fullPath))
                    {
                        zis.CopyTo(fs);
                    }
                }
            }
        }
    }
    
    // HTML로부터 텍스트 추출
    string ExtractTextFromHtml(string html)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(html);
        return doc.DocumentNode.InnerText.Trim();
    }

    void SplitIntoPages(string content, int charsPerPage)
    {
        for (int i = 0; i < content.Length; i += charsPerPage)
        {
            string page = content.Substring(i, Mathf.Min(charsPerPage, content.Length - i));
            pages.Add(page);
        }
    }

    void ShowPage()
    {
        if (pages.Count > 0)
            pageText.text = pages[currentPageIndex];
        else
            pageText.text = "(EPUB에서 텍스트를 찾을 수 없습니다)";
    }

    public void NextPage()
    {
        if (currentPageIndex + 1 < pages.Count)
        {
            currentPageIndex++;
            ShowPage();
        }
    }

    public void PreviousPage()
    {
        if (currentPageIndex - 1 >= 0)
        {
            currentPageIndex--;
            ShowPage();
        }
    }
}
