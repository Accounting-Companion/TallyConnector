namespace Tests.Models;
internal class PaginationTests
{

    [Test]
    public void TestConstructor()
    {
        TCM.Pagination pagination = new(500, 100);
        Assert.AreEqual(500, pagination.TotalCount);
        Assert.AreEqual(5, pagination.TotalPages);
        Assert.AreEqual(1, pagination.PageNum);
        Assert.AreEqual(0, pagination.Start);
        Assert.AreEqual(100, pagination.End);
    }

    [Test]
    public void TestConstructorVariant2()
    {
        TCM.Pagination pagination = new(750, 100);
        Assert.AreEqual(750, pagination.TotalCount);
        Assert.AreEqual(8, pagination.TotalPages);
        Assert.AreEqual(1, pagination.PageNum);
        Assert.AreEqual(0, pagination.Start);
        Assert.AreEqual(100, pagination.End);
    }

    [Test]
    public void TestNextPage()
    {
        TCM.Pagination pagination = new(750, 100);

        Assert.AreEqual(750, pagination.TotalCount);
        Assert.AreEqual(8, pagination.TotalPages);
        Assert.AreEqual(1, pagination.PageNum);
        Assert.AreEqual(0, pagination.Start);
        Assert.AreEqual(100, pagination.End);

        pagination.NextPage();
        Assert.AreEqual(2, pagination.PageNum);
        Assert.AreEqual(100, pagination.Start);
        Assert.AreEqual(200, pagination.End);
    }
    [Test]
    public void TestNextPageVariant2()
    {
        TCM.Pagination pagination = new(785, 50);

        Assert.AreEqual(785, pagination.TotalCount);
        Assert.AreEqual(16, pagination.TotalPages);
        Assert.AreEqual(1, pagination.PageNum);
        Assert.AreEqual(0, pagination.Start);
        Assert.AreEqual(50, pagination.End);

        pagination.NextPage();
        Assert.AreEqual(2, pagination.PageNum);
        Assert.AreEqual(50, pagination.Start);
        Assert.AreEqual(100, pagination.End);
    }
    
    [Test]
    public void TestLastPage()
    {
        TCM.Pagination pagination = new(150, 100);

        Assert.AreEqual(150, pagination.TotalCount);
        Assert.AreEqual(2, pagination.TotalPages);
        Assert.AreEqual(1, pagination.PageNum);
        Assert.AreEqual(0, pagination.Start);
        Assert.AreEqual(100, pagination.End);

        pagination.NextPage();
        Assert.AreEqual(2, pagination.PageNum);
        Assert.AreEqual(100, pagination.Start);
        Assert.AreEqual(150, pagination.End);
    }
    [Test]
    public void TestLastPageVariant2()
    {
        TCM.Pagination pagination = new(75, 40);

        Assert.AreEqual(75, pagination.TotalCount);
        Assert.AreEqual(2, pagination.TotalPages);
        Assert.AreEqual(1, pagination.PageNum);
        Assert.AreEqual(0, pagination.Start);
        Assert.AreEqual(40, pagination.End);

        pagination.NextPage();
        Assert.AreEqual(2, pagination.PageNum);
        Assert.AreEqual(40, pagination.Start);
        Assert.AreEqual(75, pagination.End);
    }
    [Test]
    public void TestGoToPage()
    {
        TCM.Pagination pagination = new(452, 40);

        Assert.AreEqual(452, pagination.TotalCount);
        Assert.AreEqual(12, pagination.TotalPages);
        Assert.AreEqual(1, pagination.PageNum);
        Assert.AreEqual(0, pagination.Start);
        Assert.AreEqual(40, pagination.End);

        pagination.GoToPage(2);
        Assert.AreEqual(2, pagination.PageNum);
        Assert.AreEqual(40, pagination.Start);
        Assert.AreEqual(80, pagination.End);

        pagination.GoToPage(12);
        Assert.AreEqual(12, pagination.PageNum);
        Assert.AreEqual(440, pagination.Start);
        Assert.AreEqual(452, pagination.End);
    }

}
