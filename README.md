# Xspec — Fluent, specification-style unit testing for .NET

Xspec is a fluent, specification-oriented testing framework for .NET that builds on xUnit.
It follows the Given–When–Then pattern and integrates seamlessly with Moq.
Tests run on the standard xUnit runner and can live side by side with existing xUnit tests.

Whether you are new to unit testing or an experienced practitioner, Xspec helps you express test intent clearly by removing boilerplate, enforcing structure, and generating readable failure descriptions.

Install the package in your test project:
```shell
dotnet add package Xspec
```

Example: testing the `PlaceOrder` method on `ShoppingService`:
```csharp
public class WhenPlaceOrder : Spec<ShoppingService>
{
    static Tag<Guid> cartId = new(); // reference an auto-generated Guid

    public WhenPlaceOrder()
        => When(_ => _.PlaceOrder(The(cartId)))
           .Given<ICartRepository>()
           .That(_ => _.GetCart(The(cartId)))
           .Returns(A<Cart>());

    [Fact] public void ThenCreatesOrder()
        => Then<IOrderService>(_ => _.CreateOrder(The<Cart>()));
}
```

The example above highlights how Xspec reduces boilerplate by handling test data, dependency mocking, and interaction verification declaratively.
In real-world usage, this typically leads to substantially smaller tests compared to xUnit + Moq, while improving readability.

## Table of Contents

1. [Introduction](#1-introduction)  
2. [The Test Pipeline](#2-the-test-pipeline)  
3. [Using Test Data](#3-using-test-data)  
4. [Mocking & Auto-Mocking](#4-mocking--auto-mocking)  
5. [Asserting Results](#5-asserting-results)  
6. [Guidelines](#6-guidelines) 
7. [Beyond TDD: Specification-Driven Agentic Development](#7-beyond-tdd-specification-driven-agentic-development)

## 1. Introduction

To write a test with Xspec, start by subclassing `Spec`.
Each test is expressed as a specification and executed as a pipeline consisting of three phases:
*arrange*, *act*, and *assert*.

The following is a complete Xspec test class (a *specification*) containing a single test method (a *requirement*):

```csharp
using Xspec;
using Xspec.Assert;
using static App.Calculator;

namespace App.Test;

public class CalculatorSpec : Spec<int>
{
    [Fact] public void WhenAdd_1_and_2_ThenSumIs_3() => When(_ => Add(1, 2)).Then().Result.Is(3);
}
```

### 1.1 Arrange

The *arrange* stage defines the setup of the test pipeline.
In Xspec, this is done by calling methods on `Spec`, either directly or fluently chained together.

The following methods are used to arrange a test:

* `Given`  — defines test setup, mocks, and input data.
* `Using`  — registers type conversions, defaults, and factories.
* `Having` — setup that runs *before* the action.
* `Until`  — teardown or verification that runs *after* the action.

`Having` and `Until` compose fluently in chain form, with the action at the center and each clause extending outward in time.

```csharp
public class WhenSendReport : Spec<EmailService, SendResult>
{
    public WhenSendReport()
        => When(_ => _.Send(A<Report>()))
           .Having(_ => _.SignIn(A<Credentials>()))
           .Having(_ => _.Configure(A<SmtpSettings>()))
           .Until(_ => _.FlushOutbox())
           .Until(_ => _.Disconnect());

    [Fact] public void ThenReturnsSent() => Then().Result.Is(SendResult.Sent);
}
```

The chain reads as one sentence: *having signed in, having configured SmtpSettings, when sending a Report, until the outbox is flushed, until disconnected.*

Xspec uses a strongly typed API to guide your setup, preventing most invalid test configurations at compile time.

Xspec also provides mechanisms for preparing and referring to test data in a stable way, so the same values can be consistently reused across arrangement, execution, and assertion.

### 1.2 Act

The *act* stage specifies the behavior under test by calling `When` with a lambda expression. 
The lambda takes the subject under test as an argument and should execute the behavior under test.

The subject under test is automatically created based on the arrangement, unless it is static or explicitly provided.

As with arrangement, the order in which `Given`, `Having`, `Until`, and `When`
are declared does not matter. Because execution is deferred until assertion, Xspec can deterministically reorder the pipeline before running it.
So the execution order of the steps is always: `Given` -> `Having` -> `When` -> `Until`.

Each specification defines exactly one action under test and therefore contains a single `When` stage.

### 1.3 Assert

Xspec includes a fluent assertion library, `Xspec.Assert`, conceptually similar to FluentAssertions,
but with a more compact syntax based on the verbs `Is`, `Has`, and `Does`.

The *assert* stage is specified by calling `Then` or `Result`, followed by one or more assertions.
It is only when one of these methods is called that the test pipeline is executed and the result evaluated.

If a test fails, this is either due to an invalid test setup or because an assertion was not satisfied.
In the latter case, Xspec provides detailed assertion failures together with an automatically
generated description of the specification, making it easier to understand the intended behavior.

Example:

**Specification:**
```csharp
=> When(_ => _.List())
   .Given<IMyRepository>()
   .That(_ => _.List()).Returns(A<MyModel[]>)
   .Given().Three<MyModel>()
   .Then().Result.Has().Count(4)
```

**Output:**
```csharp
Expected Result to have count 4 but found 3...
---- 
Given three MyModel
  and IMyRepository.List() returns a MyModel[]
When _.List()
Then Result has count 4
```

In addition to verifying return values, exceptions can also be asserted using `Then().Throws`.

Given some prior familiarity with NuGet, unit tests and mocking, you should now be ready to start writing your own tests using Xspec. 
For professional use, the remainder of this README serves as a complete, practical guide to structuring specifications, managing test data, and verifying behavior with Xspec.

## 2. The Test Pipeline

A core feature of Xspec is *deferred execution* and *lazy evaluation*: no production code is executed until the first assertion is made.
A test runs through four conceptual stages: preparation, execution, assertion, and teardown.

### 2.1 Preparation

Before the first assertion, the pipeline is configured with test data, mocks, and lambdas to execute.

#### 2.1.1 Creating the Pipeline
You typically create the pipeline by subclassing `Spec` with two generic arguments. The first is the type of the *subject under test* and the second is the return type of the *method under test*.
Other overloads exist for cases without a subject under test or return value.

You can choose to pass a class fixture (a feature of xUnit) to the constructor or start with an empty test pipeline.

#### 2.1.2 Scope of Arrangement
When preparing the pipeline, values are provided either for the **Subject** or for the **Input**. 
* **Subject**: The subject under test or any of its components, provided as constructor arguments, properties, or through type cast.
* **Input**: Data supplied directly to the execution pipeline.

Arrangements are categorized into three types, using specific verbs to dictate their scope:
* **Values**: Configured using the verb **`Given`** and apply *only* to the **Input**.
* **Types**: Configured using the verb **`Using`**. An optional scope indicates whether they apply to the **Input**, the **Subject**, or both (the default).
* **Mocks**: Configured using the verb **`Given`** and apply to *both* **Input** and **Subject**.

#### 2.1.3 Preparing the Pipeline
The preparation steps are recorded and later applied in the following order:

1. Default, constraints, and test data are applied *in reverse order of declaration*.
1. Mocked behavior *in the order of declaration*.

#### 2.1.4 Creating the Subject Under Test
After preparation, the pipeline will use AutoMock to create a new instance of your subject under test (unless you provided a value of that type explicitly).
If you haven't mocked a certain interface or method that the subject uses, a default mock will be auto-generated.

### 2.2 Execution

Execution is triggered by the first assertion (technically when `Result` is referenced or `Then()` is called). 
The pipeline then executes and captures the outcome of the execution.

#### 2.2.1 Running Setup
Setup steps can be provided as lambdas that take the subject under test as arguments, with `Having()`.
Setup will be executed in the reverse order of declaration, right after creating the subject under test.

Example:
`When(A).Having(B).Having(C)` will result in the execution order: C -> B -> A.

#### 2.2.2 Executing the Behavior Under Test
The lambda provided with `When()` will be executed right after setup.

#### 2.2.3 Collecting the outcome
The outcome of a pipeline execution is either a return value or a thrown exception.
If a value is returned, it must match the declared return type and is exposed for assertion through the `Result` property.
If an exception is thrown, it becomes the captured outcome and can be asserted using the `Then().Throws` overloads.

Accessing `Result` will implicitly execute the pipeline if it has not already been executed.

### 2.3 Assertion

Assertions consume the captured outcome or utilize the mocking framework for verifying execution paths. 
The pipeline executes at most once per test method, regardless of the number of references to `Result` or `Then()`.
Assertions are covered in depth in Chapters 5 and 6.

### 2.4 Teardown

Teardown steps can be provided as lambdas that take subject under test as arguments, with `Until()`.
Teardown will be executed in the order of declaration when the test class and pipeline are disposed, after the test method has been executed.

Example:
`When(A).Until(B).Until(C)` will result in the execution order: A -> B -> C.

### 2.5 Sync vs. Async Execution

Xspec supports testing synchronous and asynchronous code using the same test pipeline.

When the behavior under test is asynchronous (returns `Task` or `Task<T>`), Xspec waits for completion and captures the outcome in the same way as for synchronous code.
The only difference is the lambda signature provided to `When`, `Having`, `Until`, and mock setup methods.
Test methods themselves do not need to be `async`.

As a result, tests for async code read and behave the same way as tests for synchronous code.

## 3. Using Test Data

Xspec provides helpers for referring to test data that can either be supplied explicitly or automatically generated (optionally with constraints).

Two complementary mechanisms are provided:
- Mentions, for quickly referring to generated values by position or quantity
- Tags, for assigning stable, meaningful identities to values of the same type

### 3.1 Mentions

Mentions are helper methods for generating and referring to up to five numbered values of a given type, as well as collections of up to five elements.
Mentions are resolved per type and per test and always refer to the same value within a specification.

**Single values**
For a single generated value:
`A`, `An`, `The`, `AFirst`, `TheFirst`

For additional values of the same type:
`ASecond`, `TheSecond`
`AThird`, `TheThird`
`AFourth`, `TheFourth`
`AFifth`, `TheFifth`

**Collections**
For collections of generated values:
`Zero`, `One`, `Two`, `Three`, `Four`, `Five`
`Some` (at least one), `Many` (at least two), `AnyNumberOf`

**Unreferenced values**
For auto-generated values that are not intended to be referenced again:
`Any`, `Another`

**Uniqueness**
All values of the same or equivalent types are guaranteed to be unique within a test run.

### 3.2 Tags

Tags complement mentions by allowing values to be referred to by name rather than position.
They are primarily useful when working with multiple values of the same type.

A tag is an instance of `Tag<TValue>`.
Each tag represents exactly one value of the given type.

Example:
```
protected static Tag<string> name = new(nameof(name));
protected static Tag<int> age = new(nameof(age)), shoeSize = new(nameof(shoeSize));
```
Providing a name for the tag improves diagnostic output.
The parameterless constructor `new()` may also be used.

#### 3.2.1 Set and reference tagged values
Tags can be used to set or reference values during pipeline configuration and execution.

Example:
```csharp
protected static Tag<string> surname = new(), lastname = new();
...
=> Given(surname).Is("Ada").And(lastname).Is("Lovelace")
.When(_ => _.CreateUser(The(surname), The(lastname)))
.Then().Result.FullName.Is("Ada Lovelace");
```

#### 3.2.2 Use tagged values as default or for auto-generation
Tagged values may also be used as:

- the default value when generating test data (`For.Input`)
- input when auto-generating the subject under test (`For.Subject`)

Example:
```csharp
Using(name, For.Input).And(age, For.Subject);
```

### 3.3 Data Generation and Pipeline

Xspec utilizes an internal data-generation pipeline to supply arbitrary data cleanly. When requesting a value, for instance `An<int>()`, the value is pulled through a pipeline in order (last first):

**1. Value**
* **Mutate/Transform value**: apply a mutation to or transform the value (applied in the opposite order they are supplied).
* **Factory**: Provide a value from a factory, or continue.
* **Value**: Provide an explicitly supplied value, or continue.

**2. Default**
* **Mutate/Transform value**: apply a mutation to or transform the value (applied in the opposite order they are supplied).
* **Factory**: Provide a value from a factory, or continue.
* **Value**: Provide an explicitly supplied value, or continue.

**3. Generation**
* **Map value**: Get a value of a different type from the default-pipeline and map to type.
* **Relay type**: Get a value of a different type from the default-pipeline and convert to type.
* **Generate value**: Use the generation strategy of the given type.
* **Return type default**: Use the types default-value or null.

If generation is required, Xspec evaluates types using a fallback strategy chain:
* **Primitives:** Out-of-the-box generation natively supports standard primitives like `int`, `Guid`, `DateTime`, `Uri`, and `TimeSpan`.
* **Semantic Types:** Objects deriving from `Semantic<TPrimitive>` (such as `Email`, `PhoneNumber`, `Age`) are securely auto-generated using their base values.
* **Abstracts & Interfaces:** Pure interfaces and abstract classes can be successfully requested and mocked instantly without boilerplate.

### 3.4 Type Registration and Conversion

If a specific custom mapping is required during data creation, you can override default generation by using the fluent type conversion pipeline.

* **Fluent Registration:** Configure the generator pipeline via `Using<TTarget>().From<TSource>()` — use values of the target type from the set described by the source type.
* **Smart Casting:** Once mapped, Xspec automatically probes the requested type for compatibility. It securely attempts to construct the target by finding implicit cast operators, single-parameter constructors, or matching static factory methods (e.g., `Create()`).
* **Conversion:** For explicit control, inject a conversion delegate directly, such as `Using<int>().From((byte b) => b + 1)` — the source type is inferred from the lambda parameter.
* **Chaining:** Register several conversions fluently with `And`, e.g. `Using<int>().From<byte>().And<long>().From<short>()`.
* **Scoping:** Like the value-level overloads, `Using<TTarget>()` accepts an optional `For` scope, e.g. `Using<int>(For.Input).From<byte>()` applies the conversion only when generating ambient test data, leaving subject construction unaffected. The default is `For.All`. Registrations for the same target type must have disjoint scopes: one for `Input` and another for `Subject` can coexist, but overlapping scopes throw `SetupFailed`.
* **Sequences:** For numeric and temporal source types, constrain the generated source values with `StartingAt` and `Spaced`, e.g. `Using<Age>().From<int>().StartingAt(18)`, `Using<int>().From<int>().StartingAt(10).Spaced(5)` (yielding 10, 15, 20, ...) or `Using<DateTime>().From<DateTime>().StartingAt(new DateTime(2024, 5, 1)).Spaced(TimeSpan.FromHours(6))`. `Spaced` also accepts a step function of the previous value, e.g. `Spaced(i => i * 2)` or `Spaced(d => d.AddMonths(1))`, or of the previous value and the position of the value being generated, e.g. `StartingAt(0).Spaced((_, i) => i * i)` yielding 0, 1, 4, 9, ... A negative spacing yields a descending sequence. Numbers default to starting at one, spaced one; `DateTime` defaults to starting at a fixed epoch, spaced one day; `TimeSpan` to starting at one day, spaced one day. Generated values are guaranteed unique: numeric sequences wrap around at the type's boundaries (`StartingAt((byte)254)` yields 254, 255, 0, 1, ...) while temporal sequences do not, and as soon as a sequence would repeat a value or step outside the type's range, generation throws `ValuesExhausted`.
* **Generator functions:** For arbitrary value spaces — non-numeric types or stateful series — pass a generator to `From`, e.g. `Using<Guid>().From(Guid.NewGuid)` or `Using<int>().From(NextFibonacci)` with a method or closure holding the state. Generated values are converted to the target type if necessary and used exactly as produced: the user defines the value space, so duplicates are allowed (1, 1, 2, 3, 5, ...).
* **Value lists:** To use exactly the given values, pass an explicit list to `From`, e.g. `Using<int>().From([10, 20, 30])`. The values are used in declaration order, duplicates allowed, and requesting more values than the list contains throws `ValuesExhausted`.
* **Safe Failures:** If incompatible types are relayed and no logical conversion path exists, generation strictly throws an `InvalidTypeConversion`.

```csharp
// Generates an Email instance automatically by creating a primitive source and looking for constructors or static factories
public class WhenConvertByConstructor : Spec<MyEmailConstr>
{
    public WhenConvertByConstructor() => Using<MyEmailConstr>().From<Email>();
}

// Employs a specific conversion lambda to translate generated data
public class WhenRelayIntToByteWithConverter : Spec<int>
{
    public WhenRelayIntToByteWithConverter() => Using<int>().From((byte b) => b + 1);

    [Fact] public void ThenGenerateByteAsInt() => Three<int>().Is().EqualTo([2, 3, 4]);
}
```

## 4. Mocking & Auto-Mocking

This part assumes familiarity with Moq or similar mocking frameworks. You should have a clear understanding of when and how mocking is typically used together with xUnit.
Here we will examine how the mocking experience can be simplified with the help of Xspec.

### 4.1 Auto-Mocking subject under test

The subject under test will be created automatically with mocks and default values.
Remember from chapter 2 that all mocks are configured after test data has been generated. 
So regardless of where you provide test data or constraints on test-data, they will be available in the mocking stage of the pipeline execution.

You can supply your own constructor arguments by calling `Using`, or modify the generated ones by calling `Given` with a setup or transform lambda.
You can even provide the subject under test itself:
`Using(new MyClass(42, "Thursday"))`

### 4.2 Mocking

To mock the behavior of a dependency, call `Given<[TheService]>().That(_ => _.[TheMethod](...)).Returns/Throws(...)`. 
`That` accepts any lambda you would normally supply to the constructor when creating a mock using `Moq`. 
You do not need to create and manage mocks manually, but can supply mocked behavior directly to the pipeline.
This allows most mocking scenarios to be expressed inline, close to the behavior under test.

### 4.3 Mocking with arguments

If you want to vary mocked behavior based on arguments, supply a lambda with arguments to Returns. The lambda signature must match the mocked call.
Up to five arguments are possible to mock in this way.

Example with two arguments:
```csharp
=> Given<IMyCalculator>()
   .That(_ => _.Add(TheFirst<int>(), TheSecond<int>()))
   .Returns((a, b) => a + b) //The mock adds the two arguments passed to the function and returns the sum
```

### 4.4 Mocking sequence of calls

Sometimes you want to mock a scenario where more than one call is made to a mock in succession, and the mock should potentially behave differently at each successive call.
You can describe this scenario using the methods `First` and `AndNext`.

Example mocking three successive calls:
```csharp
=> Given<IMyService>().That(_ => _.GetValueAsync())
    .First().Returns(() => 1) // returns 1 on first call
    .AndNext().Throws(An<ArgumentException>) //throw exception on second call
    .AndNext().Returns(); //return on third call
```

### 4.5 Observing calls with Tap

Tap allows observing arguments passed to a mocked call without affecting its behavior.
A method with up to five arguments is possible to tap.

Example:
```csharp
int _tappedValue = -1;

=> Given<IMyInterface>()
   .That(_ => _.Get(An<int>()))
   .Tap<int>(i => _tappedValue = i)
   .Returns(() => _retVal)
```

### 4.6 Verification

To verify a call to a mocked dependency, call `Then<[TheService]>([SomeLambdaExpression])`. 

Both mocking and verification of behavior are based on `Moq` framework.
 
Example:
```csharp
namespace MyProject.Spec.ShoppingService;

public class WhenPlaceOrder : Spec<MyProject.ShoppingService>
{
    public WhenPlaceOrder() 
        => When(_ => _.PlaceOrder(An<int>()))
        .Given<ICartRepository>().That(_ => _.GetCart(The<int>()))
        .Returns(() => A<Cart>(_ => _.Id = The<int>()));

    [Fact] public void ThenOrderIsCreated() => Then<IOrderService>(_ => _.CreateOrder(The<Cart>()));

    [Fact] public void ThenLogsOrderCreated()
        => Then<ILogger>(_ => _.Information($"OrderCreated from Cart {The<int>()}"));
}
```

The built-in mocking capabilities of Xspec cover almost all scenarios that Moq covers. 
But should you need some feature that is not provided by Xspec, you can create your mock using Moq explicitly and supply it to the pipeline using `Given(myMock.Object)`.

## 5. Asserting Results

Xspec comes with its own fluent assertion framework under the `Xspec.Assert` namespace. 
Even if you don't use any other feature of Xspec, you can use this framework as an alternative to `FluentAssertions` or `AwesomeAssertions`. 
However, combined with the Xspec pipeline is where it really shines.

**Reading advice**
What follows is a compact reference manual on the fluent nature of Xspec.Assert, followed by a complete list of features.

### 5.1 Fluent assertions

Assertions are made directly on the value to be verified.
Every assertion returns a continuation, allowing chaining of assertions.
The continuation is context-aware and allows different assertions depending on what was asserted previously.

#### 5.1.1 And
When you want to combine more than one assertion, all of which must pass
```csharp
3.Is().GreaterThan(2).and.LessThan(4);
```

#### 5.1.2 Either - Or
When you want to combine two assertions, one of which must pass
```csharp
3.Is().either.GreaterThan(4).or.LessThan(4);
```

#### 5.1.3 Not
Any assertion can be negated by placing not before (note lowercase not)
```csharp
3.Is().not.GreaterThan(4);
```

### 5.2 Values
Values of any type can be verified with any of the two extension methods `Is` and `Has`

#### 5.2.1 Is
- Equal:  
  `Result.Is(3)`  
  `Result.Is().EqualTo(3)`  
- Equivalent: (for objects)  
  `Result.Is().Like(new MyObject {Id = 3})`  
  `Result.Is().EquivalentTo(new MyObject {Id = 3})`  
- Not equal:  
  `Result.Is().Not(3)`  
- Null:  
  `Result.Is().Null()`  
- Greater than:  
  `3.Is().GreaterThan(2)`  
- Less than:  
  `3.Is().LessThan(2)`  
- Approximately equal with tolerance:  
  `Result.Is().Around(3, 0.1)`  
- Even: (true if number is divisible by 2)  
  `Result.Is().Even()`  
- OneOf:  
  `Result.Is().OneOf(Three<int>())`  
- True: (for booleans)  
  `Result.Is().True()`  
- False: (for booleans)  
  `Result.Is().False()`  

#### 5.2.2 Has
- Verify that the result has a given condition:  
  `Result.Has(_ => _.Id == 3)`  
- Verify that the result has the given type:  
  `Result.Has().Type<MyModel>()`  

### 5.3 Strings

#### 5.3.1 Is
- Like  
  `" ABC ".Is().Like("abc")`  
- EquivalentTo  
  `" ABC ".Is().EquivalentTo("abc")`  
- Empty  
  `"".Is().Empty()`  
- NullOrEmpty  
  `((string)null).Is().NullOrEmpty()`  
- NullOrWhitespace  
  `" ".Is().NullOrWhitespace()`  

#### 5.3.2 Does
- Contain  
  `"ABC".Does().Contain("AB")`  
- StartWith  
  `"ABC".Does().StartWith("AB")`  
- EndWith  
  `"ABC".Does().EndWith("BC")`  

### 5.4 Time

- Before  
  `DateTime.Now.Is().Before(DateTime.Now.AddDays(1))`  
- After  
  `DateTime.Now.Is().After(DateTime.Now.AddDays(-1))`  
- CloseTo  
  `DateTime.Now.Is().CloseTo(DateTime.Now.AddDays(1), TimeSpan.FromDays(2))`  
  `TimeSpan.FromDays(4).Is().CloseTo(TimeSpan.FromDays(3), TimeSpan.FromDays(2))`  
- Positive  
  `TimeSpan.FromDays(1).Is().Positive()`  
- Negative  
  `TimeSpan.FromDays(1).Is().Negative()`  

### 5.5 Collections

#### 5.5.1 Is
- EqualTo  
  all elements are equal and in the same order  
  `list.Is().EqualTo(otherList)`  
- Like  
  all elements are equal but order may differ  
  `list.Is().Like(otherList)`  
- SameAs  
  reference equal  
  `list.Is().SameAs(otherList)`  
- EquivalentTo  
  all elements are equal but order may differ  
  `list.Is().EquivalentTo(otherList)`  
- Empty  
  `list.Is().Empty()`  
- Distinct  
  `list.Is().Distinct()` // all elements in the collection are different  
  `list.Is().Distinct(it => it.Id)` // all elements have different values of the given property  

#### 5.5.2 Does
- Contain  
  `list.Does().Contain(3)`  

#### 5.5.3 Has
- Count  
  `list.Has().Count(3)`  
  `list.Has().Count(it => it > 3).At(2)` // with condition  
- Count at least  
  `list.Has().Count().AtLeast(2)`  
  `list.Has().Count(it => it > 3).AtLeast(2)` // with condition  
- Count at most  
  `list.Has().Count().AtMost(2)`  
  `list.Has().Count(it => it > 3).AtMost(2)` // with condition  
- Count in range  
  `list.Has().Count().InRange(2, 4)`  
  `list.Has().Count(it => it > 3).InRange(2, 4)` // with condition  
- Order ascending  
  `list.Has().Order().Ascending()`  
  `list.Has().Order(it => it.Age).Ascending()` // with condition  
- Order descending  
  `list.Has().Order().Descending()`  
  `list.Has().Order(it => it.Age).Descending()` // with condition  
- [One/Two/Three/Four/Five]Items  
  verify that the collection has the given number of items and return them as a n-tuple  
  `numbers.Has().OneItem().that.Is(3)` // numbers have one item, and that item is 3  
  `patients.Has().OneItem().that.Age.Is(3)` // patients have one item, and its age is 3  
  `patients.Has().OneItem(it => it.Age == 3).that.Gender.Is('F')` // patients have one item aged 3, and its gender is female  
- All  
  `list.Has().All(it => it.Age > 3)` // all items in the collection match the criteria  
  `list.Has().All((it, i) => it.Age > i)` // with index of item  
  `list.Has().All(it => it.Age.Is().GreaterThan(3))` // apply assertion to all items  
- Some  
  `list.Has().Some(it => it.Age > 3)` - at least one item in the collection matches the criteria  
- None  
  `list.Has().None(it => it.Age > 3)` - no item in the collection matches the criteria  

### 5.6 Justifying assertions with because

Each test method contains exactly one logical assertion. To document *why* the expected outcome
is the correct outcome, provide a rationale with the named argument `because` in `Then`:

```csharp
[Fact] public void ThenCircumferenceIsAroundSixPi()
    => Then(because: "the world is round").Result.Is().Around(Math.PI * 6, 0.001);
```

The reason is included in the generated specification, appended after the assertion
in natural reading order:

```csharp
When world.Circumference
Then Result is around 18.8496, because the world is round
```

Phrase the reason so it reads naturally after the word "because".
It should justify the expectation rather than restate it — the test name already says *what*
is expected; `because` explains *why*. The reason can only be provided once per test method,
in line with one logical assertion per test method, and covers all technical assertions
chained after it (with `and`, `either`/`or` etc.).

## 6. Guidelines

### 6.1 Recommended test structure

Based on the way xUnit and Xspec work and on experience using it, this is an opinionated recommendation on how to structure your tests using Xspec.
The goal of these conventions is to keep specifications readable, navigable, and aligned with production structure as test suites grow.

1. Mimic the folder structure of your production code to be tested
   - Create one project per production project called *[ProductionFolder].Test* or *[ProductionFolder].Spec*
   - Create a leaf-folder per subject under test called *[NameOfClass]*
1. Create one test-class per method under test, called *When[NameOfMethod]*
1. Let the class for each method under test be abstract and nest concrete classes inside it for each different setup, called *Given[SomePrecondition]*
1. Place setup in the constructor and assertions in the test methods
1. Feel free to nest given classes in more than one layer (but avoid more than four levels of nesting)
1. Write one test-method per logical assertion (i.e. test only one thing per test)
1. Feel free to use all of Xunit's features - such as Fact, Theory and test-data
1. Use only the built-in assertion framework from Xspec (it will give you cleaner specifications with clearer test-output)

Example:
```csharp
public abstract class WhenPlaceOrder : Spec<ShoppingService> 
{
    static Tag<Guid> cartId = new();
  
    protected WhenPlaceOrder() => When(_ => _.PlaceOrder(The(cartId)));

    public abstract class GivenCartExists : WhenPlaceOrder 
    { 
        protected GivenCartExists()
            => Given<ICartRepository>().That(_ => _.GetCart(The(cartId))).Returns(A<Cart>());

        public class WithItems : GivenCartExists 
        {
        ...
        }

        public class WithoutItems : GivenCartExists 
        {
        ...
        }
    }

    public class GivenCartNotExists : WhenPlaceOrder 
    {
        public GivenCartNotExists()
            => Given<ICartRepository>().That(_ => _.GetCart(The(cartId))).Returns(() => Cart.NoCart);

        ...
    }
}
```

### 6.2 Class fixtures

Xspec supports the xUnit feature of sharing setup between tests in a common class fixture.
A class fixture is created in the same way as a test class, by inheriting `Spec` and providing setup.
The only difference between a class fixture implemented with Xspec and a test class implemented with Xspec
is that class fixtures don't have test methods or assertions.

When using a class fixture and having more than one test method, no setup should be put in the constructor, 
since the constructor is run once for each test method and provides the setup to the shared class fixture 
(i.e. would add the same setup multiple times, including after the test pipeline was executed, which Xspec does not allow).

### 6.3 Some final advice

Unit tests work best when they run *fast*. Write modular production code in line with best practices, 
so that each unit can be tested in isolation while mocking or ignoring the rest.
This enables tiny test methods with a single logical assertion and shared setup.

However, remember that the entire test pipeline is built and disposed for each test method (a feature of xUnit).
If a specification requires non-trivial setup or execution time, it can be reasonable to group
multiple *closely related* assertions into the same test method to reduce overall suite runtime.

Xspec is designed to thrive in clean, well-structured codebases.
Its emphasis on explicit structure and readable specifications is intended to reinforce those qualities,
helping teams maintain clarity and confidence as both code and test suites grow.

## 7. Beyond TDD: Specification-Driven Agentic Development

While Xspec is a highly effective tool for traditional Test-Driven Development (TDD), it is ultimately designed to facilitate a new paradigm: **Specification-Driven Agentic Development (SDAD)**. 

SDAD evolves TDD by treating the developer as the specifier and the AI agent as the implementer. Because Xspec enforces a strict, readable, and declarative structure, it creates the perfect shared language between human intent and AI generation.

The SDAD cycle works as follows:
1. **Specify (Human):** The developer writes a clear, executable specification using Xspec to define the exact requirements and expected behavior.
2. **Red Phase (Automated):** An AI agent takes over, generating the production code necessary to make all specifications (tests) pass.
3. **Grey Phase (Semi-Automated):** The AI agent switches to refactoring mode, cleaning up the generated code and providing quality metrics.
4. **Green Phase (Manual):** The human developer reviews the clean code, collaborating with the AI to optimize structure and readability before starting the next cycle.

By removing boilerplate and focusing purely on the *Given-When-Then* logic, Xspec allows developers to focus on the "what" while the AI handles the "how."