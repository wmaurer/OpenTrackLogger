namespace OpenTrackLogger.Mixins
{
    using System;
    using System.Linq.Expressions;
    using System.Reactive.Linq;

    using ReactiveUI;

    public static class ObservableMixins
    {
        public static IObservable<TSender> WhereNotNull<TSender>(this IObservable<TSender> This)
            where TSender : class
        {
            if (This == null)
                throw new ArgumentNullException("This");

            return This.Where(x => x != null);
        }

        public static IObservable<TRet> WhenAnyNotNullValue<TSender, TRet>(this TSender This, Expression<Func<TSender, TRet>> property1)
            where TSender : class
            where TRet : class
        {
            if (This == null)
                throw new ArgumentNullException("This");
            if (property1 == null)
                throw new ArgumentNullException("property1");

            return This.WhenAny(property1, c1 => c1.Value).WhereNotNull();
        }
    }
}
