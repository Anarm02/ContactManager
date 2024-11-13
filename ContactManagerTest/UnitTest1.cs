namespace ContactManagerTest
{
	public class UnitTest1
	{
		[Fact]
		public void Test1()
		{
			MyMath myMath = new MyMath();
			int a = 10;int b = 20; int expected = 30;
			int result=myMath.Add(a,b);
			Assert.Equal(expected,result);
		}
	}
}