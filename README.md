# NUnit and Moq

A _unit_ is the smallest testable chunk of code.
 
## Section 1 Introduction  

TDD: 
1.  Write a test
2.  Run the test to see it fail
3.  Write the code
4.  Run tests again -> Pass
5.  Refactor 

--> Red-Green-Refactor

### NUnit Test Runner

[NUnit Website](http://nunit.org/download/)  
[NUnit Github](https://github.com/nunit)  
[NUnit GUI](https://github.com/NUnitSoftware/nunit-gui/releases)  
[NUnit Console](https://github.com/nunit/nunit-console/releases/tag/3.8)

### Exercise1

Use NUnit and the TDD cycle to write a test for RockPaperScissors.

Write three classes which are **Rock**, **Paper**, and **Scissors**. Each of them has a method called **Beats**. It returns:

* 1  If it wins
* -1 If it loses
* 0  If it's a tie

```csharp
[Test]
public void TestPaperBeatsRock()
{
    Assert.IsTrue(paper.Beats(rock) > 0);
    Assert.IsFalse(rock.Beats(paper) > 0);
}
```

After making these three classes, create an abstract class named **Gesture** with this function prototype:
```cs
public int Beats(Gesture gesture);
```

This returns -2 gesture is null or some other invalid input.

[Exercise1 Solution](./Exercise1.cs)

## Section 2 Assertions

1. Basic Assertions
```csharp
Assert.That(2+2, Is.EqualTo(5));
Assert.Fail("Fail");
Assert.Inconclusive("Inconclusive");
```
2. Warnings
```csharp
Assert.Warn("This is a warn");
Warn.If(2 + 2 != 5);
Warn.If(2 + 2, Is.Not.EqualTo(5));
Warn.If(() => 2 + 2, Is.Not.EqualTo(5).After(2000));
Warn.Unless(2 + 2 != 5);
Warn.Unless(2 + 2, Is.EqualTo(5));
Warn.Unless(() => 2 + 2, Is.EqualTo(5).After(2000));
```
3. Arrange Act Assert
* AAA
```csharp
 [TestFixture]
public class BankAccountTests
{
    private BankAccount ba;
    [SetUp]
    public void SetUp()
    {
        ba = new BankAccount(100);
    }

    [Test]
    public void BankAccountShouldIncreaseOnPositiveDeposit()
    {
        ba.Deposit(100);
        Assert.That(ba.Balance, Is.EqualTo(200));
    }
}
```
4. Multiple Assertions
```csharp
[Test]
public void MultipleAsserts()
{
    ba.Withdraw(100);

    Assert.Multiple(() =>
    {
        Assert.That(ba.Balance, Is.EqualTo(0));
        Assert.That(ba.Balance, Is.LessThan(1));
    });
}
```
5. Exceptions
```csharp
[Test]
public void BankAccountShouldThrowOnNonPositiveAmount()
{
    var ex = Assert.Throws<ArgumentException>(() =>
    {
        ba.Deposit(-1);
    });

    StringAssert.StartsWith("Deposit amount must be positive", ex.Message);
}
```

### Exercises 2

1. In your previous exercise, you made the **Beats** method return -2 if it does not exist. Add warnings to test cases needed. Write a test case which deliberately invokes the warning. <br>
After adding warnings, add multiple assertions for each assertion, deliberately make a test fail to see if there are two error messages.


2. Create a **Player** class and **PlayerTest**
The player class has the method **ThrowsGesture(string gestureName)**
```csharp
public Gesture ThrowGesture(string gestureName)
```

This method returns the corresponding object. The mapping is:
```
"Rock" -> new Rock()
"Paper" -> new Paper()
"Scissors" -> new Scissors()
```
It throws an exception if the input is not valid. Make it throw an exception with a message and do an Assertion. Write a setup method which creates an instance of Player.

[Exercise2 Solution](./Exercise2.cs)
___
6. Data-Driven Testing
```csharp
private BankAccount ba;

[SetUp]
public void SetUp()
{
    ba = new BankAccount(100);
}

[Test]
[TestCase(50, true, 50)]
[TestCase(100, true, 0)]
[TestCase(1000, false, 100)]
public void TestMultipleWithdrawalScenarios(int amountToWithdraw, bool shouldSuccedd, int expectedBalance)
{
    var result = ba.Withdraw(amountToWithdraw);
    //Warn.If(!result, "Failed for some reason");
    Assert.Multiple(() =>
    {
        Assert.That(result, Is.EqualTo(shouldSuccedd));
        Assert.That(expectedBalance, Is.EqualTo(ba.Balance));
    });
}
```

However, if you pass a non-primitive types, you most likely will get the error message:  ``an attribute type must be a constant type``

To see more: [MSDN](https://docs.microsoft.com/en-us/dotnet/visual-basic/misc/bc30045)

For example, **decimal** is a type which does not work. If we change our _TestMultipleWithdrawalScenarios_ **int32** parameter types into **decimal** types, then we will encounter the same error.

To overcome this issue:
```csharp
private static object[] MyCaseSource =
{
    new object[] { 50.0m,true,50m },
    new object[] { 100m, true, 0m},
    new object[] { 1000m, false, 100m }
};

[Test]
[TestCaseSource("MyCaseSource")]
public void TestMultipleWithdrawalScenariosCaseSource(decimal amountToWithdraw, bool shouldSuccedd, decimal expectedBalance)
{
    var result = ba.Withdraw((int)amountToWithdraw);
    //Warn.If(!result, "Failed for some reason");
    Assert.Multiple(() =>
    {
        Assert.That(result, Is.EqualTo(shouldSuccedd));
        Assert.That((int)expectedBalance, Is.EqualTo(ba.Balance));
    });
}
```

### Exercise 3
1. In your **Player** class, give it an _int32_ property **Cash** and a method **Withdraw** like so:

```csharp
public class Player
{
    public int Cash { get; set; }
    public void Withdraw(int amount);
}
```

Use the _Data-Driven Testing_ mechanism to test the **Withdraw** method

2. In your first exercise, you wrote many test cases for **Gesture** which indicates who beats who. Use _Data-Driven Testing_ to re-implement those test cases.

[Exercise3 Solution](./Exercise3.cs)
## Section 3 Test Doubles
Most likely you have heard the terms **Fake**, **Stubs**, and **Mock** more or less.

Two classes interact which makes it hard to test. We need a _test double_ for each object. 
* Fake: Null Object Pattern
* Stub: Like a fake, but returns the answer you want
* Mock: A fake object where you can set expectations

Reference Reading: [Mocks are not stubs](https://martinfowler.com/articles/mocksArentStubs.html)

```csharp
public interface ILog
{
    bool Write(string msg);
}

public class BankAccount
{
    public int Balance { get; set; }
    private readonly ILog log;

    public BankAccount(ILog log)
    {
        this.log = log;
    }

    public void Deposit(int amount)
    {
        if(log.Write($"Depositing {amount}"))
            Balance += amount;
    }
}
```

To write a fake class:
```csharp
public class NullLog : ILog
{
    public bool Write(string msg)
    {
        return true;
    }
}

[Test]
public void DepositUnitTestWithFake()
{
    var log = new NullLog();
    ba = new BankAccount(log) { Balance = 100 };
    ba.Deposit(100);
    Assert.That(ba.Balance, Is.EqualTo(200));
}
```

With stubs:
```csharp
public class NullLogWithResult : ILog
{
    private bool expectedResult;

    public NullLogWithResult(bool expectedResult)
    {
        this.expectedResult = expectedResult;
    }
    public bool Write(string msg)
    {
        return expectedResult;
    }
}

[Test]
public void DepositUnitTestWithStubs()
{
    var log = new NullLogWithResult(true);
    ba = new BankAccount(log) { Balance = 100 };
    ba.Deposit(100);
    Assert.That(ba.Balance, Is.EqualTo(200));
}
```

With Mock:
```csharp
public class LogMock : ILog
{
    private bool expectedResult;
    public Dictionary<string, int> MethodCallCount;

    public LogMock(bool expectedResult)
    {
        this.expectedResult = expectedResult;
        MethodCallCount = new Dictionary<string, int>();
    }

    private void AddOrIncrement(string methodName)
    {
        if (MethodCallCount.ContainsKey(methodName))
        {
            MethodCallCount[methodName]++;
        }
        else
        {
            MethodCallCount.Add(methodName, 1);
        }
    }

    public bool Write(string msg)
    {
        AddOrIncrement(nameof(Write));
        return expectedResult;
    }
}

[Test]
public void DepositUnitTestWithMocks()
{
    var log = new LogMock(true);
    ba = new BankAccount(log) { Balance = 100 };
    ba.Deposit(100);
    Assert.Multiple(() =>
    {
        Assert.That(ba.Balance, Is.EqualTo(200));
        Assert.That(log.MethodCallCount[nameof(LogMock.Write)], Is.EqualTo(1));
    });
}
```

#### Exercise 4
1. Write an interface named **IMakeMoney**
```cs
public interface IMakeMoney
{
    int MakeMoney();
}
```

Make our **Player class** implement **IMakeMoney**
```cs
public class Player
{
    public int Cash { get; set; }

    private IMakeMoney MakeMoney;

    public Player(IMakeMoney makeMoney)
    {
        this.MakeMoney = makeMoney;
    }

    public int MakingMoney()
    {
        return MakeMoney.MakeMoney();
    }
}
```

Try to create Fake, Stubs, and Mock objects for **IMakeMoney**

[Exercise4 Solution](./Exercise4.cs)
## Section 4 Moq
* Methods
* Properties
* Verifications
* Protected members

We can first try to rewrite the unit test using **Moq**
```csharp

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
```

#### Mocking methods
A powerful scheme that **Moq** provides us is to set up what it returns.

Simple setups we can do using **Moq**:
```csharp
public interface IFoo
{
    bool DoSomething(string value);
    string ProcessString(string value);
    bool add(int x);
    int Counting();
}

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
```

Of course, sometimes we need more controls such as callbacks:
```cs
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
```

We also have some cases which we want to check to see if it throws exceptions:
```cs
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
```



Now let's look at how to mock properties.
```cs
public interface IFoo
{
    bool DoSomething(string value);
    string ProcessString(string value);
    bool add(int x);
    int Counting();
    string Name{get;set;}
    int SomeOtherProperty { get; set; }
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
```

However if we want to write our own setter
```cs
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
    
    mock.Object.Name = "Hello";
    mock.SetupGet(foo => foo.Name).Returns(name);
    Assert.AreEqual("Hello", mock.Object.Name);
    Assert.IsTrue(called);
    mock.VerifyGet(foo => foo.Name);
    mock.VerifySet(foo => foo.Name = It.IsRegex("[a-z]+"), Times.AtLeastOnce);
}
```

Our last topic is about mocking protected members. To do this, you have to use the **Moq.protected** namespace.

```csharp
var mock = new Mock<Player>(MockBehavior.Strict, new FakeMoney()) { CallBase = true };
mock.Protected().Setup<int>("GetRandomNumber").Returns(1);
```
#### Exercises 5
1. For your Player class, try to use **Moq** to control how much money he makes. Also, use a local variable to keep a count of how many times **makeMoney** is called.

2. Mock Player's Cash to whatever you want. Also verify that **MakeMoney()** is called at least once.

3. Create a method called **ThrowRandomGesture()**. The way it generates random gesture depends on a private method called **GetRandomNumber()**. Try to mock  **GetRandomNumber()** so **ThrowRandomGesture()** always returns what you want.

```cs
private int GetRandomNumber()
{
    return new Random().Next(0, 2);
}

public Gesture ThrowRandomGesture()
{
    var random = GetRandomNumber();
    var gestures = new Gesture[] { new Rock(), new Paper(), new Scissors() };
    return gestures[random];
}
```
[Exercise5 Solution](./Exercise5.cs)