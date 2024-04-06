using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;


namespace Roulette.Scripts.General
{
    /**
     * https://forum.unity.com/threads/awaitable-feature-requests-bugs.1434892/#post-9641099
     */
    public static class Awaitables
    {
        //! Gets an Awaitable that has already completed successfully.
        public static Awaitable CompletedAwaitable
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var acs = new AwaitableCompletionSource();
                acs.SetResult();
                return acs.Awaitable;
            }
        }

        //! Gets a Awaitable that will complete when all of the supplied Awaitable have completed.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Awaitable WhenAll(params Awaitable[] awaitables)
        {
            Debug.Assert(awaitables != null);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static async Awaitable __AwaitAll(params Awaitable[] awaitables)
            {
                for (int i = 0; i < awaitables.Length; i++)
                {
                    Debug.Assert(awaitables[i] != null);
                    await awaitables[i];
                }
            }

            if (awaitables.Length == 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"Performance warning : awaiting an empty array of Awaitable");
#endif
                return CompletedAwaitable;
            }

            if (awaitables.Length == 1)
            {
                Debug.Assert(awaitables[0] != null);
                return awaitables[0];
            }

            return __AwaitAll(awaitables);
        }

        //! Gets a Awaitable that will complete when any of the supplied Awaitable have completed.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Awaitable WhenAny(params Awaitable[] awaitables)
        {
            Debug.Assert(awaitables != null);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static async Awaitable __AwaitAny(params Awaitable[] awaitables)
            {
                AwaitableCompletionSource awaited = new AwaitableCompletionSource();

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                async Awaitable __WaitCompletion(Awaitable awaitable)
                {
                    await awaitable;
                    if (awaited != null)
                    {
                        awaited.SetResult();
                        awaited = null;
                    }
                }

                for (int i = 0; i < awaitables.Length; i++)
                {
                    Debug.Assert(awaitables[i] != null);
                    Run(__WaitCompletion(awaitables[i]));
                }

                if (awaited != null)
                    await awaited.Awaitable;
            }

            if (awaitables.Length == 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"Performance warning : awaiting an empty array of Awaitable");
#endif
                return CompletedAwaitable;
            }

            if (awaitables.Length == 1)
                return awaitables[0];

            return __AwaitAny(awaitables);
        }

        static FieldInfo __continuationFieldInfo =
            typeof(Awaitable).GetField("_continuation", BindingFlags.NonPublic | BindingFlags.Instance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool __HasContinuation(Awaitable awaitable)
            => __continuationFieldInfo.GetValue(awaitable) != null;

        //! Runs an Awaitable without awaiting for it.
        //! On completion, rethrow any exception raised during execution.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(this Awaitable self)
        {
            Debug.Assert(self != null);
            Debug.Assert(!__HasContinuation(self), "Awaitable already have a continuation, is it already awaited?");

            var awaiter = self.GetAwaiter();
            awaiter.OnCompleted(() => awaiter.GetResult());
        }


        //! Run multiple Awaitable without awaiting for any.
        //! On completion, rethrow any exception raised during execution.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(IEnumerable<Awaitable> list)
        {
            Debug.Assert(list != null);

            foreach (var item in list)
            {
                Debug.Assert(item != null);
                var awaiter = item.GetAwaiter();
                awaiter.OnCompleted(() => awaiter.GetResult());
            }
        }

        //! Create an Awaitable that first await the supplied Awaitable, then execute the continuation, once completed.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Awaitable WithContinuation(this Awaitable self, Action continuation)
        {
            Debug.Assert(self != null);
            Debug.Assert(continuation != null);

            if (!self.IsCompleted)
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                async Awaitable __AwaitAndContinue()
                {
                    await self;
                    continuation();
                }

                return __AwaitAndContinue();
            }
            else
            {
                continuation();
                return self;
            }
        }

        //! Set a continuation, executed once the Awaitable has completed.
        //! Note that continuation will be overwritten if Awaitable is awaited.
        //! This is an unusual method to use, be sure what you are doing.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ContinueWith(this Awaitable self, Action continuation)
        {
            Debug.Assert(self != null);
            Debug.Assert(continuation != null);

            if (!self.IsCompleted)
            {
                var awaiter = self.GetAwaiter();
                awaiter.OnCompleted(() =>
                {
                    continuation();
                    awaiter.GetResult();
                });
            }
            else continuation();
        }

        // For debugging purpose only:
        //

        /*
        [MethodImpl ( MethodImplOptions.AggressiveInlining )]
        public static IntPtr GetHandle ( this Awaitable awaitable )
        {
            Debug.Assert ( awaitable != null );
            var handle = (IntPtr)awaitable.GetFieldValue ( "_handle" ).GetFieldValue ( "_handle" );
            return handle;
        }

        public static bool IsManagedAwaitableDone ( this Awaitable awaitable )
        {
            Debug.Assert ( awaitable != null );
            return (bool)awaitable.GetFieldValue ( "_managedAwaitableDone" );
        }

        public static Exception GetException ( this Awaitable awaitable )
        {
            Debug.Assert ( awaitable != null );
            var edi = (ExceptionDispatchInfo)awaitable.GetFieldValue ( "_exceptionToRethrow" );
            if ( edi != null )
                return edi.SourceException;
            return null;
        }
        */
    }
}