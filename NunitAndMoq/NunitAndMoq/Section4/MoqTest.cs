using Moq;
using Moq.Protected;
using NUnit.Framework;
using NunitAndMoq.Section1;
using NunitAndMoq.Section2;
using NunitAndMoq.Section3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NunitAndMoq.Section4
{
    public interface IFoo
    {
        bool DoSomething(string value);
        string ProcessString(string value);
        bool add(int x);
        int Counting();
        string Name { get; set; }
        int SomeOtherProperty { get; set; }
    }

    [TestFixture]
    public class MoqTest
    {
        [Test]
        public void MoqMethods()
        {
            var mock = new Mock<IFoo>();
            mock.Setup(foo => foo.DoSomething("TITAN")).Returns(true);
            mock.Setup(foo => foo.DoSomething("SOFT")).Returns(false);
            mock.Setup(foo => foo.DoSomething(It.IsIn("titan", "soft"))).Returns(false);

            mock.Setup(foo => foo.ProcessString(It.IsAny<string>())).Returns("good");
            mock.Setup(foo => foo.add(It.Is<int>(x => x % 2 == 0))).Returns(true);
            mock.Setup(foo => foo.ProcessString(It.IsRegex("[a-z]+"))).Returns("regex");
            Assert.Multiple(() =>
            {
                Assert.IsTrue(mock.Object.DoSomething("TITAN"));
                Assert.IsFalse(mock.Object.DoSomething("SOFT"));
                Assert.IsFalse(mock.Object.DoSomething("titan"));
                Assert.IsFalse(mock.Object.DoSomething("soft"));
                Assert.That(mock.Object.ProcessString("321"), Is.EqualTo("good"));
                Assert.IsTrue(mock.Object.add(10));
                Assert.IsFalse(mock.Object.add(11));
                Assert.That(mock.Object.ProcessString("abc"), Is.EqualTo("regex"));
            });
        }

        [Test]
        public void MoqMethodsMoreControls()
        {
            var mock = new Mock<IFoo>();
            mock.Setup(foo => foo.ProcessString(It.IsAny<string>())).Returns((string s) => s.ToLower());
            
            var calls = 0;
            mock.Setup(foo => foo.Counting())
                .Returns(() => calls)
                .Callback(() => calls++);

            mock.Object.Counting();
            mock.Object.Counting();

            Assert.Multiple(() =>
            {
                Assert.That(mock.Object.ProcessString("ABC"), Is.EqualTo("abc"));
                Assert.That(mock.Object.Counting(), Is.EqualTo(2));
                Assert.That(calls, Is.EqualTo(3));
            });
        }

        [Test]
        public void MoqMethodsException()
        {
            var mock = new Mock<IFoo>();
            mock.Setup(foo => foo.DoSomething(null)).Throws(new ArgumentException());

            Assert.Throws<ArgumentException>(() =>
            {
                mock.Object.DoSomething(null);
            });
        }

        [Test]
        public void MoqExercise()
        {
            var mock = new Mock<IMakeMoney>();
            var count = 0;
            mock.Setup(m => m.MakeMoney()).Returns(100).Callback(()=>count++);
            var player = new Player(mock.Object) { Cash = 10 }; 
            player.Cash += player.MakingMoney(); 
            player.Cash += player.MakingMoney();
            Assert.That(player.Cash, Is.EqualTo(210));
            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public void ValueTracking()
        {
            var mock = new Mock<IFoo>();
            //mock.SetupProperty(f => f.Name); // we can now manimulate Name
            mock.SetupAllProperties();
            IFoo foo = mock.Object;
            foo.Name = "abc";
            Assert.That(mock.Object.Name, Is.EqualTo("abc"));
            foo.SomeOtherProperty = 123;
            Assert.That(mock.Object.SomeOtherProperty, Is.EqualTo(123));
        }

        [Test]
        public void MyProperties()
        {
            var mock = new Mock<IFoo>();
            bool called = false;
            string name = string.Empty;
            mock.SetupSet(foo =>
            {
                foo.Name = It.IsAny<string>();
            }).Callback<string>((value) => {
                called = true; name = value;
            });
            
            mock.Object.Name = "abc";
            mock.SetupGet(foo => foo.Name).Returns(name);
            Assert.AreEqual("abc", mock.Object.Name);
            Assert.IsTrue(called);

            mock.VerifyGet(foo => foo.Name);
            mock.VerifySet(foo => foo.Name = It.IsRegex("[a-z]+"), Times.AtLeastOnce);
        }

        [Test]
        public void Verification()
        {
            var mock = new Mock<IMakeMoney>();
            var player = new Player(mock.Object);
            player.Cash += player.MakingMoney();
            player.Cash += player.MakingMoney();
            mock.Verify(m => m.MakeMoney(), Times.AtLeastOnce);
        }

        [Test]
        public void MockProtected()
        {
            var mock = new Mock<Player>(MockBehavior.Strict, new FakeMoney()) { CallBase = true };
            mock.Protected().Setup<int>("GetRandomNumber").Returns(1);

            Assert.IsTrue(mock.Object.ThrowRandomGesture() is Paper);
        }
    }
}
