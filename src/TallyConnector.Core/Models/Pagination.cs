namespace TallyConnector.Core.Models;
public class Pagination
{

    public Pagination(int totalCount, int pageSize = 100)
    {
        TotalCount = totalCount;
        PageSize = pageSize;
        PageNum = 1;
        IntialCalculate();
    }

    public Pagination(int totalCount, int pageSize = 100, int pageNum = 1)
    {
        TotalCount = totalCount;
        PageNum = pageNum;
        PageSize = pageSize;
        IntialCalculate();
        GoToPage(PageNum);
    }


    public int TotalCount { get; }
    public int TotalPages { get; private set; }
    public int PageSize { get; }
    public int PageNum { get; private set; }
    public int Start { get; private set; }
    public int End { get; private set; }

    public string GetFilterFormulae()
    {
        return $"##vLineIndex <= {End} AND ##vLineIndex > {Start}";
    }
    public void NextPage()
    {
        PageNum++;
        Start += PageSize;

        End += PageSize;
        if (End > TotalCount)
        {
            End = TotalCount;
        }
    }
    public void GoToPage(int pageNum)
    {
        if (pageNum <= TotalPages)
        {
            PageNum = pageNum;
            Start = PageSize * (pageNum - 1);
            End = PageSize * pageNum;
            if (End > TotalCount)
            {
                End = TotalCount;
            }
        }
        else
        {
            throw new InvalidOperationException($"pageNum - {pageNum} should be less than {TotalPages}");
        }
    }
    public void IntialCalculate()
    {
        TotalPages = (int)Math.Ceiling((decimal)TotalCount / PageSize);
        Start = 0;
        End = 0 + PageSize;
    }
}
