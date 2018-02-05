using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Moq
{
    public interface ILog
    {
        bool Write(string msg);
    }

    public interface IFoo
    {
        bool DoSomething(string value);
        string ProcessString(string value);
        bool TryParse(string value, out string outputValue);
        bool Submit(ref Bar bar);
        int GetCount();
        bool Add(int amount);

        string Name { get; set; }
        IBaz SomeBaz { get; }
        int SomeOtherProperty { get; set; }
    }

    public interface IBaz
    {
        string Name { get; set; }
    }

    public class Bar : IEquatable<Bar>
    {
        public string Name { get; set; }

        public bool Equals(Bar other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Bar)obj);
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }

        public static bool operator == (Bar left, Bar right)
        {
            return Equals(left, right);
        }

        public static bool operator != (Bar left, Bar right)
        {
            return !Equals(left, right);
        }
    }

    public class BankAccount
    {
        public int Balance { get; set; }
        private ILog log;

        public BankAccount(ILog log)
        {
            this.log = log;
        }

        public void Deposit(int amount)
        {
            log.Write($"User has withdrawn {amount}");
            Balance += amount;
        }
    }

    [TestFixture]
    public class MethodSamples
    {
        [Test]
        public void ArgumentDependentMatching()
        {
            var mock = new Mock<IFoo>();

            mock.Setup(foo => foo.DoSomething(It.IsAny<string>())).Returns(false);
            mock.Setup(foo => foo.Add(It.Is<int>(x => x % 2 == 0))).Returns(true);

            mock.Setup(foo => foo.Add(It.IsInRange<int>(1, 10, Range.Inclusive))).Returns(false);

            mock.Setup(foo => foo.DoSomething(It.IsRegex("[a-z]+"))).Returns(false);

            mock.Object.DoSomething("abc"); // returns false
        }

        [Test]
        public void OrdinaryMethodCalls()
        {
            var mock = new Mock<IFoo>();
            mock.Setup(foo =>
                foo.DoSomething("ping")
            ).Returns(true);

            //mock.Setup(foo =>
            //    foo.DoSomething("pong")
            //).Returns(false);

            mock.Setup(foo => foo.DoSomething(It.IsIn("pong", "foo"))).Returns(false);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(mock.Object.DoSomething("ping"));
                Assert.IsFalse(mock.Object.DoSomething("pong"));
                Assert.IsFalse(mock.Object.DoSomething("foo"));
            });
        }

        [Test]
        public void OutAndRefArguments()
        {
            var mock = new Mock<IFoo>();
            var requiredOutput = "ok";
            mock.Setup(foo => foo.TryParse("ping", out requiredOutput)).Returns(true);
            string result;
            Assert.Multiple(() =>
            {
                Assert.IsTrue(mock.Object.TryParse("ping", out result));
                Assert.That(result, Is.EqualTo(requiredOutput));

                var thisShouldBeFalse = mock.Object.TryParse("pong", out result);
                Console.WriteLine(thisShouldBeFalse);
                Console.WriteLine(result);
            });

            var bar = new Bar() { Name = "abc" };
            mock.Setup(foo => foo.Submit(ref bar)).Returns(true);
            Assert.That(mock.Object.Submit(ref bar), Is.EqualTo(true));

            var someOtherBar = new Bar() { Name = "abc" };
            Assert.IsFalse(mock.Object.Submit(ref someOtherBar)); // moq compares references
        }

        [Test]
        public void MyTest()
        {
            var mock = new Mock<IFoo>();
            mock.Setup(foo => foo.ProcessString(It.IsAny<string>())).Returns((string s) => s.ToLowerInvariant());

            var calls = 0;
            mock.Setup(foo => foo.GetCount()).Returns(() => calls)
                .Callback(() => ++calls);

            mock.Object.GetCount();
            mock.Object.GetCount();

            Assert.Multiple(() =>
            {
                Assert.That(mock.Object.ProcessString("ABC"), Is.EqualTo("abc"));
                Assert.That(mock.Object.GetCount(), Is.EqualTo(2));
            });
        }

        [Test]
        public void MyException()
        {
            var mock = new Mock<IFoo>();
            //mock.Setup(foo => foo.DoSomething("kill")).Throws<InvalidOperationException>();
            //Assert.Throws<InvalidOperationException>(() => mock.Object.DoSomething("kill"));
            mock.Setup(foo => foo.DoSomething(null)).Throws(new ArgumentException("cmd"));

            Assert.Throws<ArgumentException>(() =>
            {
                mock.Object.DoSomething(null);
            }, "cmd");
        }

        [Test]
        public void MyProperties()
        {
            var mock = new Mock<IFoo>();
            mock.Setup(foo => foo.Name).Returns("bar");

            mock.Object.Name = "will not be assigned"; //This will not be assigned. The setter doesn't exist
            Assert.That(mock.Object.Name, Is.EqualTo("bar"));
            mock.Setup(foo => foo.SomeBaz.Name).Returns("hello");
            Assert.That(mock.Object.SomeBaz.Name, Is.EqualTo("hello"));

            bool setterCalled = false;
            mock.SetupSet(foo =>
            {
                foo.Name = It.IsAny<string>();
            }).Callback<string>((value) => setterCalled = true);

            mock.Object.Name = "def"; // now the setter exists
            mock.VerifySet(foo =>
            {
                foo.Name = "def";
            }, Times.AtLeastOnce); //assert if it is called once
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
        public void MockingEvents()
        {
            var mock = new Mock<IAnimal>();
            var doctor = new Doctor(mock.Object);

            mock.Raise(a => a.FallsIll += null, new EventArgs());

            Assert.That(doctor.TimesCured, Is.EqualTo(1));

            mock.Setup(a => a.Stumble()).Raises(a => a.FallsIll += null, new EventArgs());
            mock.Object.Stumble();
            Assert.That(doctor.TimesCured, Is.EqualTo(2));

            mock.Raise(a => a.AbductedByAliens += null, 42, true);
            Assert.That(doctor.AbductionsObserved, Is.EqualTo(1));
        }

        [Test]
        public void Callbacks()
        {
            var mock = new Mock<IFoo>();

            int x = 0;
            mock.Setup(foo => foo.DoSomething("ping"))
                .Returns(true)
                .Callback(() => x++);
            mock.Object.DoSomething("ping");
            Assert.That(x, Is.EqualTo(1));

            mock.Setup(foo => foo.DoSomething(It.IsAny<string>()))
                .Returns(true)
                .Callback((string s) => x += s.Length);

            //mock.Setup(foo => foo.DoSomething(It.IsAny<string>()))
            //    .Returns(true)
            //    .Callback<string>((s) => x += s.Length);

            mock.Setup(foo => foo.DoSomething("pong"))
                .Callback(() => Console.WriteLine("before pong"))
                .Returns(false)
                .Callback(() => Console.WriteLine("after pong"));
            mock.Object.DoSomething("pong");
        }

        [Test]
        public void Verification()
        {
            var mock = new Mock<IFoo>();
            var consumer = new Consumer(mock.Object);

            consumer.Hello();
            mock.Verify(foo => foo.DoSomething("ping"), Times.AtLeastOnce);

            mock.Verify(foo => foo.DoSomething("pong"), Times.Never);

            mock.VerifyGet(foo => foo.Name);

            mock.VerifySet(foo => foo.SomeOtherProperty = It.IsInRange(100, 200, Range.Inclusive));
        }

        [Test]
        public void BehaviorCustomization()
        {
            var mock = new Mock<IFoo>(MockBehavior.Strict);
            mock.Object.DoSomething("abc"); //this is gonna throw an exception because it is strict

            mock.Setup(f => f.DoSomething("abc")).Returns(true);
            mock.Object.DoSomething("abc");
        }

        [Test]
        public void BehaviorCustomization1()
        {
            var mock = new Mock<IFoo>()
            {
                DefaultValue = DefaultValue.Mock //assigns IBaz default value
            };

            var baz = mock.Object.SomeBaz;
            var bazMock = Mock.Get(baz);
            bazMock.SetupGet(f => f.Name).Returns("abc");

            var mockRepository = new MockRepository(MockBehavior.Strict)
            {
                DefaultValue = DefaultValue.Mock
            };

            var fooMock = mockRepository.Create<IFoo>();
            var otherMock = mockRepository.Create<IBaz>(MockBehavior.Loose);
            mockRepository.Verify();
        }

        [Test]
        public void ProtectedMembers()
        {
            var mock = new Mock<Person>();
            mock.Protected().SetupGet<int>("SSN").Returns(42);
            //mock.Protected().Setup<string>("Execute", ItExpr.IsAny<string>());
            Assert.AreEqual(42,mock.Object.GetSSN());
        }
    }

    public abstract class Person
    {
        protected virtual int SSN { get; set; }
        protected abstract void Execute(string cmd);
        public int GetSSN() { return SSN; }
    }

    public class Consumer
    {
        private IFoo foo;

        public Consumer(IFoo foo)
        {
            this.foo = foo;
        }

        public void Hello()
        {
            foo.DoSomething("ping");
            var name = foo.Name;
            foo.SomeOtherProperty = 123;
        }
    }

    public delegate void AlientAbductionEventHandler(int galaxy, bool returned);

    public interface IAnimal
    {
        event EventHandler FallsIll;
        void Stumble();
        event AlientAbductionEventHandler AbductedByAliens;
    }

    public class Doctor
    {
        public int TimesCured;
        public int AbductionsObserved;

        public Doctor(IAnimal animal)
        {
            animal.FallsIll += (sender, args) =>
            {
                Console.WriteLine("I will cure you!");
                TimesCured++;
            };

            animal.AbductedByAliens += (galaxy, returned) => ++AbductionsObserved;
        }
    }

    [TestFixture]
    public class BankAccountTests
    {
        private BankAccount ba;

        [Test]
        public void DepositTest()
        {
            var log = new Mock<ILog>();
            ba = new BankAccount(log.Object) { Balance = 100 };
            ba.Deposit(100);
            Assert.That(ba.Balance, Is.EqualTo(200));
        }
    }
}
