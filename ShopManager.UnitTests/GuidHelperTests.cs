using ShopManager.Client.Common;

namespace ShopManager.UnitTests;

public class GuidHelperTests
{
    [Test]
    public void ToBase64String_Returns_Base64String()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var result = guid.ToBase64String();

        // Assert
        Assert.That(result, Is.Not.Null);
    }
    
    [Test]
    public void ToBase64String_Returns_Base64String_With_Length_22()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var result = guid.ToBase64String();

        // Assert
        Assert.That(result.Length, Is.EqualTo(22));
    }
    
    [Test]
    public void ToBase64String_Returns_Valid_Base64String()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var result = guid.ToBase64String();

        // Assert
        Assert.That(() => GuidHelper.FromBase64String(result), Throws.Nothing);
        Assert.That(GuidHelper.FromBase64String(result), Is.EqualTo(guid));
    }
    
    [Test]
    public void FromBase64String_Returns_Guid()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var base64String = guid.ToBase64String();

        if (base64String is null)
            throw new Exception();

        // Act
        var result = GuidHelper.FromBase64String(base64String);

        // Assert
        Assert.That(result, Is.EqualTo(guid));
    }
    
    [Test]
    public void FromBase64String_Returns_Valid_Base64String()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var base64String = guid.ToBase64String();

        // Act
        var result = GuidHelper.FromBase64String(base64String);

        // Assert
        Assert.That(result.ToBase64String(), Is.EqualTo(base64String));
    }
}