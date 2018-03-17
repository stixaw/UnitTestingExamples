using NUnit.Framework;
using Moq;
using TestNinja.Mocking;
using System.Linq;
using System.Collections.Generic;
using System;

namespace TestNinja.UnitTests.Mocking
{
	[TestFixture]
	public class BookingHelperTest_OverlappingBookingsExit
	{
		private Booking _existingBooking;
		private Mock<IBookingRepository> _repository;

		[SetUp]
		public void SetUp()
		{
			_existingBooking = new Booking
			{
				Id = 2,
				ArrivalDate = ArriveOn(2017, 1, 15),
				DepartureDate = DepartOn(2017, 1, 20),
				Reference = "a"
			};
			_repository = new Mock<IBookingRepository>();
			_repository.Setup(r => r.GetActiveBookings(1)).Returns(new List<Booking>
			{
				_existingBooking
			}.AsQueryable());
		}

		[Test]
		public void BookingStartsAndFinishesBeforeAnExistingBooking_ReturnEmptyString()
		{

			//Act
			var result = BookingHelper.OverlappingBookingsExist(new Booking
			{
				Id = 1,
				ArrivalDate = Before(_existingBooking.ArrivalDate, days: 2),
				DepartureDate = Before(_existingBooking.ArrivalDate)
			}, _repository.Object);

			//Act
			Assert.That(result, Is.Empty);

		}

		[Test]
		public void BookingStartsBeforeAndFinishesInTheMiddleOfAnExistingBooking_ReturnExistingBookingsReference()
		{

			//Act
			var result = BookingHelper.OverlappingBookingsExist(new Booking
			{
				Id = 1,
				ArrivalDate = Before(_existingBooking.ArrivalDate),
				DepartureDate = After(_existingBooking.ArrivalDate)
			}, _repository.Object);

			//Act
			Assert.That(result, Is.EqualTo(_existingBooking.Reference));

		}

		[Test]
		public void BookingStartsBeforeAndFinishesAfterAnExistingBooking_ReturnExistingBookingsReference()
		{

			//Act
			var result = BookingHelper.OverlappingBookingsExist(new Booking
			{
				Id = 1,
				ArrivalDate = Before(_existingBooking.ArrivalDate),
				DepartureDate = After(_existingBooking.DepartureDate)
			}, _repository.Object);

			//Act
			Assert.That(result, Is.EqualTo(_existingBooking.Reference));

		}

		[Test]
		public void BookingStartsAndFinishesInTheMiddleOfAnExistingBooking_ReturnExistingBookingsReference()
		{

			//Act
			var result = BookingHelper.OverlappingBookingsExist(new Booking
			{
				Id = 1,
				ArrivalDate = After(_existingBooking.ArrivalDate),
				DepartureDate = Before(_existingBooking.DepartureDate)
			}, _repository.Object);

			//Act
			Assert.That(result, Is.EqualTo(_existingBooking.Reference));

		}

		[Test]
		public void BookingStartsInTheMiddleOfAnExistingBookingAndFinishesAfter_ReturnExistingBookingsReference()
		{
			//Act
			var result = BookingHelper.OverlappingBookingsExist(new Booking
			{
				Id = 1,
				ArrivalDate = After(_existingBooking.ArrivalDate),
				DepartureDate = After(_existingBooking.DepartureDate)
			}, _repository.Object);

			//Act
			Assert.That(result, Is.EqualTo(_existingBooking.Reference));

		}

		[Test]
		public void BookingStartsAndFinishesAfterAnExistingBooking_ReturnEmptyString()
		{
			//Act
			var result = BookingHelper.OverlappingBookingsExist(new Booking
			{
				Id = 1,
				ArrivalDate = After(_existingBooking.DepartureDate),
				DepartureDate = After(_existingBooking.DepartureDate, 2)
			}, _repository.Object);

			//Act
			Assert.That(result, Is.Empty);

		}

		[Test]
		public void BookingsOverlapButNewBookingIsCancelled_ReturnEmptyString()
		{
			//Act
			var result = BookingHelper.OverlappingBookingsExist(new Booking
			{
				Id = 1,
				ArrivalDate = After(_existingBooking.ArrivalDate),
				DepartureDate = After(_existingBooking.DepartureDate),
				Status = "Cancelled"
			}, _repository.Object);

			//Act
			Assert.That(result, Is.Empty);

		}



		private DateTime Before(DateTime dateTime, int days = 1)
		{
			var result = dateTime.AddDays(-days);
			return result;
		}

		private DateTime After(DateTime dateTime, int days = 1)
		{
			var result = dateTime.AddDays(+days);
			return result;
		}

		private DateTime ArriveOn(int year, int month, int day)
		{
			var result = new DateTime(year, month, day, 14, 0, 0);
			return result;
		}

		private DateTime DepartOn(int year, int month, int day)
		{
			var result = new DateTime(year, month, day, 10, 0, 0);
			return result;
		}
	}
}
