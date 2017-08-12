using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NovelTheory
{
	public class Bucket<TId, TRange>  where TRange : IComparable<TRange>
	{
		public TId Id { get; private set; }
		public  TRange Lower { get; private set; }
		public TRange Upper { get; private set; }

		public Bucket(TId id, TRange lower, TRange upper)
		{
			Id = id;
			Lower = lower;
			Upper = upper;
		}

		public virtual bool  Contains(TRange test)
		{
			return Lower.CompareTo(test) <= 0 && test.CompareTo(Upper) < 0;
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}-{2}", Id, Lower, Upper);
		}

		public virtual bool Overlaps(Bucket<TId, TRange> higher)
		{
			return this.Upper.CompareTo(higher.Lower) > 0;
		}
	}


	public class BucketInclusive<TId, TRange>  : Bucket<TId, TRange>
		where TRange : IComparable<TRange>
	{
		public BucketInclusive(TId id, TRange lower, TRange upper) : base(id, lower, upper)
		{
		}

		public override bool Contains(TRange test)
		{
			return Lower.CompareTo(test) <= 0 && test.CompareTo(Upper) <= 0;
		}

		public override bool Overlaps(Bucket<TId, TRange> higher)
		{
			return this.Upper.CompareTo(higher.Lower) >= 0;
		}

	}

	public class BucketBelow<TId, TRange> : Bucket<TId, TRange>
	where TRange : IComparable<TRange>
	{
		public BucketBelow(TId id, TRange upper) : base(id, default(TRange), upper)
		{
		}

		public override bool Contains(TRange test)
		{
			return test.CompareTo(Upper) <= 0;
		}

		public override bool Overlaps(Bucket<TId, TRange> higher)
		{
			return this.Upper.CompareTo(higher.Lower) > 0;
		}

		public override string ToString()
		{
			return string.Format("{0}: Up to {1}", Id, Upper);
		}
	}


	public static class Bucket
	{
		/// <summary>
		/// Create a new Bucket object without have to specify those ugly type arguments
		/// </summary>
		/// <typeparam name="TId">Data type of the bucket ID</typeparam>
		/// <typeparam name="TRange">Data Type of the bucket range</typeparam>
		/// <param name="id">id for this bucket</param>
		/// <param name="lower">Inclusive lower bound</param>
		/// <param name="upper">EXCLUSIVE upper bound.</param>
		/// <returns></returns>
		public static Bucket<TId, TRange> New<TId, TRange>(TId id, TRange lower, TRange upper) where TRange : IComparable<TRange>
		{
			return new Bucket<TId, TRange>(id, lower, upper);
		}

		public static Bucket<TId, TRange> Inclusive<TId, TRange>(TId id, TRange lower, TRange upper) where TRange : IComparable<TRange>
		{
			return new BucketInclusive<TId, TRange>(id, lower, upper);
		}

		public static Bucket<TId, TRange> Below<TId, TRange>(TId id, TRange upper) where TRange : IComparable<TRange>
		{
			return new BucketBelow<TId, TRange>(id,  upper);

		}

		/// <summary>
		/// Returns the ID of the bucket which the given item falls.  Buckets in array are assumed to by non-overlapping.
		/// </summary>
		/// <typeparam name="TId"></typeparam>
		/// <typeparam name="TRange"></typeparam>
		/// <param name="buckets"></param>
		/// <param name="test"></param>
		/// <returns></returns>
		//public static TId Which<TId, TRange>(this IEnumerable<Bucket<TId, TRange>> buckets, TRange test)
		//where TRange : IComparable<TRange>
		//{
		//	return (from b in buckets
		//			where b.Contains(test)
		//			select b.Id).FirstOrDefault();
		//}


		public static Bucket<TId, TRange> Which<TId, TRange>(this IEnumerable<Bucket<TId, TRange>> buckets, TRange test)
		where TRange : IComparable<TRange>
		{
			return buckets.FirstOrDefault(b => b.Contains(test));
		}

		public static Bucket<TId, TRange>[] Array<TId, TRange>(params Bucket<TId, TRange>[] coll)
					where TRange : IComparable<TRange>

		{
			return coll;
		}


		public static bool Test<TId, TRange>(this IEnumerable<Bucket<TId, TRange>> buckets, TextWriter tw = null)
					where TRange : IComparable<TRange>
		{
			if (tw == null)
				tw = TextWriter.Null;
			bool result = true;

			if (buckets.Any())
			{
				var sorted = buckets.OrderBy(b => b.Lower).ToList();
				var test = sorted.First();
				foreach(var next in sorted.Skip(1))
				{
					if (test.Overlaps(next))
					{
						tw.WriteLine("Bucket [{0}] overlaps bucket [{1}]", test, next);
						result = false;
					}
					test = next;
				}

			}

			return result;
		}



	}


}