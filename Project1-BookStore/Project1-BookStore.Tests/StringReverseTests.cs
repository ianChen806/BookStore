namespace Project1_BookStore.Tests;

public class StringReverseTests
{
    // 題目:將字串反轉,且「不可使用內建反轉方法」
    //       (禁止 Array.Reverse / Enumerable.Reverse / 任何現成的 Reverse)
    [Fact(Skip = "面試題:實作 Reverse 後移除此 Skip")]
    public void Reverse_GivenString_ReturnsReversed()
    {
        // Arrange
        var input = "hello";
        var expected = "olleh";

        // Act
        var actual = Reverse(input);

        // Assert
        Assert.Equal(expected, actual);

        // TODO(面試者):實作此方法 —— 不可使用任何內建反轉
        static string Reverse(string value)
        {
            throw new NotImplementedException();
        }
    }
}
