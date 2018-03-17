using Moq;
using NUnit.Framework;
using TestNinja.Mocking;

namespace TestNinja.UnitTests.Mocking
{
	[TestFixture]
	public class EmployeeControllerTests
	{
		[Test]
		public void DeleteEmployee_WhenCalled_DeleteEmployeeFromDb()
		{
			//Arrange
			var storage = new Mock<IEmployeeStorage>();
			var controller = new EmployeeController(storage.Object);

			//Action
			controller.DeleteEmployee(1);

			//Assert
			storage.Verify(s => s.DeleteEmployee(1));

		}

		[Test]
		public void DeleteEmployee_WhenCalled_ReturnRedirectObject()
		{
			//Arrange
			//Act
			//Assert
		}

	}
}
