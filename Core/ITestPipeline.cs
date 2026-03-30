using Moq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Xspec.Continuations;

namespace Xspec;

/// <summary>
/// A continuation to further specify the test
/// </summary>
/// <typeparam name="TSUT"></typeparam>
/// <typeparam name="TResult"></typeparam>
public interface ITestPipeline<TSUT, TResult>
{
    /// <summary>
    /// Runs the test pipeline and generates the result, which can be accessed by the Result property. When the test is run, any provided arrangement will be applied, then the subject-under-test will be created and the method-under-test called.<br/>
    /// </summary>
    /// <returns>The test result, containing any return values or exceptions thrown, upon which assertions can be made</returns>
    ITestResultWithSUT<TSUT, TResult> Then();

    /// <summary>
    /// Runs the test pipeline and generates the result, which can be accessed by the Result property. When the test is run, any provided arrangement will be applied, then the subject-under-test will be created and the method-under-test called.<br/>
    /// </summary>
    /// <typeparam name="TSubject"></typeparam>
    /// <param name="subject"></param>
    /// <returns>The provided argument is returned, allowing assertions on the provided arguments to be chained</returns>
    TSubject Then<TSubject>(TSubject subject);

    /// <summary>
    /// Run the test-pipeline, generates the result and asserts the given mock invocation.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="expression"></param>
    /// <param name="expressionExpr"></param>
    /// <returns>A continuation to apply additional assertions on the test result</returns>
    IAndVerify<TResult> Then<TService>(
        Expression<Action<TService>> expression,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null)
        where TService : class;

    /// <summary>
    /// Run the test-pipeline and verify mock invocation.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="expression"></param>
    /// <param name="times"></param>
    /// <param name="expressionExpr"></param>
    /// <returns>A continuation to apply additional assertions on the test result</returns>
    IAndVerify<TResult> Then<TService>(
        Expression<Action<TService>> expression, Times times,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null)
        where TService : class;

    /// <summary>
    /// Run the test-pipeline and verify mock invocation.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="expression"></param>
    /// <param name="times"></param>
    /// <param name="expressionExpr"></param>
    /// <returns>A continuation to apply additional assertions on the test result</returns>
    IAndVerify<TResult> Then<TService>(
        Expression<Action<TService>> expression, Func<Times> times,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null)
        where TService : class;

    /// <summary>
    /// Run the test-pipeline and verify mock invocation.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TReturns"></typeparam>
    /// <param name="expression"></param>
    /// <param name="expressionExpr"></param>
    /// <returns>A continuation to apply additional assertions on the test result</returns>
    IAndVerify<TResult> Then<TService, TReturns>(
        Expression<Func<TService, TReturns>> expression,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null)
        where TService : class;

    /// <summary>
    /// Run the test-pipeline and verify mock invocation.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TReturns"></typeparam>
    /// <param name="expression"></param>
    /// <param name="times"></param>
    /// <param name="expressionExpr"></param>
    /// <returns>A continuation to apply additional assertions on the test result</returns>
    IAndVerify<TResult> Then<TService, TReturns>(
        Expression<Func<TService, TReturns>> expression, Times times,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null)
        where TService : class;

    /// <summary>
    /// Run the test-pipeline and verify mock invocation.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TReturns"></typeparam>
    /// <param name="expression"></param>
    /// <param name="times"></param>
    /// <param name="expressionExpr"></param>
    /// <returns>A continuation to apply additional assertions on the test result</returns>
    IAndVerify<TResult> Then<TService, TReturns>(
        Expression<Func<TService, TReturns>> expression, Func<Times> times,
        [CallerArgumentExpression(nameof(expression))] string? expressionExpr = null)
        where TService : class;

    /// <summary>
    /// Provide the method-under-test to the test-pipeline
    /// </summary>
    /// <param name="act"></param>
    /// <param name="actExpr"></param>
    /// <returns></returns>
    ITestPipeline<TSUT, TResult> When(Action<TSUT> act,
        [CallerArgumentExpression(nameof(act))] string? actExpr = null);

    /// <summary>
    /// Provide the method-under-test to the test-pipeline
    /// </summary>
    /// <param name="act"></param>
    /// <param name="actExpr"></param>
    /// <returns></returns>
    ITestPipeline<TSUT, TResult> When(Action act,
        [CallerArgumentExpression(nameof(act))] string? actExpr = null);

    /// <summary>
    /// Provide the method-under-test to the test-pipeline
    /// </summary>
    /// <param name="act"></param>
    /// <param name="actExpr"></param>
    /// <returns>A continuation for providing further arrangement, or executing the test</returns>
    ITestPipeline<TSUT, TResult> When(Func<TSUT, TResult> act,
        [CallerArgumentExpression(nameof(act))] string? actExpr = null);

    /// <summary>
    /// Provide the method-under-test to the test-pipeline
    /// </summary>
    /// <param name="act"></param>
    /// <param name="actExpr"></param>
    /// <returns>A continuation for providing further arrangement, or executing the test</returns>
    ITestPipeline<TSUT, TResult> When(Func<TResult> act,
        [CallerArgumentExpression(nameof(act))] string? actExpr = null);

    /// <summary>
    /// Provide the method-under-test to the test-pipeline
    /// </summary>
    /// <param name="act"></param>
    /// <param name="actExpr"></param>
    /// <returns></returns>
    ITestPipeline<TSUT, TResult> When(Func<TSUT, Task> act,
        [CallerArgumentExpression(nameof(act))] string? actExpr = null);

    /// <summary>
    /// Provide the method-under-test to the test-pipeline
    /// </summary>
    /// <param name="act"></param>
    /// <param name="actExpr"></param>
    /// <returns></returns>
    ITestPipeline<TSUT, TResult> When(Func<Task> act,
        [CallerArgumentExpression(nameof(act))] string? actExpr = null);

    /// <summary>
    /// Provide the method-under-test to the test-pipeline
    /// </summary>
    /// <param name="act"></param>
    /// <param name="actExpr"></param>
    /// <returns></returns>
    ITestPipeline<TSUT, TResult> When(Func<TSUT, Task<TResult>> act,
        [CallerArgumentExpression(nameof(act))] string? actExpr = null);

    /// <summary>
    /// Provide the method-under-test to the test-pipeline
    /// </summary>
    /// <param name="act"></param>
    /// <param name="actExpr"></param>
    /// <returns></returns>
    ITestPipeline<TSUT, TResult> When(
        Func<Task<TResult>> act,
        [CallerArgumentExpression(nameof(act))] string? actExpr = null);

    /// <summary>
    /// Provide a Setup method that will be called before the method-under-test, 
    /// Setup methods are executed in the opposite order that they are provided
    /// </summary>
    /// <param name="setUp">the method to call as setup before executing the method-under-test</param>
    /// <param name="delayBeforeNextMs">Delay between this method invocation and the next in the pipeline</param>
    /// <param name="setUpExpr">Provided by the compiler</param>
    /// <param name="delayExpr">Provided by the compiler</param>
    /// <returns>A continuation to provide further arrangement to the test-pipeline</returns>
    ITestPipeline<TSUT, TResult> After(
        Action<TSUT> setUp, 
        Func<int>? delayBeforeNextMs = null,
        [CallerArgumentExpression(nameof(setUp))] string? setUpExpr = null,
        [CallerArgumentExpression(nameof(delayBeforeNextMs))] string? delayExpr = null);

    /// <summary>
    /// Provide a Setup method that will be called before the method-under-test.
    /// Setup methods are executed in the opposite order that they are provided
    /// </summary>
    /// <param name="setUp">the method to call as setup before executing the method-under-test</param>
    /// <param name="delayBeforeNextMs">Delay between this method invocation and the next in the pipeline</param>
    /// <param name="setUpExpr">Provided by the compiler</param>
    /// <param name="delayExpr">Provided by the compiler</param>
    /// <returns>A continuation to provide further arrangement to the test-pipeline</returns>
    ITestPipeline<TSUT, TResult> After(
        Func<TSUT, Task> setUp, 
        Func<int>? delayBeforeNextMs = null,
        [CallerArgumentExpression(nameof(setUp))] string? setUpExpr = null,
        [CallerArgumentExpression(nameof(delayBeforeNextMs))] string? delayExpr = null);

    /// <summary>
    /// Provide a Teardown method that will be called on Dispose of the test class/fixture, 
    /// Teardown methods are executed in the order that they are provided
    /// </summary>
    /// <param name="tearDown">the method to call as teardown after executing the method-under-test</param>
    /// <param name="tearDownExpr"></param>
    /// <returns>A continuation to provide further arrangement to the test-pipeline</returns>
    ITestPipeline<TSUT, TResult> Before(Action<TSUT> tearDown,
        [CallerArgumentExpression(nameof(tearDown))] string? tearDownExpr = null);

    /// <summary>
    /// Provide a Teardown method that will be called on Dispose of the test class/fixture, 
    /// Teardown methods are executed in the order that they are provided
    /// </summary>
    /// <param name="tearDown">the method to call as teardown after executing the method-under-test</param>
    /// <param name="tearDownExpr"></param>
    /// <returns>A continuation to provide further arrangement to the test-pipeline</returns>
    ITestPipeline<TSUT, TResult> Before(Func<TSUT, Task> tearDown,
        [CallerArgumentExpression(nameof(tearDown))] string? tearDownExpr = null);

    /// <summary>
    /// Transform any value and use the transformed value as default
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="transform"></param>
    /// <param name="transformExpr"></param>
    /// <returns></returns>
    IGivenTestPipeline<TSUT, TResult> Given<TValue>(
        Func<TValue, TValue> transform,
        [CallerArgumentExpression(nameof(transform))] string? transformExpr = null);

    /// <summary>
    /// Provide any arrangement to the test, which will be applied during test execution in reverse order of where in the test-pipeline it was provided
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="setup"></param>
    /// <param name="setupExpr"></param>
    /// <returns></returns>
    IGivenTestPipeline<TSUT, TResult> Given<TValue>(
        Action<TValue> setup,
        [CallerArgumentExpression(nameof(setup))] string? setupExpr = null)
        where TValue : class;

    /// <summary>
    /// Provide a default value, that will be applied in all mocks and auto-generated test-data, where no specific value or setup is given.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="defaultValue"></param>
    /// <param name="defaultValueExpr"></param>
    /// <returns></returns>
    IGivenTestPipeline<TSUT, TResult> Given<TValue>(
        TValue defaultValue,
        [CallerArgumentExpression(nameof(defaultValue))] string? defaultValueExpr = null);

    /// <summary>
    /// A continuation to provide further arrangement
    /// </summary>
    /// <returns></returns>
    IGivenContinuation<TSUT, TResult> Given();

    /// <summary>
    /// A continuation for providing mock-setup for the given type
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    IGivenServiceContinuation<TSUT, TResult, TService> Given<TService>() where TService : class;

    /// <summary>
    /// Provide a default value as a lambda, to be evaluated during test execution AFTER any subsequently added arrangement.
    /// Providing a default value as a lambda, to defer execution, is useful when the default value is created based on test data that is specified later in the test-pipeline.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="defaultValue"></param>
    /// <param name="defaultValueExpr"></param>
    /// <returns></returns>
    IGivenTestPipeline<TSUT, TResult> Given<TValue>(
        Func<TValue> defaultValue,
        [CallerArgumentExpression(nameof(defaultValue))] string? defaultValueExpr = null);

    /// <summary>
    /// Provide a tag to setup some expectation, such as associating it with a value.
    /// </summary>
    /// <typeparam name="TValue">The type of value the tag is associated with</typeparam>
    /// <param name="tag">The tag</param>
    /// <param name="tagExpr">Leave empty. Provided by the compiler</param>
    /// <returns></returns>
    IGivenTag<TSUT, TResult, TValue> Given<TValue>(
        Tag<TValue> tag,
        [CallerArgumentExpression(nameof(tag))] string? tagExpr = null);
}