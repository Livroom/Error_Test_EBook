using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VersOne.Epub;
using System.IO;

public class EBookUIController : MonoBehaviour
{
    public TMP_Text pageText;
    public Button prevButton;
    public Button nextButton;

    private EPUBPaginator paginator;
    private int currentPage = 0;

    void Start()
    {
        LoadBook();
        prevButton.onClick.AddListener(PrevPage);
        nextButton.onClick.AddListener(NextPage);
    }

    // EPUB 파일 불러오기
    void LoadBook()
    {
        // 파일 경로 구성
        string path = Path.Combine(Application.streamingAssetsPath, "TestBook.epub");

        // 경로에 파일이 없다면 로그 띄우기
        if (!File.Exists(path))
        {
            Debug.LogError("EPUB 파일이 존재하지 않습니다.");
            return;
        }

        // 바이트 배열로 파일 전부 읽기
        byte[] epubBytes = File.ReadAllBytes(path);

        // 메모리 스트림으로 변환
        using (MemoryStream stream = new MemoryStream(epubBytes))
        {
            EpubBook book = EpubReader.ReadBook(stream);
            paginator = new EPUBPaginator(book, 8);
            ShowPage(0);
        }
    }

    // 텍스트 보여주기
    void ShowPage(int index)
    {
        // 만약 페이지가 없으면
        if (paginator == null || paginator.Pages.Count == 0)
        {
            pageText.text = "책이 없습니다.";
            return;
        }

        // 현재 페이지 번호 범위
        currentPage = Mathf.Clamp(index, 0, paginator.Pages.Count - 1);

        // 현재 페이지 텍스트 보여주기
        pageText.text = paginator.Pages[currentPage];

        // 버튼
        prevButton.interactable = currentPage > 0;
        nextButton.interactable = currentPage < paginator.Pages.Count - 1;
    }

    void PrevPage()
    {
        if (currentPage > 0)
            ShowPage(currentPage - 1);
    }

    void NextPage()
    {
        if (currentPage < paginator.Pages.Count - 1)
            ShowPage(currentPage + 1);
    }
}
