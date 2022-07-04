namespace Tests.Models;
internal class PaginationTests
{

    [Test]
    public void TestConstructor()
    {
        TCM.Pagination pagination = new(500, 100);
        Assert.AreEqual(pagination.TotalCount,500);
        Assert.AreEqual(pagination.TotalPages,5);
        Assert.AreEqual(pagination.PageNum,1);
        Assert.AreEqual(pagination.Start,0);
        Assert.AreEqual(pagination.End,100);
    }

    [Test]
    public void TestConstructorVariant2()
    {
        TCM.Pagination pagination = new(750, 100);
        Assert.AreEqual(pagination.TotalCount, 750);
        Assert.AreEqual(pagination.TotalPages, 8);
        Assert.AreEqual(pagination.PageNum, 1);
        Assert.AreEqual(pagination.Start, 0);
        Assert.AreEqual(pagination.End, 100);
    }

    [Test]
    public void TestNextPage()
    {
        TCM.Pagination pagination = new(750, 100);

        Assert.AreEqual(pagination.TotalCount, 750);
        Assert.AreEqual(pagination.TotalPages, 8);
        Assert.AreEqual(pagination.PageNum, 1);
        Assert.AreEqual(pagination.Start, 0);
        Assert.AreEqual(pagination.End, 100);

        pagination.NextPage();
        Assert.AreEqual(pagination.PageNum,2);
        Assert.AreEqual(pagination.Start,100);
        Assert.AreEqual(pagination.End,200);
    }
    [Test]
    public void TestNextPageVariant2()
    {
        TCM.Pagination pagination = new(785, 50);

        Assert.AreEqual(pagination.TotalCount, 785);
        Assert.AreEqual(pagination.TotalPages, 16);
        Assert.AreEqual(pagination.PageNum, 1);
        Assert.AreEqual(pagination.Start, 0);
        Assert.AreEqual(pagination.End, 50);


        pagination.NextPage();
        Assert.AreEqual(pagination.PageNum, 2);
        Assert.AreEqual(pagination.Start, 50);
        Assert.AreEqual(pagination.End, 100);
    }
    
    [Test]
    public void TestLastPage()
    {
        TCM.Pagination pagination = new(150, 100);

        Assert.AreEqual(pagination.TotalCount, 150);
        Assert.AreEqual(pagination.TotalPages, 2);
        Assert.AreEqual(pagination.PageNum, 1);
        Assert.AreEqual(pagination.Start, 0);
        Assert.AreEqual(pagination.End, 100);


        pagination.NextPage();
        Assert.AreEqual(pagination.PageNum, 2);
        Assert.AreEqual(pagination.Start, 100);
        Assert.AreEqual(pagination.End, 150);

    }
    [Test]
    public void TestLastPageVariant2()
    {
        TCM.Pagination pagination = new(75, 40);

        Assert.AreEqual(pagination.TotalCount, 75);
        Assert.AreEqual(pagination.TotalPages, 2);
        Assert.AreEqual(pagination.PageNum, 1);
        Assert.AreEqual(pagination.Start, 0);
        Assert.AreEqual(pagination.End, 40);


        pagination.NextPage();
        Assert.AreEqual(pagination.PageNum, 2);
        Assert.AreEqual(pagination.Start, 40);
        Assert.AreEqual(pagination.End, 75);
    }
    [Test]
    public void TestGoToPage()
    {
        TCM.Pagination pagination = new(452, 40);

        Assert.AreEqual(pagination.TotalCount, 452);
        Assert.AreEqual(pagination.TotalPages, 12);
        Assert.AreEqual(pagination.PageNum, 1);
        Assert.AreEqual(pagination.Start, 0);
        Assert.AreEqual(pagination.End, 40);


        pagination.GoToPage(2);
        Assert.AreEqual(pagination.PageNum, 2);
        Assert.AreEqual(pagination.Start, 40);
        Assert.AreEqual(pagination.End, 80);

        pagination.GoToPage(12);
        Assert.AreEqual(pagination.PageNum, 12);
        Assert.AreEqual(pagination.Start, 440);
        Assert.AreEqual(pagination.End, 452);
    }

}
