using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using FluentAssertions;
using Xunit;
using Rocks.Dataflow.Extensions;
using Rocks.Dataflow.SplitJoin;

namespace Rocks.Dataflow.Tests.CoreTests
{
    public class SplitJoinAsyncTests
    {
        [Fact]
        public async Task SplitJoin_CorrectlyBuild ()
        {
            // arrange
            var process = new ConcurrentBag<string> ();

            var split_block = DataflowSplitJoin.CreateSplitBlockAsync<string, char> (async s =>
                                                                                           {
                                                                                               await Task.Yield ();
                                                                                               process.Add (s);
                                                                                               return s.ToCharArray ();
                                                                                           });

            var join_block = DataflowSplitJoin.CreateFinalJoinBlock<string, char> ();

            split_block.LinkWithCompletionPropagation (join_block);


            // act
            foreach (var str in new[] { "a", "bb", "ccc" })
                await split_block.SendAsync (str);

            split_block.Complete ();
            await Task.WhenAll (split_block.Completion, join_block.Completion);


            // assert
            process.Should ().BeEquivalentTo ("a", "bb", "ccc");
        }


        [Fact]
        public async Task SplitProcessJoin_CorrectlyBuild ()
        {
            // arrange
            var process = new ConcurrentBag<char> ();

            var split_block = DataflowSplitJoin.CreateSplitBlockAsync<string, char> (async s =>
                                                                                           {
                                                                                               await Task.Yield ();
                                                                                               return s.ToCharArray ();
                                                                                           });

            var process_block = DataflowSplitJoin.CreateProcessBlockAsync<string, char> (async (s, c) =>
                                                                                               {
                                                                                                   await Task.Yield ();
                                                                                                   process.Add (c);
                                                                                                   await Task.Yield ();
                                                                                               });

            var join_block = DataflowSplitJoin.CreateFinalJoinBlock<string, char> ();

            split_block.LinkWithCompletionPropagation (process_block);
            process_block.LinkWithCompletionPropagation (join_block);


            // act
            foreach (var str in new[] { "a", "bb", "ccc" })
                await split_block.SendAsync (str);

            split_block.Complete ();
            await Task.WhenAll (split_block.Completion, join_block.Completion);


            // assert
            process.Should ().BeEquivalentTo ('a', 'b', 'b', 'c', 'c', 'c');
        }


        [Fact]
        public async Task SplitProcessJoinAction_CorrectlyBuild ()
        {
            // arrange
            var result = new ConcurrentBag<string> ();
            var process = new ConcurrentBag<char> ();

            var split_block = DataflowSplitJoin.CreateSplitBlockAsync<string, char> (async s =>
                                                                                           {
                                                                                               await Task.Yield ();
                                                                                               return s.ToCharArray ();
                                                                                           });

            var process_block = DataflowSplitJoin.CreateProcessBlockAsync<string, char> (async (s, c) =>
                                                                                               {
                                                                                                   await Task.Yield ();
                                                                                                   process.Add (c);
                                                                                               });

            var join_block = DataflowSplitJoin.CreateJoinBlock<string, char> ();

            var final_block = new ActionBlock<SplitJoinResult<string, char>> (async x =>
                                                                                    {
                                                                                        await Task.Yield ();
                                                                                        result.Add (
                                                                                            new string (x.SuccessfullyCompletedItems.ToArray ()));
                                                                                    });

            split_block.LinkWithCompletionPropagation (process_block);
            process_block.LinkWithCompletionPropagation (join_block);
            join_block.LinkWithCompletionPropagation (final_block);


            // act
            foreach (var str in new[] { "a", "bb", "ccc" })
                await split_block.SendAsync (str);

            split_block.Complete ();
            await Task.WhenAll (split_block.Completion, final_block.Completion);


            // assert
            result.Should ().BeEquivalentTo ("a", "bb", "ccc");
            process.Should ().BeEquivalentTo ('a', 'b', 'b', 'c', 'c', 'c');
        }


        [Fact]
        public async Task SplitProcessProcessJoinAction_CorrectlyBuild ()
        {
            // arrange
            var result = new ConcurrentBag<string> ();
            var process = new ConcurrentBag<char> ();
            var process2 = new ConcurrentBag<char> ();

            var split_block = DataflowSplitJoin.CreateSplitBlockAsync<string, char> (async s =>
                                                                                           {
                                                                                               await Task.Yield ();
                                                                                               return s.ToCharArray ();
                                                                                           });

            var process_block = DataflowSplitJoin.CreateProcessBlockAsync<string, char> (async (s, c) =>
                                                                                               {
                                                                                                   await Task.Yield ();
                                                                                                   process.Add (c);
                                                                                               });

            var process_block2 = DataflowSplitJoin.CreateProcessBlockAsync<string, char> (async (s, c) =>
                                                                                                {
                                                                                                    await Task.Yield ();
                                                                                                    process2.Add (c);
                                                                                                });

            var join_block = DataflowSplitJoin.CreateJoinBlock<string, char> ();

            var final_block = new ActionBlock<SplitJoinResult<string, char>> (async x =>
                                                                                    {
                                                                                        await Task.Yield ();
                                                                                        result.Add (
                                                                                            new string (x.SuccessfullyCompletedItems.ToArray ()));
                                                                                    });

            split_block.LinkWithCompletionPropagation (process_block);
            process_block.LinkWithCompletionPropagation (process_block2);
            process_block2.LinkWithCompletionPropagation (join_block);
            join_block.LinkWithCompletionPropagation (final_block);


            // act
            foreach (var str in new[] { "a", "bb", "ccc" })
                await split_block.SendAsync (str);

            split_block.Complete ();
            await Task.WhenAll (split_block.Completion, final_block.Completion);


            // assert
            result.Should ().BeEquivalentTo ("a", "bb", "ccc");
            process.Should ().BeEquivalentTo ('a', 'b', 'b', 'c', 'c', 'c');
            process2.Should ().BeEquivalentTo ('a', 'b', 'b', 'c', 'c', 'c');
        }


        [Fact]
        public async Task SplitProcessTransformJoinAction_CorrectlyBuild ()
        {
            // arrange
            var result = new ConcurrentBag<string> ();
            var process = new ConcurrentBag<char> ();
            var process2 = new ConcurrentBag<char> ();

            var split_block = DataflowSplitJoin.CreateSplitBlockAsync<string, char> (async s =>
                                                                                           {
                                                                                               await Task.Yield ();
                                                                                               return s.ToCharArray ();
                                                                                           });

            var process_block = DataflowSplitJoin.CreateProcessBlockAsync<string, char> (async (s, c) =>
                                                                                               {
                                                                                                   await Task.Yield ();
                                                                                                   process.Add (c);
                                                                                               });

            var transform_block = DataflowSplitJoin.CreateTransformBlockAsync<string, char, string> (async (s, c) =>
                                                                                                           {
                                                                                                               await Task.Yield ();

                                                                                                               process2.Add (c);
                                                                                                               return
                                                                                                                   c.ToString (
                                                                                                                       CultureInfo.InvariantCulture);
                                                                                                           });

            var join_block = DataflowSplitJoin.CreateJoinBlock<string, string> ();
            var final_block = new ActionBlock<SplitJoinResult<string, string>> (async x =>
                                                                                      {
                                                                                          await Task.Yield ();

                                                                                          foreach (var item in x.SuccessfullyCompletedItems)
                                                                                              result.Add (item);
                                                                                      });

            split_block.LinkWithCompletionPropagation (process_block);
            process_block.LinkWithCompletionPropagation (transform_block);
            transform_block.LinkWithCompletionPropagation (join_block);
            join_block.LinkWithCompletionPropagation (final_block);


            // act
            foreach (var str in new[] { "a", "bb", "ccc" })
                await split_block.SendAsync (str);

            split_block.Complete ();
            await Task.WhenAll (split_block.Completion, final_block.Completion);


            // assert
            result.Should ().BeEquivalentTo ("a", "b", "b", "c", "c", "c");
            process.Should ().BeEquivalentTo ('a', 'b', 'b', 'c', 'c', 'c');
            process2.Should ().BeEquivalentTo ('a', 'b', 'b', 'c', 'c', 'c');
        }


        [Fact]
        public async Task SplitProcessJoinAction_ProcessThrowsOnOneItem_ExecutedWithFailedItemHavingTheException ()
        {
            // arrange
            var result = new ConcurrentBag<SplitJoinResult<string, char>> ();

            var split_block = DataflowSplitJoin.CreateSplitBlockAsync<string, char>
                (async s =>
                       {
                           await Task.Yield ();
                           return s.ToCharArray ();
                       });

            var process_block = DataflowSplitJoin.CreateProcessBlockAsync<string, char>
                (async (s, c) =>
                       {
                           await Task.Yield ();

                           if (c == 'b')
                               throw new TestException ();
                       });

            var join_block = DataflowSplitJoin.CreateJoinBlock<string, char> ();

            var final_block = new ActionBlock<SplitJoinResult<string, char>>
                (async x =>
                       {
                           await Task.Yield ();
                           result.Add (x);
                       });


            split_block.LinkWithCompletionPropagation (process_block);
            process_block.LinkWithCompletionPropagation (join_block);
            join_block.LinkWithCompletionPropagation (final_block);


            // act
            foreach (var str in new[] { "a", "bc" })
                await split_block.SendAsync (str);

            split_block.Complete ();
            await Task.WhenAll (split_block.Completion, final_block.Completion);


            // assert
            result.Should().BeEquivalentTo
                (new[]
                 {
                     new SplitJoinResult<string, char>
                         ("a",
                          new[] { 'a' },
                          new SplitJoinFailedItem<char>[0],
                          1),
                     new SplitJoinResult<string, char>
                         ("bc",
                          new[] { 'c' },
                          new[] { new SplitJoinFailedItem<char> ('b', new TestException ()) },
                          2)
                 },
                 options => options.Using<Exception> (x => x.Subject.Should ().BeOfType<TestException> ())
                                   .WhenTypeIs<Exception> ());
        }


        [Fact]
        public async Task SplitProcessTransformJoinAction_ProcessThrowsOnOneItem_ExecutedWithFailedItemHavingTheException ()
        {
            // arrange
            var result = new ConcurrentBag<SplitJoinResult<string, string>> ();

            var split_block = DataflowSplitJoin.CreateSplitBlockAsync<string, char>
                (async s =>
                       {
                           await Task.Yield ();
                           return s.ToCharArray ();
                       });

            var process_block = DataflowSplitJoin.CreateProcessBlockAsync<string, char>
                (async (s, c) =>
                       {
                           await Task.Yield ();

                           if (c == 'b')
                               throw new TestException ();
                       });

            var transform_block = DataflowSplitJoin.CreateTransformBlockAsync<string, char, string>
                (async (s, c) =>
                       {
                           await Task.Yield ();
                           return c.ToString (CultureInfo.InvariantCulture);
                       });

            var join_block = DataflowSplitJoin.CreateJoinBlock<string, string> ();
            var final_block = new ActionBlock<SplitJoinResult<string, string>> (x => result.Add (x));


            split_block.LinkWithCompletionPropagation (process_block);
            process_block.LinkWithCompletionPropagation (transform_block);
            transform_block.LinkWithCompletionPropagation (join_block);
            join_block.LinkWithCompletionPropagation (final_block);


            // act
            foreach (var str in new[] { "a", "bc" })
                await split_block.SendAsync (str);

            split_block.Complete ();
            await Task.WhenAll (split_block.Completion, final_block.Completion);


            // assert
            result.Should().BeEquivalentTo
                (new[]
                 {
                     new SplitJoinResult<string, string> ("a",
                                                          new[] { "a" },
                                                          new SplitJoinFailedItem<string>[0],
                                                          1),
                     new SplitJoinResult<string, string> ("bc",
                                                          new[] { "c" },
                                                          new[] { new SplitJoinFailedItem<string> (null, new TestException ()) },
                                                          2)
                 },
                 options => options.Using<Exception> (x => x.Subject.Should ().BeOfType<TestException> ())
                                   .WhenTypeIs<Exception> ());
        }


        [Fact]
        public async Task SplitJoin_SplitReturnsNull_DoesNotThrow ()
        {
            // arrange
            var exceptions = new ConcurrentBag<Exception> ();

            var split_block = DataflowSplitJoin.CreateSplitBlockAsync<string, char>
                (async s =>
                       {
                           await Task.Yield ();
                           return null;
                       },
                 defaultExceptionLogger: (ex, obj) => exceptions.Add (ex));

            var join_block = DataflowSplitJoin.CreateFinalJoinBlock<string, char> ();

            split_block.LinkWithCompletionPropagation (join_block);


            // act
            await split_block.SendAsync ("abc");

            split_block.Complete ();
            await Task.WhenAll (split_block.Completion, join_block.Completion);


            // assert
            exceptions.Should ().BeEmpty ();
        }
    }
}


