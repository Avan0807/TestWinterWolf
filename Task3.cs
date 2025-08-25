using System;

public static class Task3
{
	public static int Task3(int[] A)
	{
		Array.Sort(A);
		long moves = 0;
		for (int i = 0; i < A.Length; i++)
		{
			moves += Math.Abs((long)A[i] - (i + 1L));
			if (moves > 1_000_000_000L) return -1;
		}

		return (int)moves;
	}
}
