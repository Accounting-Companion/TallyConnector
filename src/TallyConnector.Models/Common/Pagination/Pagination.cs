namespace TallyConnector.Models.Common.Pagination;

public class Pagination : PaginationBase
{

    public Pagination(ulong totalCount, int pageSize = 100) : base(totalCount, pageSize)
    {

        IntialCalculate();
    }

    public Pagination(ulong totalCount, int pageSize = 100, int pageNum = 1) : base(totalCount, pageSize, pageNum)
    {

        IntialCalculate();
        GoToPage(PageNum);
    }

    public int Start { get; private set; }
    public ulong End { get; private set; }

    public string GetFilterFormulae()
    {
        return $"##vLineIndex <= {End} AND ##vLineIndex > {Start}";
    }
    public void NextPage()
    {
        PageNum++;
        Start += PageSize;

        End += (ulong)PageSize;
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
            End = (ulong)(PageSize * pageNum);
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
        End = (ulong)(0 + PageSize);
    }
}
