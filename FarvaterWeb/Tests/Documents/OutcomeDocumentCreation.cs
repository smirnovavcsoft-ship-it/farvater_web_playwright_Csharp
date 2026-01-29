using FarvaterWeb.Base;
using FarvaterWeb.Data;
using FarvaterWeb.Pages.Common;
using FarvaterWeb.Pages.Documents;
using Xunit.Abstractions;

namespace FarvaterWeb.Tests.Documents
{
    public class OutcomeDocumentCreationTests : BaseTest
    {
        private SideMenuPage SideMenu => new SideMenuPage(Page, Log, _test);
        public OutcomeDocumentCreationTests(ITestOutputHelper output) : base(output) { }
        

        
    }
}
