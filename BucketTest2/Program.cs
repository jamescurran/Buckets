using NovelTheory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BucketTest
{
	class Program
	{
		static void Main(string[] args)
		{
#if true
			var buckets = new Bucket<int, int>[] 
			{
				Bucket.Below(1, 10),
				Bucket.New(2, 10, 20),
				Bucket.New(3, 20, 30),
				Bucket.New(4, 30, 40)
			 };
			var rnd = new Random();

			var q = (from r in Enumerable.Range(1,400000).Select(r=> rnd.Next(0,40))
#elif true
			var buckets = new Bucket<string, DateTime>[] 
			{
				Bucket.New("180+", DateTime.Today.AddMonths(-12), DateTime.Today.AddMonths(-6)),
				Bucket.New("180-91", DateTime.Today.AddMonths(-6), DateTime.Today.AddMonths(-3)),
				Bucket.New("90-31", DateTime.Today.AddMonths(-3), DateTime.Today.AddMonths(-1)),
				Bucket.New("last month", DateTime.Today.AddMonths(-2), DateTime.Today.AddDays(1)),
			};
			var q = (from r in Enumerable.Range(1, 365).Select(r => DateTime.Today.AddDays(-r))
#elif true
			var buckets = Bucket.Array(
				Bucket.Inclusive(1, 0, 9),
				Bucket.Inclusive(2, 10, 19),
				Bucket.New(3, 20, 30),
				Bucket.Inclusive(4, 30, 39)
			 );
		var rnd = new Random();


		var q = (from r in Enumerable.Range(1, 1000000).Select(r => rnd.Next(0, 40))
#else
			var buckets = new Bucket<DateTime, double>[]
			{
				Bucket.New(DateTime.Today, 0, .25),
				Bucket.New(DateTime.Today.AddDays(1), .25, .5),
				Bucket.New(DateTime.Today.AddDays(2), .5, .75),
				Bucket.New(DateTime.Today.AddDays(3), .75, 1.0),
			 };
			var rnd = new Random();

			var q = (from r in Enumerable.Range(1, 400000).Select(r => rnd.NextDouble())

#endif
				 group r by buckets.Which(r) into g
					 select new { Bucket = g.Key, Count = g.Count() });

			foreach (var qq in q)
				Console.WriteLine("Bucket={0}, count = {1}", qq.Bucket, qq.Count);

			buckets.Test(Console.Out);

		}
	}
}