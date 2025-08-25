using System;

public static class Task1
{
	public static String Task1(string S)
	{
		if (string.IsNullOrEmpty(S)) || S.length == 1) ) return "";
		int n = S.Length; remove = n -1 ;
		for (int i = 0; i < n - 1 ; i++)
			if (S[i] > S[i + 1] {remove = i ; break;}
		return string.Concat(S.AsSpan(0, remove), S.AsSpan(remove + 1));
	}
}
