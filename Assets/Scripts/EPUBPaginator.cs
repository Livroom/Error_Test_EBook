using System.Collections.Generic;
using System.Text.RegularExpressions;
using VersOne.Epub;

public class EPUBPaginator
{
    // 페이지별 텍스트를 담는 리스트
    public List<string> Pages { get; private set; } = new List<string>();

    // EPUB 책 객체와 페이지당 문장 수를 입력받기
    public EPUBPaginator(EpubBook book, int sentencesPerPage = 8)
    {
        string fullText = "";

        // 책의 모든 챕터 텍스트를 하나의 문자열로 합치기
        foreach (var chapter in book.ReadingOrder)
        {
            fullText += StripHtml(chapter.Content) + "/n";
        }

        // 문장 단위로 나누어 페이지를 생성
        PaginateBySentences(fullText, sentencesPerPage);
    }

    // 문장 단위로 페이지 나누기
    void PaginateBySentences(string text, int sentencesPerPage)
    {
        List<string> sentences = SplitIntoSentences(text);

        // sentencesPerPage에 따라 문장씩 묶어서 페이지 구성
        for (int i = 0; i < sentences.Count; i += sentencesPerPage)
        {
            int count = System.Math.Min(sentencesPerPage, sentences.Count - i);
            string pageText = string.Join(" ", sentences.GetRange(i, count));
            Pages.Add(pageText.Trim());
        }
    }

    // 전체 텍스트를 문장 단위로 분리
    List<string> SplitIntoSentences(string text)
    {
        // 문장 단위 분리 (문단부호)
        var regex = new Regex(@"(?<=[\.!\?…])\s+");
        var sentences = regex.Split(text);
        var result = new List<string>();

        // 공백 제거하고 빈 문장 제거
        foreach (var s in sentences)
        {
            string trimmed = s.Trim();
            if (!string.IsNullOrEmpty(trimmed))
                result.Add(trimmed);
        }

        return result;
    }

    string StripHtml(string input)
    {
        return Regex.Replace(input, "<.*?>", string.Empty);
    }
}

