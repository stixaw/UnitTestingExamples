using System.Net;
using Moq;
using NUnit.Framework;
using TestNinja.Mocking;

namespace TestNinja.UnitTests.Mocking
{
	[TestFixture]
	public class InstallerHelperTests
	{
		private Mock<IFileDownloader> _fileDownloader;
		private InstallerHelper _installerHelper;

		[SetUp]
		public void SetUp()
		{
			_fileDownloader = new Mock<IFileDownloader>();
			_installerHelper = new InstallerHelper(_fileDownloader.Object);
		}

		[Test]
		public void DownloadInstaller_DownloadFails_ReturnFalse()
		{
			//Arrange
			_fileDownloader.Setup(fd => fd.DownloadFile(It.IsAny<string>(), null)).Throws<WebException>();

			//Action
			var result = _installerHelper.DownloadInstaller("customer", "installer");

			//Assert
			Assert.That(result, Is.False);
		}

		[Test]
		public void DownloadInstaller_DownloadSucceeds_ReturnTrue()
		{

			//Action
			var result = _installerHelper.DownloadInstaller("customer", "installer");

			//Assert
			Assert.That(result, Is.True);
		}

	}
}
