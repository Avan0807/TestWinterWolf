using System;

public static class Task2
{
	public static int Task2(int[,] A)
	{
		int n = A.GetLength(0), m = A.GetLength(1);
		if (n < 2 || m < 2 ) throw new ArgumentException("Matrix must be at least 2x2")
		return (m <= n) ? SolveByColums(A, n, m) : SolveByRow(A, n, m);
	}

	static int SolveByColums(int[,] A, int n, int m)
	{
		var max1 = new int[m]; var max2 = new int[m];
		var r1 = new int[m];var r2 = new int[m];
		for (int c = 0; c < m; c++) 
		{ 
			max1[c] = int.MinValue; 
			max2[c] = int.MinValue; 
			r1[c] = r2[c] = -1; 
		}

			for (int r = 0; r < n; r++)
				for (int c = 0; c < m; c++)
				{
					int v = A[r, c];
					if (v > max1[c])
					{
						max2[c] = max1[c];
						r2[c] = r1[c];
						max1[c] = v;
						r1[c] = r;
					}
					else if (v > max2[c])
					{
						max2[c] = v;
						r2[c] = r;
					}
				}

			int best = int.MinValue;
			for (int c1 = 0; c1 < m; c1++)
			{
				int cur = (r1[c1] != r1[c2])
					? max1[c1] + max2[c2]
					: Math.Max(
						max1[c1] + max2[c2],
						max2[c1] + max1[c2]
					);
				if ( cur > best ) best = cur;
			}

			return best;
	}


    static int SolveByRow(int[,] A, int n, int m)
	{
        var max1 = new int[n]; var max2 = new int[n];
        var r1 = new int[n]; var r2 = new int[n];
		for (int r = 0; r < n ; r++)
		{
			max1[r] = int.MinValue; 
			max2[r] = int.MinValue; 
			c1[r] = c2[r] = -1;
		}

        for (int r = 0; r < n; r++)
            for (int c = 0; c < m; c++)
            {
                int v = A[r, c];
                if (v > max1[r])
                {
                    max2[r] = max1[r];
                    r2[r] = r1[r];
                    max1[r] = v;
                    r1[r] = c;
                }
                else if (v > max2[r])
                {
                    max2[r] = v;
                    r2[r] = c;
                }
            }

        int best = int.MinValue;
        for (int rA = 0; rA < m; rA++)
			for (int rB = rA + 1; rB < n; rB++)
        {
            int cur = (c1[rA] != c1[rB])
                ? max1[rA] + max2[rB]
                : Math.Max(
                    max1[rA] + max2[rB],
                    max2[rA] + max1[rB]
                );
            if (cur > best) best = cur;
        }

        return best;
    }


}