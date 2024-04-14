using OpenQA.Selenium;

namespace ShopManager.Web.SeleniumTests;

public class GeneralTests : SeleniumTestFixture
{
    private const string WebAppUrl = "http://localhost:3000";
    
    [Test, Order(1)]
    public void NavBar_Has_LoginButton()
    {
        // Arrange
        Driver.Navigate()
            .GoToUrl(WebAppUrl);
        
        // Act
        
        // Assert
        var element = Driver.FindElement(By.XPath("/html/body/div[1]/div[3]/header/div/button[2]/span"));
        Assert.NotNull(element);
        Assert.That(element.Text, Is.EqualTo("LOGIN"));
    }
    
    [Test, Order(2)]
    public void NavBar_Has_RegisterButton()
    {
        // Arrange
        Driver.Navigate()
            .GoToUrl(WebAppUrl);
        
        // Act
        
        // Assert
        var element = Driver.FindElement(By.XPath("/html/body/div[1]/div[3]/header/div/button[3]/span"));
        Assert.NotNull(element);
        Assert.That(element.Text, Is.EqualTo("REGISTER"));
    }
      
    [Test, Order(3)]
    public void LoginButton_Shows_LoginDialog()
    {
        // Arrange
        Driver.Navigate()
            .GoToUrl(WebAppUrl);
        
        // Act
        var loginButton = Driver.FindElement(By.XPath("/html/body/div[1]/div[3]/header/div/button[2]/span"));
        loginButton.Click();
        
        // Assert
        var dialog = Driver.FindElement(By.XPath("//div[@role='dialog']"));
        Assert.NotNull(dialog);
    }
      
    [Test, Order(4)]
    public void RegisterButton_Shows_RegisterDialog()
    {
        // Arrange
        Driver.Navigate()
            .GoToUrl(WebAppUrl);
        
        // Act
        var registerButton = Driver.FindElement(By.XPath("/html/body/div[1]/div[3]/header/div/button[3]/span"));
        registerButton.Click();
        
        // Assert
        var dialog = Driver.FindElement(By.XPath("//div[@role='dialog']"));
        Assert.NotNull(dialog);
    }

    [Test, Order(5)]
    public void LoginDialog_Allows_Login()
    {
        // Arrange
        Driver.Navigate()
            .GoToUrl(WebAppUrl);

        // Act
        var loginButton = Driver.FindElement(By.XPath("/html/body/div[1]/div[3]/header/div/button[2]/span"));
        loginButton.Click();

        var emailInput = Driver.FindElement(By.XPath("//input[@type='email']"));
        emailInput.SendKeys("contact@senti.dev");
        
        var passwordInput = Driver.FindElement(By.XPath("//input[@type='password']"));
        passwordInput.SendKeys("Admin123!");
        
        var submitButton = Driver.FindElement(By.XPath("/html/body/div[1]/div[2]/div[2]/div[2]/div[4]/div[2]/button[2]"));
        submitButton.Click();
        
        // Assert
        var userNameChip = Driver.FindElement(By.XPath("/html/body/div[1]/div[3]/header/div/div[2]/div[1]/div"));
        Assert.NotNull(userNameChip);
    }
    
    [Test, Order(6)]
    public void ProductTab_Shows_Products()
    {
        // Arrange
        Driver.Navigate()
            .GoToUrl(WebAppUrl);

        // Act
        var productTab = Driver.FindElement(By.XPath("/html/body/div[1]/div[3]/aside/div/div/div[2]/a"));
        productTab.Click();
        
        // Assert
        Assert.That(Driver.Url, Is.EqualTo($"{WebAppUrl}/products"));
    }
    
    [Test, Order(7)]
    public void DeleteProductButton_Shows_DeleteDialog()
    {
        // Arrange
        Driver.Navigate()
            .GoToUrl($"{WebAppUrl}/products");

        // Act
        var deleteButton = Driver.FindElement(By.XPath("/html/body/div[1]/div[3]/div/div/div[3]/table/tbody/tr[1]/td[5]/div[3]/button"));
        deleteButton.Click();
        
        // Assert
        var dialog = Driver.FindElement(By.XPath("//div[@role='dialog']"));
        Assert.NotNull(dialog);
    }
    
    [Test, Order(8)]
    public void EditProductButton_Shows_EditDialog()
    {
        // Arrange
        Driver.Navigate()
            .GoToUrl($"{WebAppUrl}/products");

        // Act
        var editButton = Driver.FindElement(By.XPath("/html/body/div[1]/div[3]/div/div/div[3]/table/tbody/tr[1]/td[5]/div[2]/button"));
        editButton.Click();
        
        // Assert
        var dialog = Driver.FindElement(By.XPath("//div[@role='dialog']"));
        Assert.NotNull(dialog);
    }
    
    [Test, Order(9)]
    public void AddProductButton_Shows_AddDialog()
    {
        // Arrange
        Driver.Navigate()
            .GoToUrl($"{WebAppUrl}/products");

        // Act
        var addButton = Driver.FindElement(By.XPath("/html/body/div[1]/div[3]/div/div/div[1]/button"));
        addButton.Click();
        
        // Assert
        var dialog = Driver.FindElement(By.XPath("//div[@role='dialog']"));
        Assert.NotNull(dialog);
    }
    
    [Test, Order(10)]
    public void ProductDetailsButton_RedirectsTo_ProductDetails()
    {
        // Arrange
        Driver.Navigate()
            .GoToUrl($"{WebAppUrl}/products");
        var productId = Guid.Parse(Driver.FindElement(By.XPath("/html/body/div[1]/div[3]/div/div/div[3]/table/tbody/tr[1]/td[1]")).Text);
        
        // Act
        var detailsButton = Driver.FindElement(By.XPath("/html/body/div[1]/div[3]/div/div/div[3]/table/tbody/tr[1]/td[5]/div[1]/a"));
        detailsButton.Click();
        
        // Assert
        Assert.That(Driver.Url, Does.StartWith($"{WebAppUrl}/products/"));
    }
}