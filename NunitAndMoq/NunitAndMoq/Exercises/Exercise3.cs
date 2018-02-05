using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NunitAndMoq.Exercise3
{
    [TestFixture]
    public class Exercise3
    {
        [Test]
        public void Exercise3Test()
        {
            Gesture paper = new Paper();
            Gesture rock = new Rock();
            Assert.Multiple(() =>
            {
                Assert.IsTrue(paper.Beats(rock) > 0);
                Assert.IsFalse(rock.Beats(paper) > 0);
            });
        }

        [Test]
        public void Exercise3Test1()
        {
            Gesture paper = new Paper();
            Gesture paper1 = new Paper();
            Assert.That(paper.Beats(paper1), Is.EqualTo(0));
        }

        [Test]
        public void Exercise3Test2()
        {
            Gesture paper = new Paper();
            Assert.AreEqual(paper.Beats(null),-2);
        }

        [Test]
        public void Exercise3Test3()
        {
            Gesture paper = new Paper();
            Gesture gesture = null;
            Warn.If(paper.Beats(gesture) == -2);
        }

        private Player _flash;

        [SetUp]
        public void Setup()
        {
            _flash = new Player();
        }

        [Test]
        public void Exercise3Test4()
        {
            Assert.Throws<FlashException>(() =>
            {
                _flash.ThrowGesture(null);
            });
        }

        [Test]
        public void Exercise3Test5()
        {
            var ex = Assert.Throws<KeyNotFoundException>(() =>
            {
                _flash.ThrowGesture("YA");
            });
            StringAssert.StartsWith("This gesture is not implemented", ex.Message);
        }

        private Player _phillip = new Player();

        [Test]
        [TestCase(100, 100, 0)]
        [TestCase(100, 101, 100)]
        public void Exercise3Test6(int initialAmount, int withdrawalAmount, int result)
        {
            _phillip.Cash = initialAmount;
            _phillip.Withdraw(withdrawalAmount);
            Assert.That(_phillip.Cash, Is.EqualTo(result));
        }

        private static object[] MyCaseSource =
        {
            new object[] { new Rock(), new Paper(), -1 },
            new object[] { new Rock(), new Rock(), 0 },
            new object[] { new Rock(), new Scissors(), 1 },
        };

        [Test]
        [TestCaseSource("MyCaseSource")]
        public void Exercise3Test7(Gesture gesture1, Gesture gesture2, int result)
        {
            Assert.That(gesture1.Beats(gesture2), Is.EqualTo(result));
        }
    }

    public class FlashException : Exception { }

    public class Player
    {
        public int Cash { get; set; }
        public void Withdraw(int amount)
        {
            if (amount <= Cash)
                Cash -= amount;
        }

        public Gesture ThrowGesture(string gestureName)
        {
            if (string.IsNullOrEmpty(gestureName))
                throw new FlashException();
            var gestureDict = new Dictionary<string, Gesture>
                {
                    { "Rock", new Rock()},
                    { "Paper", new Paper()},
                    { "Scissors", new Scissors()}
                };

            Gesture gesture;
            try
            {
                gesture = gestureDict[gestureName];
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException("This gesture is not implemented!!!");
            }

            return gesture;
        }
    }

    public abstract class Gesture
    {
        public int Beats(Gesture gesture)
        {
            if (gesture is Rock) return Beats(gesture as Rock);
            if (gesture is Paper) return Beats(gesture as Paper);
            if (gesture is Scissors) return Beats(gesture as Scissors);
            return -2;
        }
        abstract protected int Beats(Scissors scissors);
        abstract protected int Beats(Rock rock);
        abstract protected int Beats(Paper paper);
    }


    public class Rock: Gesture
    {
        protected override int Beats(Scissors scissors) => 1;
        protected override int Beats(Rock rock) => 0;
        protected override int Beats(Paper paper) => -1;
    }

    public class Paper : Gesture
    {
        protected override int Beats(Scissors scissors) => -1;
        protected override int Beats(Rock rock) => 1;
        protected override int Beats(Paper paper) => 0;
    }

    public class Scissors : Gesture
    {
        protected override int Beats(Scissors scissors) => 0;
        protected override int Beats(Rock rock) => -1;
        protected override int Beats(Paper paper) => 1;
    }
}
