using System;
using System.Collections.Generic;
using Monad;

namespace Monad
{

    class Program
    {

        static Monad<T, string> genericFunction<T>(Monad<T, int> m) where T : TMonad<T>, new()
        {
            var t = new T();
            return m.Bind<string>(i => t.Return((i * 5).ToString()));
        }

        static ListMonad<int> multiply(int i)
        {
            return TListMonad.FromEnumerable(new List<int> { i * 1, i * 2, i * 3, i * 4 });
        }

        static void Main(string[] args)
        {
            var perhaps = new List<Maybe<int>> { TMaybe.Some(5), TMaybe.Some(3), TMaybe.Some(7) };
            var orNot = new List<Maybe<int>> { TMaybe.Some(5), TMaybe.None<int>(), TMaybe.Some(7) };

            var seqSome = MonadUtils.Sequence(perhaps);
            var seqNone = MonadUtils.Sequence(orNot);

            var a = genericFunction(TMaybe.Some(4));
            var b = genericFunction(TMaybe.None<int>());
            var c = genericFunction(TListMonad.Val(5));
            var d = genericFunction(TListMonad.Empty<int>());
            var e = genericFunction(TListMonad.FromEnumerable(new List<int> { 1, 2, 3, 4, 5 }));

            var f = TListMonad.Val(5).Bind(multiply).Bind(multiply);
        }
    }
}
