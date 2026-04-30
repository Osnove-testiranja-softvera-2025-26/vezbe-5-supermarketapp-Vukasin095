using NUnit.Framework;
using OTS_Supermarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace OTS_Supermarket.Test
{
    [TestFixture]
    public class CartTest
    {
        [Test]
        public void AddOneToCart_ShouldAddItemToCart_Success()
        {
            //ARANGE
            Cart cart = new Cart();
            Monitor monitor = new Monitor();

            //ACT
            cart.AddOneToCart(monitor);

            //ASSERT
            Assert.That(cart.Size, Is.EqualTo(1));
            Assert.That(cart.Amount, Is.EqualTo(100));

        }

        [Test]
        public void AddMultipleToCart_ShouldAddMultipleItems_Success()
        {
            // ARRANGE
            Cart cart = new Cart();
            Monitor monitor = new Monitor();

            // ACT
            cart.AddMultipleToCart(monitor, 3);

            // ASSERT
            Assert.That(cart.Size, Is.EqualTo(3));
            Assert.That(cart.Amount, Is.EqualTo(300));
            Assert.That(cart.Monitor_counter, Is.EqualTo(3));
        }

        [Test]
        public void AddOneToCart_WhenExceedsLimit_ShouldThrowException()
        {
            // ARRANGE
            Cart cart = new Cart();
            Monitor monitor = new Monitor();

            // fill to limit
            cart.AddMultipleToCart(monitor, 10);

            // ACT / ASSERT
            Assert.Throws<Exception>(() => cart.AddOneToCart(monitor));
        }

        [Test]
        public void DeleteAll_ShouldClearCart_Success()
        {
            // ARRANGE
            Cart cart = new Cart();
            Monitor monitor = new Monitor();

            cart.AddMultipleToCart(monitor, 2);

            // ACT
            cart.DeleteAll();

            // ASSERT
            Assert.That(cart.Size, Is.EqualTo(0));
            Assert.That(cart.Monitor_counter, Is.EqualTo(0));
            Assert.That(cart.Items.Count, Is.EqualTo(0));
        }

        [Test]
        public void Print_WhenEmpty_ShouldThrowException()
        {
            // ARRANGE
            Cart cart = new Cart();

            // ACT / ASSERT
            Assert.Throws<Exception>(() => cart.Print());
        }

        [Test]
        public void Calculate_InvalidDateFormat_ShouldThrowException()
        {
            // ARRANGE
            Cart cart = new Cart();
            cart.Budget = 1000;

            // ACT / ASSERT
            Assert.Throws<Exception>(() => cart.Calculate("01-01-2025"));
        }

        [Test]
        public void Calculate_TodayDate_ShouldThrowException()
        {
            // ARRANGE
            Cart cart = new Cart();
            cart.Budget = 1000;
            string today = DateTime.Today.ToString("yyyy-MM-dd");

            // ACT / ASSERT
            Assert.Throws<Exception>(() => cart.Calculate(today));
        }

        [Test]
        public void Calculate_NotEnoughBudget_ShouldThrowException()
        {
            // ARRANGE
            Cart cart = new Cart();
            Monitor monitor = new Monitor();
            cart.AddOneToCart(monitor); // Amount = 100
            cart.Budget = 50; // less than amount

            DateTime delivery = DateTime.Today.AddDays(1);
            // ensure not today's date and within 7 days
            string date = delivery.ToString("yyyy-MM-dd");

            // ACT / ASSERT
            Assert.Throws<Exception>(() => cart.Calculate(date));
        }

        [Test]
        public void Calculate_BudgetDeductedOnSuccess()
        {
            // ARRANGE
            Cart cart = new Cart();
            Monitor monitor = new Monitor();
            cart.AddOneToCart(monitor); // Amount = 100
            cart.Budget = 500;

            // pick next weekday within 7 days
            DateTime delivery = DateTime.Today.AddDays(1);
            while (delivery.DayOfWeek == DayOfWeek.Saturday || delivery.DayOfWeek == DayOfWeek.Sunday)
            {
                delivery = delivery.AddDays(1);
            }
            string date = delivery.ToString("yyyy-MM-dd");

            double expectedPrice = cart.Amount; // no discount in this simple case

            // ACT
            cart.Calculate(date);

            // ASSERT
            Assert.That(cart.Budget, Is.EqualTo(500 - expectedPrice));
        }

        [TestCase(0, 3, 3)]
        [TestCase(8, 2, 10)]
        public void AddMultipleToCart_DataDriven(int initialSize, int quantity, int expectedSize)
        {
            // ARRANGE
            Cart cart = new Cart();
            Monitor monitor = new Monitor();
            cart.Size = initialSize;

            // ACT
            cart.AddMultipleToCart(monitor, quantity);

            // ASSERT
            Assert.That(cart.Size, Is.EqualTo(expectedSize));
        }

        [TestCase(0, 3, ExpectedResult = 300)]
        [TestCase(2, 5, ExpectedResult = 700)]
        public double AddMultipleToCart_AmountDataDrivenWithReturn(int initialSize, int quantity)
        {
            // ARRANGE
            Cart cart = new Cart();
            Monitor monitor = new Monitor();
            // initialize cart with initialSize items so Amount matches Size
            if (initialSize > 0)
            {
                cart.AddMultipleToCart(monitor, initialSize);
            }

            // ACT
            cart.AddMultipleToCart(monitor, quantity);

            // RETURN amount for ExpectedResult assertion
            return cart.Amount;
        }

        [TestCase(10)]
        [TestCase(11)]
        public void AddOneToCart_WhenStartingAtLimit_ShouldThrow(int startingSize)
        {
            // ARRANGE
            Cart cart = new Cart();
            Monitor monitor = new Monitor();
            cart.Size = startingSize;

            // ACT / ASSERT
            Assert.Throws<Exception>(() => cart.AddOneToCart(monitor));
        }


    }
}
