using OverkillDocs.Tests.E2E.Components;

namespace OverkillDocs.Tests.E2E.Tests.Document;

[Collection(OkdCollection.Name)]
public class ArticleViewerTests(PlaywrightFixture fixture, ITestOutputHelper outputHelper) : TestBase(fixture, outputHelper)
{
    [Fact]
    public async Task DocumentCRUDShouldSync()
    {
        var (_, desktopPage) = await NewBrowserSession();
        var (_, mobilePage) = await NewBrowserSession(mobile: true);

        var originalTitle = Ulid.NewUlid().ToString();
        var modifiedTitle = Ulid.NewUlid().ToString();

        LogData(originalTitle, modifiedTitle);

        await desktopPage.GotoAsync(Routes.Documents);
        await mobilePage.GotoAsync(Routes.Documents);

        await CreateDocumentAndRedirect(originalTitle, desktopPage, mobilePage);
        await RenameDocument(originalTitle, modifiedTitle, desktopPage, mobilePage);
        await DeleteDocumentAndRedirect(modifiedTitle, desktopPage, mobilePage);
    }

    [Fact]
    public async Task FragmentCRUDShouldSync()
    {
        var (_, desktopPage) = await NewBrowserSession();
        var (_, mobilePage) = await NewBrowserSession(mobile: true);

        var documentTitle = Ulid.NewUlid().ToString();
        var imageAlt = Ulid.NewUlid().ToString();

        LogData(documentTitle, imageAlt);

        await desktopPage.GotoAsync(Routes.Documents);
        await mobilePage.GotoAsync(Routes.Documents);

        await CreateDocumentAndRedirect(documentTitle, desktopPage, mobilePage);
        await new SidebarComponent(desktopPage).Close();
        await new SidebarComponent(mobilePage).Close();

        await CreateImageFragment(imageAlt, desktopPage, mobilePage);
        await DeleteFragment(imageAlt, desktopPage, mobilePage);
    }

    private static async Task CreateDocumentAndRedirect(string title, IPage desktopPage, IPage mobilePage)
    {
        var desktopSidebar = new SidebarComponent(desktopPage);
        var mobileSidebar = new SidebarComponent(mobilePage);

        await desktopSidebar.ShowDocumentIndex();
        await mobileSidebar.ShowDocumentIndex();

        await desktopSidebar.DocumentIndex.AddArticle(title);
        await desktopSidebar.DocumentIndex.ExpectDocumentVisibility(title, visible: true);
        await mobileSidebar.DocumentIndex.ExpectDocumentVisibility(title, visible: true);

        await desktopSidebar.DocumentIndex.SelectDocument(title);
        await mobileSidebar.DocumentIndex.SelectDocument(title);
    }

    private static async Task RenameDocument(string originalTitle, string modifiedTitle, IPage desktopPage, IPage mobilePage)
    {
        var desktop = new
        {
            Article = new ArticleViewerPage(desktopPage),
            Sidebar = new SidebarComponent(desktopPage)
        };

        var mobile = new
        {
            Article = new ArticleViewerPage(mobilePage),
            Sidebar = new SidebarComponent(mobilePage)
        };

        await desktop.Sidebar.Close();
        await mobile.Sidebar.Close();

        await desktop.Article.ExpectTitleToBe(originalTitle);
        await mobile.Article.ExpectTitleToBe(originalTitle);

        await mobile.Article.ChangeTitle(modifiedTitle);
        await desktop.Article.ExpectTitleToBe(modifiedTitle);
        await mobile.Article.ExpectTitleToBe(modifiedTitle);

        await desktop.Sidebar.ShowDocumentIndex();
        await desktop.Sidebar.DocumentIndex.ExpectDocumentVisibility(originalTitle, visible: false);
        await desktop.Sidebar.DocumentIndex.ExpectDocumentVisibility(modifiedTitle, visible: true);

        await mobile.Sidebar.ShowDocumentIndex();
        await mobile.Sidebar.DocumentIndex.ExpectDocumentVisibility(originalTitle, visible: false);
        await mobile.Sidebar.DocumentIndex.ExpectDocumentVisibility(modifiedTitle, visible: true);
    }

    private static async Task DeleteDocumentAndRedirect(string title, IPage desktopPage, IPage mobilePage)
    {
        var desktop = new
        {
            Article = new ArticleViewerPage(desktopPage),
            Intro = new DocumentIntroPage(desktopPage),
            Sidebar = new SidebarComponent(desktopPage)
        };

        var mobile = new
        {
            Article = new ArticleViewerPage(mobilePage),
            Intro = new DocumentIntroPage(mobilePage),
            Sidebar = new SidebarComponent(mobilePage)
        };

        await desktop.Sidebar.ShowDocumentIndex();
        await desktop.Sidebar.DocumentIndex.SelectDocument(title);
        await desktop.Sidebar.Close();
        await desktop.Article.ExpectTitleToBe(title);

        await mobile.Sidebar.ShowDocumentIndex();
        await mobile.Sidebar.DocumentIndex.SelectDocument(title);
        await mobile.Sidebar.Close();
        await mobile.Article.ExpectTitleToBe(title);

        await desktop.Article.DeleteCurrentDocument();

        await desktop.Sidebar.ShowDocumentIndex();
        await desktop.Sidebar.DocumentIndex.ExpectDocumentVisibility(title, visible: false);
        await mobile.Sidebar.ShowDocumentIndex();
        await mobile.Sidebar.DocumentIndex.ExpectDocumentVisibility(title, visible: false);

        await desktop.Intro.ExpectToBeVisible();
        await mobile.Intro.ExpectToBeVisible();
    }

    private static async Task CreateImageFragment(string imageAlt, IPage desktopPage, IPage mobilePage)
    {
        var desktopArticle = new ArticleViewerPage(desktopPage);
        var mobileArticle = new ArticleViewerPage(mobilePage);

        await desktopArticle.EnableEditMode();
        await mobileArticle.EnableEditMode();

        await desktopArticle.CreateImageFragment(url: "/assets/favicon.ico", alt: imageAlt);
        await mobileArticle.ExpectFragmentLock(isLocked: true);

        await desktopArticle.FinishEdit();
        await mobileArticle.ExpectFragmentLock(isLocked: false);

        await desktopArticle.ExpectElementWithAlt(imageAlt);
        await mobileArticle.ExpectElementWithAlt(imageAlt);
    }

    private static async Task DeleteFragment(string text, IPage desktopPage, IPage mobilePage)
    {
        var desktopArticle = new ArticleViewerPage(desktopPage);
        var mobileArticle = new ArticleViewerPage(mobilePage);

        await desktopArticle.EnableEditMode();
        await mobileArticle.EnableEditMode();

        await mobileArticle.DeleteFragment();

        await desktopArticle.ExpectNoElementWithAlt(text);
        await mobileArticle.ExpectNoElementWithAlt(text);
    }
}
