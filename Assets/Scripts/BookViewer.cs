using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using Unity.SharpZipLib.Zip;
using UnityEngine.Networking;
using HtmlAgilityPack;

// �̰� ó���� �Ľ��Ѱ� ��Ʈ������ �޾Ƽ�
// �ϴ� epub�� �����ΰ� �׷��� ���� Ǯ��  �����°ǵ�, �̰� ������ streamingasset�� �����ؼ� �ҷ��;� �ߴ��ɷ� ���
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
            Debug.LogError("EPUB ���� �ҷ����� ����: " + www.error);
        }
        else
        {
            byte[] epubData = www.downloadHandler.data;
            string extractPath = Path.Combine(Application.persistentDataPath, "ExtractedBook");

            using (MemoryStream ms = new MemoryStream(epubData))
            {
                UnzipEPUB(ms, extractPath);
            }

            // XHTML ���� �а� �ؽ�Ʈ �����ϱ�
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

    // EPUB ���� ����
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
    
    // HTML�κ��� �ؽ�Ʈ ����
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
            pageText.text = "(EPUB���� �ؽ�Ʈ�� ã�� �� �����ϴ�)";
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
