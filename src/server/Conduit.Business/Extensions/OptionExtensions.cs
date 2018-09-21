using Optional;
using System;
using System.Threading.Tasks;

namespace Conduit.Business.Extensions
{
    public static class OptionExtensions
    {
        public static Option<T, TException> Filter<T, TException>(this Option<T, TException> option, Func<T, bool> predicate, TException exception) =>
            option.Match(
                x => predicate(x) ?
                        option :
                        Option.None<T, TException>(exception),
                _ => option);

        public static Task<Option<T>> FilterAsync<T>(this Option<T> option, Func<T, Task<bool>> predicate) =>
            option.MatchAsync(
                async x => await predicate(x) ?
                    option :
                    Option.None<T>(),
                () => option);

        public static Task<Option<T, TException>> FilterAsync<T, TException>(this Option<T, TException> option, Func<T, Task<bool>> predicate, TException exception) =>
            option.MatchAsync(
                async x => await predicate(x) ?
                    option :
                    Option.None<T, TException>(exception),
                _ => option);

        public static async Task<Option<T>> FilterAsync<T>(this Task<Option<T>> optionTask, Func<T, Task<bool>> predicate) =>
            await (await optionTask).FilterAsync(predicate);

        public static async Task<Option<T, TException>> FilterAsync<T, TException>(this Task<Option<T, TException>> optionTask, Func<T, Task<bool>> predicate, TException exception) =>
            await (await optionTask).FilterAsync(predicate, exception);

        public static async Task<Option<T, TException>> WithException<T, TException>(this Task<Option<T>> option, TException exception) =>
            (await option).WithException(exception);

        public static Task<Option<TResult, TException>> FlatMapAsync<T, TException, TResult>(this Task<Option<T, TException>> option, Func<T, Task<Option<TResult, TException>>> mapping) =>
            option.MatchAsync(
                some: async val => (await mapping(val)),
                none: async err => Option.None<TResult, TException>(err));

        public static Task<Option<TResult>> FlatMapAsync<T, TResult>(this Task<Option<T>> option, Func<T, Task<Option<TResult>>> mapping) =>
            option.MatchAsync(
                some: async val => (await mapping(val)),
                none: async () => Option.None<TResult>());

        public static Task<Option<TResult, TException>> FlatMapAsync<T, TException, TResult>(this Option<T, TException> option, Func<T, Task<Option<TResult, TException>>> mapping) =>
            option.MatchAsync(
                some: async val => await mapping(val),
                none: err => Option.None<TResult, TException>(err));

        public static Task<Option<TResult, TException>> FlatMapAsync<T, TException, TResult>(this Option<T> option, Func<T, Task<Option<TResult, TException>>> mapping, TException exception) =>
            option.MatchAsync(
                some: async val => await mapping(val),
                none: async () => Option.None<TResult, TException>(exception));

        public static Task<Option<TResult>> FlatMapAsync<T, TResult>(this Option<T> option, Func<T, Task<Option<TResult>>> mapping) =>
            option.MatchAsync(
                some: async val => await mapping(val),
                none: () => Option.None<TResult>());

        public static Task<Option<TResult>> MapAsync<T, TResult>(this Task<Option<T>> option, Func<T, Task<TResult>> mapping) =>
            option.MatchAsync(
                some: async val => (await mapping(val)).Some<TResult>(),
                none: async () => Option.None<TResult>());

        public static Task<Option<TResult, TException>> MapAsync<T, TException, TResult>(this Option<T, TException> option, Func<T, Task<TResult>> mapping) =>
            option.MatchAsync(
                some: async val => (await mapping(val)).Some<TResult, TException>(),
                none: err => Option.None<TResult, TException>(err));

        public static Task<Option<TResult, TException>> MapAsync<T, TException, TResult>(this Task<Option<T, TException>> option, Func<T, Task<TResult>> mapping) =>
            option.MatchAsync(
                some: async val => (await mapping(val)).Some<TResult, TException>(),
                none: async err => Option.None<TResult, TException>(err));

        public static Task<Option<TResult>> MapAsync<T, TResult>(this Option<T> option, Func<T, Task<TResult>> mapping) =>
            option.MatchAsync(
                some: async val => (await mapping(val)).Some<TResult>(),
                none: () => Option.None<TResult>());

        public static Task<TResult> MatchAsync<T, TResult>(this Option<T> option, Func<T, Task<TResult>> some, Func<Task<TResult>> none) =>
            option.Match(some, none);

        public static Task<TResult> MatchAsync<T, TException, TResult>(this Option<T, TException> option, Func<T, Task<TResult>> some, Func<TException, Task<TResult>> none) =>
            option.Match(some, none);

        public static async Task<TResult> MatchAsync<T, TException, TResult>(this Task<Option<T, TException>> option, Func<T, Task<TResult>> some, Func<TException, Task<TResult>> none) =>
            await (await option).Match(some, none);

        public static async Task<TResult> MatchAsync<T, TResult>(this Task<Option<T>> option, Func<T, Task<TResult>> some, Func<Task<TResult>> none) =>
            await (await option).Match(some, none);

        public static Task<TResult> MatchAsync<T, TResult>(this Option<T> option, Func<T, Task<TResult>> some, Func<TResult> none) =>
            option.Match(
                some: x => some(x),
                none: () => Task.FromResult(none()));

        public static Task<TResult> MatchAsync<T, TException, TResult>(this Option<T, TException> option, Func<T, Task<TResult>> some, Func<TException, TResult> none) =>
            option.Match(
                some: x => some(x),
                none: e => Task.FromResult(none(e)));

        public static async Task MatchSomeAsync<T>(this Option<T> option, Func<T, Task> some) =>
            option.MatchSome(val => some(val));

        public static async Task<Option<T, TException>> SomeNotNull<T, TException>(this Task<T> task, TException exception) =>
            (await task).SomeNotNull(exception);

        public static async Task<Option<T>> SomeNotNull<T>(this Task<T> task) =>
            (await task).SomeNotNull();

        public static Option<T, TException> SomeWhen<T, TException>(
            this T value,
            Func<T, bool> predicate,
            Func<T, TException> exceptionFactory)
        {
            var result = predicate(value);

            return result ?
                value.Some<T, TException>() :
                Option.None<T, TException>(exceptionFactory(value));
        }

        public static async Task<Option<T, TException>> SomeWhen<T, TException>(
            this Task<T> valueTask,
            Func<T, bool> predicate,
            Func<T, TException> exceptionFactory)
        {
            var value = await valueTask;
            var result = predicate(value);

            return result ?
                value.Some<T, TException>() :
                Option.None<T, TException>(exceptionFactory(value));
        }

        public static async Task<Option<T, TException>> SomeWhen<T, TException>(this Task<T> task, Func<T, bool> predicate, TException exception) =>
            (await task).SomeWhen(predicate, exception);

        public static async Task<Option<T, TException>> SomeWhenAsync<T, TException>(
            this T value,
            Func<T, Task<bool>> predicate,
            TException exception)
        {
            var result = await predicate(value);

            return result ?
                value.Some<T, TException>() :
                Option.None<T, TException>(exception);
        }

        public static async Task<Option<T, TException>> SomeWhenAsync<T, TException>(
            this T value,
            Func<T, Task<bool>> predicate,
            Func<T, TException> exceptionFactory)
        {
            var result = await predicate(value);

            return result ?
                value.Some<T, TException>() :
                Option.None<T, TException>(exceptionFactory(value));
        }
    }
}