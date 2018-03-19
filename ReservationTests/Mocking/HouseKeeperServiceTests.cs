using NUnit.Framework;
using Moq;
using TestNinja.Mocking;
using static TestNinja.Mocking.HouseKeeperService;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TestNinja.UnitTests.Mocking
{
	[TestFixture]
	public class HouseKeeperServiceTests
	{
		private HouseKeeperService _service;
		private Mock<IStatementGenerator> _statementGenerator;
		private Mock<IEmailSender> _emailSender;
		private Mock<IXtraMessageBox> _xtraMessageBox;
		private DateTime _statementDate = new DateTime(2017, 1, 1);
		private Housekeeper _houseKeeper;
		private string _statementFileName;

		[SetUp]
		public void SetUp()
		{
			_houseKeeper = new Housekeeper { Email = "a@a.com", FullName = "b", Oid = 1, StatementEmailBody = "statement" };
			_emailSender = new Mock<IEmailSender>();
			_xtraMessageBox = new Mock<IXtraMessageBox>();

			var unitOfWork = new Mock<IUnitOfWork>();
			unitOfWork.Setup(uow => uow.Query<Housekeeper>()).Returns(new List<Housekeeper>
			{
				_houseKeeper
			}.AsQueryable());

			_statementFileName = "fileName";
			_statementGenerator = new Mock<IStatementGenerator>();
			_statementGenerator
				.Setup(sg => sg.SaveStatement(_houseKeeper.Oid, _houseKeeper.FullName, _statementDate))
				.Returns(() => _statementFileName); //lazy evaluation

			_service = new HouseKeeperService(
				unitOfWork.Object,
				_statementGenerator.Object,
				_emailSender.Object,
				_xtraMessageBox.Object);
		}

		[Test]
		public void SendStatementEmails_WhenCalled_ShouldGenerateStatements()
		{
			//Act
			_service.SendStatementEmails(_statementDate);

			//Assert
			_statementGenerator.Verify(sg => sg.SaveStatement(_houseKeeper.Oid, _houseKeeper.FullName, _statementDate));
		}

		[Test]
		[TestCase(null)]
		[TestCase("")]
		[TestCase(" ")]
		public void SendStatementEmails_HouseKeeperEMailNullOrEmpty_ShouldNotGenerateStatements(string email)
		{
			//Arrange
			_houseKeeper.Email = email;

			//Act
			_service.SendStatementEmails(_statementDate);

			//Assert
			_statementGenerator.Verify(sg => sg.SaveStatement(_houseKeeper.Oid, _houseKeeper.FullName, _statementDate), Times.Never);
		}

		[Test]
		public void SendStatementEmails_WhenCalled_EmailTheStatement()
		{
			//Act
			_service.SendStatementEmails(_statementDate);

			//Assert
			VerifyEmailSent();
		}

		[Test]
		[TestCase(null)]
		[TestCase("")]
		[TestCase(" ")]
		public void SendStatementEmails_FilenameEmptyWhiteOrNull_ShouldNotEmailTheStatement(string filename)
		{
			//Arrange
			_statementFileName = filename;

			//Act
			_service.SendStatementEmails(_statementDate);

			//Assert
			VerifyEmailNotSent();
		}

		[Test]
		public void SendStatementEmails_EmailSendingFails_DisplaysMessageBox()
		{
			_emailSender.Setup(es => es.EmailFile(
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>()
				)).Throws<Exception>();

			//Act
			_service.SendStatementEmails(_statementDate);

			//Assert
			_xtraMessageBox.Verify(xm => xm.Show(It.IsAny<string>(), It.IsAny<string>(), MessageBoxButtons.OK));
		}

		private void VerifyEmailSent()
		{
			_emailSender.Verify(es => es.EmailFile(
				_houseKeeper.Email,
				_houseKeeper.StatementEmailBody,
				_statementFileName,
				It.IsAny<string>()));
		}

		private void VerifyEmailNotSent()
		{
			_emailSender.Verify(es => es.EmailFile(
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>()), Times.Never);
		}
	}
}
