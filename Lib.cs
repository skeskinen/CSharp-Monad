using System;
using System.Collections.Generic;
using System.Linq;

namespace Monad
{
    abstract class TMonad<T> where T : TMonad<T>
    {
        public abstract Monad<T, B> Return<B>(B val);
    }

    abstract class Monad<T, A> where T : TMonad<T>
    {
        public abstract Monad<T, B> Bind<B>(Func<A, Monad<T, B>> f);
    }

    static class MonadUtils
    {
        static public Monad<T, List<A>> Sequence<T, A>(IEnumerable<Monad<T, A>> seq) where T : TMonad<T>, new()
        {
            var t = new T();
            return seq.Aggregate(t.Return(new List<A>()), (p, q) => p.Bind(x => q.Bind<List<A>>(y => t.Return(x.Concat(new[] { y }).ToList()))));
        }

        static public Monad<T, List<B>> MapM<T, A, B>(Func<A, Monad<T, B>> f, IEnumerable<A> xs) where T : TMonad<T>, new()
        {
            return Sequence(xs.Select(f));
        }
    }


    class TListMonad : TMonad<TListMonad>
    {
        public override Monad<TListMonad, A> Return<A>(A val)
        {
            return new ListMonad<A> { list = new List<A> { val } };
        }

        public static ListMonad<A> Empty<A>()
        {
            return new ListMonad<A> { list = new List<A>() };
        }

        public static ListMonad<A> Val<A>(A val)
        {
            return new ListMonad<A> { list = new List<A> { val } };
        }

        public static ListMonad<A> FromEnumerable<A>(IEnumerable<A> xs)
        {
            return new ListMonad<A> { list = xs.ToList() };
        }
    }

    class ListMonad<A> : Monad<TListMonad, A>
    {
        public List<A> list;

        public override Monad<TListMonad, B> Bind<B>(Func<A, Monad<TListMonad, B>> f)
        {
            var listMonads = list.Select(f).Select(m => m as ListMonad<B>);
            return new ListMonad<B> { list = listMonads.Aggregate(new List<B>(), (acc, l) => { acc.AddRange(l.list); return acc; }) };
        }
    }

    class TMaybe : TMonad<TMaybe>
    {
        public override Monad<TMaybe, A> Return<A>(A val)
        {
            return new Maybe<A> { some = true, val = val };
        }

        public static Maybe<A> Some<A>(A val)
        {
            return new Maybe<A> { some = true, val = val };
        }

        public static Maybe<A> None<A>()
        {
            return new Maybe<A> { some = false };
        }
    }

    class Maybe<A> : Monad<TMaybe, A>
    {
        public Boolean some;
        public A val;

        public override Monad<TMaybe, B> Bind<B>(Func<A, Monad<TMaybe, B>> f)
        {
            if (some)
                return f(val) as Maybe<B>;
            else
                return new Maybe<B> { some = false };
        }
    }
}