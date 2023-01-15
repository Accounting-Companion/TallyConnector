using TallyConnector.Core.Models.Pagination;

namespace Tests.Models;
internal class PaginationTests
{

    [Test]
    public void TestConstructor()
    {
        Pagination pagination = new(500, 100);
        Assert.Multiple(() =>
        {
            Assert.That(pagination.TotalCount, Is.EqualTo(500));
            Assert.That(pagination.TotalPages, Is.EqualTo(5));
            Assert.That(pagination.PageNum, Is.EqualTo(1));
            Assert.That(pagination.Start, Is.EqualTo(0));
            Assert.That(pagination.End, Is.EqualTo(100));
        });
    }

    [Test]
    public void TestConstructorVariant2()
    {
        Pagination pagination = new(750, 100);


        Assert.Multiple(() =>
        {
            Assert.That(pagination.TotalCount, Is.EqualTo(750));
            Assert.That(pagination.TotalPages, Is.EqualTo(8));
            Assert.That(pagination.PageNum, Is.EqualTo(1));
            Assert.That(pagination.Start, Is.EqualTo(0));
            Assert.That(pagination.End, Is.EqualTo(100));
        });
    }

    [Test]
    public void TestNextPage()
    {
        Pagination pagination = new(750, 100);

        Assert.Multiple(() =>
        {
            Assert.That(pagination.TotalCount, Is.EqualTo(750));
            Assert.That(pagination.TotalPages, Is.EqualTo(8));
            Assert.That(pagination.PageNum, Is.EqualTo(1));
            Assert.That(pagination.Start, Is.EqualTo(0));
            Assert.That(pagination.End, Is.EqualTo(100));
        });
        pagination.NextPage();

        Assert.Multiple(() =>
        {
            Assert.That(pagination.PageNum, Is.EqualTo(2));
            Assert.That(pagination.Start, Is.EqualTo(100));
            Assert.That(pagination.End, Is.EqualTo(200));
        });
    }
    [Test]
    public void TestNextPageVariant2()
    {
        Pagination pagination = new(785, 50);

        Assert.Multiple(() =>
        {
            Assert.That(pagination.TotalCount, Is.EqualTo(785));
            Assert.That(pagination.TotalPages, Is.EqualTo(16));
            Assert.That(pagination.PageNum, Is.EqualTo(1));
            Assert.That(pagination.Start, Is.EqualTo(0));
            Assert.That(pagination.End, Is.EqualTo(50));
        });

        pagination.NextPage();

        Assert.Multiple(() =>
        {
            Assert.That(pagination.PageNum, Is.EqualTo(2));
            Assert.That(pagination.Start, Is.EqualTo(50));
            Assert.That(pagination.End, Is.EqualTo(100));
        });
    }

    [Test]
    public void TestLastPage()
    {
        Pagination pagination = new(150, 100);

        Assert.Multiple(() =>
        {
            Assert.That(pagination.TotalCount, Is.EqualTo(150));
            Assert.That(pagination.TotalPages, Is.EqualTo(2));
            Assert.That(pagination.PageNum, Is.EqualTo(1));
            Assert.That(pagination.Start, Is.EqualTo(0));
            Assert.That(pagination.End, Is.EqualTo(100));
        });

        pagination.NextPage();

        Assert.Multiple(() =>
        {
            Assert.That(pagination.PageNum, Is.EqualTo(2));
            Assert.That(pagination.Start, Is.EqualTo(100));
            Assert.That(pagination.End, Is.EqualTo(150));
        });

    }
    [Test]
    public void TestLastPageVariant2()
    {
        Pagination pagination = new(75, 40);


        Assert.Multiple(() =>
        {
            Assert.That(pagination.TotalCount, Is.EqualTo(75));
            Assert.That(pagination.TotalPages, Is.EqualTo(2));
            Assert.That(pagination.PageNum, Is.EqualTo(1));
            Assert.That(pagination.Start, Is.EqualTo(0));
            Assert.That(pagination.End, Is.EqualTo(40));
        });
        pagination.NextPage();
        Assert.Multiple(() =>
        {
            Assert.That(pagination.PageNum, Is.EqualTo(2));
            Assert.That(pagination.Start, Is.EqualTo(40));
            Assert.That(pagination.End, Is.EqualTo(75));
        });
    }
    [Test]
    public void TestGoToPage()
    {
        TCM.Pagination.Pagination pagination = new(452, 40);

        Assert.Multiple(() =>
        {
            Assert.That(pagination.TotalCount, Is.EqualTo(452));
            Assert.That(pagination.TotalPages, Is.EqualTo(12));
            Assert.That(pagination.PageNum, Is.EqualTo(1));
            Assert.That(pagination.Start, Is.EqualTo(0));
            Assert.That(pagination.End, Is.EqualTo(40));
        });

        pagination.GoToPage(2);
        Assert.Multiple(() =>
        {
            Assert.That(pagination.PageNum, Is.EqualTo(2));
            Assert.That(pagination.Start, Is.EqualTo(40));
            Assert.That(pagination.End, Is.EqualTo(80));
        });

        pagination.GoToPage(12);

        Assert.Multiple(() =>
        {
            Assert.That(pagination.PageNum, Is.EqualTo(12));
            Assert.That(pagination.Start, Is.EqualTo(440));
            Assert.That(pagination.End, Is.EqualTo(452));
        });
    }

}
