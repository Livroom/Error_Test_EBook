using System.Collections.Generic;
using System.Text.RegularExpressions;
using VersOne.Epub;

public class EPUBPaginator
{
    // �������� �ؽ�Ʈ�� ��� ����Ʈ
    public List<string> Pages { get; private set; } = new List<string>();

    // EPUB å ��ü�� �������� ���� ���� �Է¹ޱ�
    public EPUBPaginator(EpubBook book, int sentencesPerPage = 8)
    {
        string fullText = "";

        // å�� ��� é�� �ؽ�Ʈ�� �ϳ��� ���ڿ��� ��ġ��
        foreach (var chapter in book.ReadingOrder)
        {
            fullText += StripHtml(chapter.Content) + "/n";
        }

        // ���� ������ ������ �������� ����
        PaginateBySentences(fullText, sentencesPerPage);
    }

    // ���� ������ ������ ������
    void PaginateBySentences(string text, int sentencesPerPage)
    {
        List<string> sentences = SplitIntoSentences(text);

        // sentencesPerPage�� ���� ���徿 ��� ������ ����
        for (int i = 0; i < sentences.Count; i += sentencesPerPage)
        {
            int count = System.Math.Min(sentencesPerPage, sentences.Count - i);
            string pageText = string.Join(" ", sentences.GetRange(i, count));
            Pages.Add(pageText.Trim());
        }
    }

    // ��ü �ؽ�Ʈ�� ���� ������ �и�
    List<string> SplitIntoSentences(string text)
    {
        // ���� ���� �и� (���ܺ�ȣ)
        var regex = new Regex(@"(?<=[\.!\?��])\s+");
        var sentences = regex.Split(text);
        var result = new List<string>();

        // ���� �����ϰ� �� ���� ����
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

