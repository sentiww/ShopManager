using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ShopManager.Web.SeleniumTests;

[TestFixture]
public class SeleniumTestFixture : IDisposable
{
    protected readonly IWebDriver Driver;
    public SeleniumTestFixture()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless");
        Driver = new ChromeDriver(options);
        Driver.Manage().Window.Size = new System.Drawing.Size(1920, 1080);
        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }

    public void Dispose()
    {
        Driver.Quit();
        Driver.Dispose();
    }
}